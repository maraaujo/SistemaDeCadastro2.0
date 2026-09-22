using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.Validators;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.APP.APP
{
    public class PatientApp : IPatientApp
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ICurrentUserServiceContext _currentUserService;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IPatientEmployeeRepository _patientEmployeeRepository;
        private readonly IPatientClinicalConditionRepository _patientClinicalConditionRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IMedicinePatientClinicalConditionRepository _medicinePatientClinicalConditionRepository;
        public PatientApp(
            IPatientRepository patientRepository,
            IResponsibleRepository responsibleRepository,
            IPatientEmployeeRepository patientEmployeeRepository,
            IPatientClinicalConditionRepository patientClinicalConditionRepository,
            IAppointmentRepository appointmentRepository,
            IPaymentRepository paymentRepository,
            IMedicinePatientClinicalConditionRepository medicinePatientClinicalConditionRepository,
            IMedicineRepository _medicineRepository,
            ICurrentUserServiceContext currentUserService
            )
        {
            this._patientRepository = patientRepository;
            this._responsibleRepository = responsibleRepository;
            this._patientEmployeeRepository = patientEmployeeRepository;
            this._patientClinicalConditionRepository = patientClinicalConditionRepository;
            this._appointmentRepository = appointmentRepository;
            this._paymentRepository = paymentRepository;
            this._medicineRepository = _medicineRepository;
           this._medicinePatientClinicalConditionRepository = medicinePatientClinicalConditionRepository;
           this._currentUserService = currentUserService;
        }
    

        public async Task<List<Patient>> GetAllPatients()
            => await _patientRepository.GetAll();

        public async Task<List<Patient>> GetPatientById(long id)
            => await _patientRepository.GetPatientById(id);
        public async Task<PagedPatientDTO> FilterPatient(PatientFilterDTO filter)
            => await _patientRepository.FilterPatient(filter);

        public async Task<DetailsPatientDTO?> DetailsPatient(long id) =>
            await _patientRepository.DetailsPatient(id);

        public async Task<ApiResponse> CreatePatient(CreatePatientDTO patient)
        {
            ApiResponse ret = new();
            try
            {
                var institutionId = _currentUserService.InstitutionId;

                if (!institutionId.HasValue)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Não foi possível identificar a instituição do usuário logado.";
                    return ret;
                }
                if (!string.IsNullOrWhiteSpace(patient.Cpf))
                {
                    if (!CpfValidator.IsValid(patient.Cpf))
                    {
                        ret.Success = false;
                        ret.ErrorMessage = CpfValidator.MensagemInvalido;
                        return ret;
                    }

                    // Armazena apenas os números; a máscara fica para o frontend.
                    patient.Cpf = CpfValidator.Normalize(patient.Cpf);

                    var existingPatient = await _patientRepository.FindPatientByCPF(patient.Cpf, institutionId.Value);
                    if (existingPatient != null)
                    {
                        ret.Success = false;
                        ret.ErrorMessage = "CPF já é atribuido a outro paciente";
                        return ret;
                    }
                }

                // Valida os telefones dos responsáveis antes de criar qualquer registro.
                if (patient.Responsibles != null)
                {
                    foreach (var responsibleDto in patient.Responsibles)
                    {
                        if (!string.IsNullOrWhiteSpace(responsibleDto.Phone) &&
                            !PhoneValidator.IsValid(responsibleDto.Phone))
                        {
                            ret.Success = false;
                            ret.ErrorMessage = PhoneValidator.MensagemInvalido;
                            return ret;
                        }
                    }
                }
                var clinicalConditionMap = new Dictionary<long, long>();
                if (patient.Id == 0)
                {
                    var newPatient = new Patient
                    {
                        Name = patient.Name,
                        BirthDate = patient.BirthDate,
                        Gender = patient.Gender,
                        Cpf = patient.Cpf,
                        Observations = patient.Observations,
                        CreatedAt = DateTime.Now,
                        InstitutionId = institutionId.Value,
                        BloodTypeId = patient.BloodTypeId
                    };

                    await _patientRepository.CreatePatient(newPatient);

                    //se for diferente de nulo e tiver algum responsável, cria os responsáveis
                    if (patient.Responsibles != null && patient.Responsibles.Any())
                    {
                        //percorre a lista de responsáveis
                        //e cria cada um deles, associando ao paciente recém-criado
                        foreach (var resposibleDto in patient.Responsibles)
                        {
                            //cria um novo responsável com os dados do DTO
                            //e o ID do paciente recém-criado
                            var responsible = new Responsible
                            {
                                PatientId = newPatient.Id,
                                Name = resposibleDto.Name,
                                Phone = string.IsNullOrWhiteSpace(resposibleDto.Phone)
                                    ? resposibleDto.Phone
                                    : PhoneValidator.Normalize(resposibleDto.Phone),
                                Relationship = resposibleDto.Relationship,
                                Address = resposibleDto.Address
                            };
                            await _responsibleRepository.Create(responsible);
                        }

                    }
                    if (patient.ClinicalConditions != null && patient.ClinicalConditions.Any())
                    {
                        //percorre a lista de condições clinicas
                        //e cria cada um deles, associando ao paciente recém-criado
                        foreach (var clinicalCoDto in patient.ClinicalConditions)
                        {
                            var newClinical = new PatientClinicalCondition
                            {
                                PatientId = newPatient.Id,
                                ClinicalConditionId = clinicalCoDto.ClinicalConditionId,
                                DiagnosisDate = clinicalCoDto.DiagnosisDate,
                                Observations = clinicalCoDto.Observations
                            };
                            await _patientClinicalConditionRepository.Create(newClinical);
                            clinicalConditionMap[clinicalCoDto.ClinicalConditionId] = newClinical.Id;
                        }
                    }

                    if (patient.ScheduledMedicines != null && patient.ScheduledMedicines.Any())
                    {
                        foreach (var medicineDto in patient.ScheduledMedicines)
                        {
                            // resolve a condição clínica do medicamento; se não vier
                            // corretamente (chave ausente/0), usa a primeira condição criada
                            // — mesmo comportamento do UpdatePatient. Sem nenhuma condição,
                            // não há como associar o medicamento.
                            if (!clinicalConditionMap.TryGetValue(medicineDto.ClinicalConditionId, out var patientClinicalConditionId))
                            {
                                if (clinicalConditionMap.Count == 0)
                                {
                                    ret.Success = false;
                                    ret.ErrorMessage = "Não é possível cadastrar medicamento sem uma condição clínica associada.";
                                    return ret;
                                }
                                patientClinicalConditionId = clinicalConditionMap.Values.First();
                            }

                            var newMedicine = new MedicinePatientClinicalCondition
                            {
                                PatientClinicalConditionId = patientClinicalConditionId,
                                MedicineId = medicineDto.MedicineId,
                                ResponsibleEmployeeId = medicineDto.ResponsibleEmployeeId,
                                Frequency = medicineDto.Frequency,
                                StartDate = medicineDto.StartDate,
                                EndDate = medicineDto.EndDate,
                                Observations = medicineDto.Observations,
                                AdministrationTime = medicineDto.AdministrationTime,
                                PrescribedDosage = medicineDto.PrescribedDosage
                            };
                            await _medicinePatientClinicalConditionRepository.Create(newMedicine);
                        }

                    }

                    ret.Success = true;
                    return ret;
                }

                // patient.Id != 0 não é um cadastro novo
                ret.Success = false;
                ret.ErrorMessage = "Id inválido para criação de paciente.";
            }

            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }


        public async Task<ApiResponse> UpdatePatient(PatientDTO patient)
        {
            ApiResponse ret = new();

            try
            {
                var updatePatient =
                    await _patientRepository.GetByIdWithRelations(patient.Id);

                if (updatePatient == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Paciente não encontrado.";
                    return ret;
                }

                // Valida os telefones dos responsáveis antes de aplicar qualquer alteração.
                if (patient.Responsibles != null)
                {
                    foreach (var responsibleDto in patient.Responsibles)
                    {
                        if (!string.IsNullOrWhiteSpace(responsibleDto.Phone) &&
                            !PhoneValidator.IsValid(responsibleDto.Phone))
                        {
                            ret.Success = false;
                            ret.ErrorMessage = PhoneValidator.MensagemInvalido;
                            return ret;
                        }
                    }
                }

                // ============================
                // DADOS DO PACIENTE
                // ============================

                if (!string.IsNullOrWhiteSpace(patient.Name))
                    updatePatient.Name = patient.Name;

        

                if (patient.BloodTypeId.HasValue && patient.BloodTypeId.Value != 0)
                    updatePatient.BloodTypeId = patient.BloodTypeId.Value;

                if (patient.BirthDate.HasValue)
                    updatePatient.BirthDate = patient.BirthDate.Value;

                if (!string.IsNullOrWhiteSpace(patient.Gender))
                    updatePatient.Gender = patient.Gender;

                if (!string.IsNullOrWhiteSpace(patient.Cpf))
                {
                    if (!CpfValidator.IsValid(patient.Cpf))
                    {
                        ret.Success = false;
                        ret.ErrorMessage = CpfValidator.MensagemInvalido;
                        return ret;
                    }

                    // Armazena apenas os números; a máscara fica para o frontend.
                    updatePatient.Cpf = CpfValidator.Normalize(patient.Cpf);
                }

                if (!string.IsNullOrWhiteSpace(patient.Observations))
                    updatePatient.Observations = patient.Observations;

                await _patientRepository.Update(updatePatient);


                // ============================
                // RESPONSÁVEIS
                // ============================

                if (patient.Responsibles != null)
                {
                    foreach (var responsibleDto in patient.Responsibles)
                    {
                        var existingResponsible =
                            updatePatient.Responsibles
                                .FirstOrDefault(r => r.Id == responsibleDto.Id);

                        if (existingResponsible != null)
                        {
                            if (!string.IsNullOrWhiteSpace(responsibleDto.Name))
                                existingResponsible.Name = responsibleDto.Name;

                            if (!string.IsNullOrWhiteSpace(responsibleDto.Phone))
                                existingResponsible.Phone = PhoneValidator.Normalize(responsibleDto.Phone);

                            if (!string.IsNullOrWhiteSpace(responsibleDto.Relationship))
                                existingResponsible.Relationship = responsibleDto.Relationship;

                            if (!string.IsNullOrWhiteSpace(responsibleDto.Address))
                                existingResponsible.Address = responsibleDto.Address;

                            await _responsibleRepository.Update(existingResponsible);
                        }
                        else
                        {
                            var newResponsible = new Responsible
                            {
                                PatientId = updatePatient.Id,
                                Name = responsibleDto.Name,
                                Phone = string.IsNullOrWhiteSpace(responsibleDto.Phone)
                                    ? responsibleDto.Phone
                                    : PhoneValidator.Normalize(responsibleDto.Phone),
                                Relationship = responsibleDto.Relationship,
                                Address = responsibleDto.Address
                            };

                            await _responsibleRepository.Create(newResponsible);
                        }
                    }
                }


                // ============================
                // CONDIÇÕES CLÍNICAS
                // ============================

                if (patient.ClinicalConditions != null)
                {
                    foreach (var conditionDto in patient.ClinicalConditions)
                    {
                        var existingCondition =
                            updatePatient.PatientClinicalConditions
                                .FirstOrDefault(x => x.Id == conditionDto.Id);

                        if (existingCondition != null)
                        {
                            if (conditionDto.ClinicalConditionId != 0)
                                existingCondition.ClinicalConditionId =
                                    conditionDto.ClinicalConditionId;

                            if (conditionDto.DiagnosisDate.HasValue)
                                existingCondition.DiagnosisDate =
                                    conditionDto.DiagnosisDate.Value;

                            if (conditionDto.Observations != null)
                                existingCondition.Observations =
                                    conditionDto.Observations;

                            await _patientClinicalConditionRepository
                                .Update(existingCondition);
                        }
                        else
                        {
                            var newCondition = new PatientClinicalCondition
                            {
                                PatientId = updatePatient.Id,
                                ClinicalConditionId =
                                    conditionDto.ClinicalConditionId,

                                DiagnosisDate =
                                    conditionDto.DiagnosisDate,

                                Observations =
                                    conditionDto.Observations
                            };

                            await _patientClinicalConditionRepository
                                .Create(newCondition);
                        }
                    }
                }


                // ============================
                // MEDICAMENTOS
                // ============================

                if (patient.ScheduledMedicines != null)
                {
                    foreach (var medicineDto in patient.ScheduledMedicines)
                    {
                        var existingMedicine =
                            updatePatient.PatientClinicalConditions
                                .SelectMany(x => x.Medicines)
                                .FirstOrDefault(x => x.Id == medicineDto.Id);

                        if (existingMedicine != null)
                        {
                            if (medicineDto.MedicineId != 0)
                                existingMedicine.MedicineId =
                                    medicineDto.MedicineId;

                            if (medicineDto.ResponsibleEmployeeId != 0)
                                existingMedicine.ResponsibleEmployeeId =
                                    medicineDto.ResponsibleEmployeeId;

                            if (medicineDto.Frequency != null)
                                existingMedicine.Frequency =
                                    medicineDto.Frequency;

                            if (medicineDto.StartDate.HasValue)
                                existingMedicine.StartDate =
                                    medicineDto.StartDate.Value;

                            if (medicineDto.EndDate.HasValue)
                                existingMedicine.EndDate =
                                    medicineDto.EndDate.Value;

                            if (medicineDto.Observations != null)
                                existingMedicine.Observations =
                                    medicineDto.Observations;

                            if (medicineDto.AdministrationTime != null)
                                existingMedicine.AdministrationTime =
                                    medicineDto.AdministrationTime;

                            if (medicineDto.PrescribedDosage != null)
                                existingMedicine.PrescribedDosage =
                                    medicineDto.PrescribedDosage;

                            await _medicinePatientClinicalConditionRepository
                                .Update(existingMedicine);
                        }
                        else
                        {
                            var newMedicine =
                                new MedicinePatientClinicalCondition
                                {
                                    PatientClinicalConditionId =
                                        updatePatient
                                            .PatientClinicalConditions
                                            .FirstOrDefault()?.Id ?? 0,

                                    MedicineId =
                                        medicineDto.MedicineId,

                                    ResponsibleEmployeeId =
                                        medicineDto.ResponsibleEmployeeId,

                                    Frequency =
                                        medicineDto.Frequency,

                                    StartDate =
                                        medicineDto.StartDate,

                                    EndDate =
                                        medicineDto.EndDate,

                                    Observations =
                                        medicineDto.Observations,

                                    AdministrationTime =
                                        medicineDto.AdministrationTime,

                                    PrescribedDosage =
                                        medicineDto.PrescribedDosage
                                };

                            await _medicinePatientClinicalConditionRepository
                                .Create(newMedicine);
                        }
                    }
                }

                ret.Success = true;
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }
        public async Task<ApiResponse> DeletePatient(long id)
        {
            ApiResponse ret = new();

            try
            {
 


                // Find appointments for patient and delete related payments and care services
                var appointments = await _appointmentRepository.FindBy(a => a.PatientId == id);
                foreach (var ap in appointments)
                {
                    var payments = await _paymentRepository.FindBy(p => p.AppointmentId == ap.Id);
                    if (payments.Any()) await _paymentRepository.DeleteRange(payments);

                }
                if (appointments.Any()) await _appointmentRepository.DeleteRange(appointments);

                // Delete patient employees
                //var patientEmployees = await _patient_employeeRepository.FindBy(pe => pe.PatientId == id);
                //if (patientEmployees.Any()) await _patient_employeeRepository.DeleteRange(patientEmployees);

                // Delete patient clinical conditions and related medicine entries
                var patientClinicalConditions = await _patientClinicalConditionRepository.FindBy(pcc => pcc.PatientId == id);
                //foreach (var pcc in patientClinicalConditions)
                //{
                //    var meds = await _medicinePatientClinicalConditionRepository.FindBy(m => m.PatientClinicalConditionId == pcc.Id);
                //    if (meds.Any()) await _medicinePatientClinicalConditionRepository.DeleteRange(meds);
                //}
                if (patientClinicalConditions.Any()) await _patientClinicalConditionRepository.DeleteRange(patientClinicalConditions);



                // Delete responsibles
                var responsibles = await _responsibleRepository.FindBy(r => r.PatientId == id);
                if (responsibles.Any()) await _responsibleRepository.DeleteRange(responsibles);

                // Finally delete patient
                var deletePatient = (await _patientRepository.FindBy(p => p.Id == id)).FirstOrDefault();
                if (deletePatient != null) await _patientRepository.DeletePatient(deletePatient);

            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }
        public async Task GetPatientByAny(string patient) =>
            await this._patientRepository.GetPatientByAny(patient);

    }
}


