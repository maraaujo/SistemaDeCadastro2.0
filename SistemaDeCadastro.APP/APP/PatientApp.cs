using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
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
                if (patient.Cpf != null)
                {
                    var existingPatient =  _patientRepository.FindPatientByCPF(patient.Cpf);
                    ret.Success = false;
                    ret.ErrorMessage = "CPF já é atribuido a outro paciente";
                    return ret;
                }
                var clinicalConditionMap = new Dictionary<long, long>();
                if (patient.Id == 0)
                {
                    var newPatient = new Patient
                    {
                        Name = patient.Name,
                        BirthDate = patient.BirthDate,
                        Phone = patient.Phone,
                        Document = patient.Document,
                        Gender = patient.Gender,
                        Cpf = patient.Cpf,
                        Observations = patient.Observations,
                        CreatedAt = DateTime.Now,
                        InstitutionId = institutionId,
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
                                Phone = resposibleDto.Phone,
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
                            var newMedicine = new MedicinePatientClinicalCondition
                            {
                                PatientClinicalConditionId = clinicalConditionMap[medicineDto.ClinicalConditionId],
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
                    return ret;
                }
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
                var updatePatient = await this._patientRepository.GetByIdWithRelations(patient.Id);
                if (updatePatient != null)
                {
                    updatePatient.Name = patient.Name ?? updatePatient.Name;
                    updatePatient.Document = patient.Document ?? updatePatient.Document;
                    updatePatient.Phone = patient.Phone ?? updatePatient.Phone;
                    updatePatient.BloodTypeId = patient.BloodTypeId != 0 ? patient.BloodTypeId : updatePatient.BloodTypeId;

                    if (patient.BirthDate != null) updatePatient.BirthDate = patient.BirthDate ?? DateTime.Now; ;
                    if (!string.IsNullOrWhiteSpace(patient.Gender)) updatePatient.Gender = patient.Gender;
                    if (!string.IsNullOrWhiteSpace(patient.Cpf)) updatePatient.Cpf = patient.Cpf;
                    if (!string.IsNullOrWhiteSpace(patient.Observations)) updatePatient.Observations = patient.Observations;
                    await this._patientRepository.Update(updatePatient);

                    // Update responsibles
                    if (patient.Responsibles != null && patient.Responsibles.Any())
                    {
                        foreach (var responsibleDto in patient.Responsibles)
                        {
                            var existingResponsible = updatePatient.Responsibles.FirstOrDefault(r => r.Id == responsibleDto.Id);
                            if (existingResponsible != null)
                            {
                                existingResponsible.Name = responsibleDto.Name ?? existingResponsible.Name;
                                existingResponsible.Phone = responsibleDto.Phone ?? existingResponsible.Phone;
                                existingResponsible.Relationship = responsibleDto.Relationship ?? existingResponsible.Relationship;
                                existingResponsible.Address = responsibleDto.Address ?? existingResponsible.Address;
                                await _responsibleRepository.Update(existingResponsible);
                            }
                            else
                            {
                                var newResponsible = new Responsible
                                {
                                    PatientId = updatePatient.Id,
                                    Name = responsibleDto.Name,
                                    Phone = responsibleDto.Phone,
                                    Relationship = responsibleDto.Relationship,
                                    Address = responsibleDto.Address
                                };
                                await _responsibleRepository.Create(newResponsible);
                            }
                        }
                    }
                    // Update clinical conditions
                    if (patient.ClinicalConditions != null && patient.ClinicalConditions.Any())
                    {
                        foreach (var clinicalConditionDto in patient.ClinicalConditions)
                        {
                            var existingClinicalCondition = updatePatient.PatientClinicalConditions.FirstOrDefault(cc => cc.Id == clinicalConditionDto.Id);
                            if (existingClinicalCondition != null)
                            {
                                existingClinicalCondition.ClinicalConditionId = clinicalConditionDto.ClinicalConditionId != 0 ? clinicalConditionDto.ClinicalConditionId : existingClinicalCondition.ClinicalConditionId;
                                existingClinicalCondition.DiagnosisDate = clinicalConditionDto.DiagnosisDate ?? existingClinicalCondition.DiagnosisDate;
                                existingClinicalCondition.Observations = clinicalConditionDto.Observations ?? existingClinicalCondition.Observations;
                                await _patientClinicalConditionRepository.Update(existingClinicalCondition);
                            }
                            else
                            {
                                var newClinicalCondition = new PatientClinicalCondition
                                {
                                    PatientId = updatePatient.Id,
                                    ClinicalConditionId = clinicalConditionDto.ClinicalConditionId,
                                    DiagnosisDate = clinicalConditionDto.DiagnosisDate,
                                    Observations = clinicalConditionDto.Observations
                                };
                                await _patientClinicalConditionRepository.Create(newClinicalCondition);
                            }
                        }
                    }
                    // Update scheduled medicines
                    if (patient.ScheduledMedicines != null && patient.ScheduledMedicines.Any())
                    {
                        foreach (var medicineDto in patient.ScheduledMedicines)
                        {
                            var existingMedicine = updatePatient.PatientClinicalConditions
                                .SelectMany(cc => cc.Medicines)
                                .FirstOrDefault(m => m.Id == medicineDto.Id);
                            if (existingMedicine != null)
                            {
                                existingMedicine.MedicineId = medicineDto.MedicineId != 0 ? medicineDto.MedicineId : existingMedicine.MedicineId;
                                existingMedicine.ResponsibleEmployeeId = medicineDto.ResponsibleEmployeeId != 0 ? medicineDto.ResponsibleEmployeeId : existingMedicine.ResponsibleEmployeeId;
                                existingMedicine.Frequency = medicineDto.Frequency ?? existingMedicine.Frequency;
                                existingMedicine.StartDate = medicineDto.StartDate ?? existingMedicine.StartDate;
                                existingMedicine.EndDate = medicineDto.EndDate ?? existingMedicine.EndDate;
                                existingMedicine.Observations = medicineDto.Observations ?? existingMedicine.Observations;
                                existingMedicine.AdministrationTime = medicineDto.AdministrationTime ?? existingMedicine.AdministrationTime;
                                existingMedicine.PrescribedDosage = medicineDto.PrescribedDosage ?? existingMedicine.PrescribedDosage;
                                await _medicinePatientClinicalConditionRepository.Update(existingMedicine);
                            }
                            else
                            {
                                var newMedicine = new MedicinePatientClinicalCondition
                                {
                                    PatientClinicalConditionId = updatePatient.PatientClinicalConditions.FirstOrDefault()?.Id ?? 0,
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
                    }
                }

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
                // Delete care services directly linked to patient


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


