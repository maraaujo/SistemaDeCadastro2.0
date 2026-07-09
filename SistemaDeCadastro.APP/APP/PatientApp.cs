using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.APP.APP
{
    public class PatientApp : IPatientApp
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IPatientEmployeeRepository _patientEmployeeRepository;
        private readonly IPatientClinicalConditionRepository _patientClinicalConditionRepository;
        private readonly IPatientIllnessRepository _patientIllnessRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICareServiceRepository _careServiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        public PatientApp(
            IPatientRepository patientRepository,
            IResponsibleRepository responsibleRepository,
            IPatientEmployeeRepository patientEmployeeRepository,
            IPatientClinicalConditionRepository patientClinicalConditionRepository,
            IPatientIllnessRepository patientIllnessRepository,
            IAppointmentRepository appointmentRepository,
            ICareServiceRepository careServiceRepository,
            IPaymentRepository paymentRepository,
            IMedicinePatientClinicalConditionRepository medicinePatientClinicalConditionRepository
            )
        {
            this._patientRepository = patientRepository;
            this._responsibleRepository = responsibleRepository;
            this._patientEmployeeRepository = patientEmployeeRepository;
            this._patientClinicalConditionRepository = patientClinicalConditionRepository;
            this._patientIllnessRepository = patientIllnessRepository;
            this._appointmentRepository = appointmentRepository;
            this._careServiceRepository = careServiceRepository;
            this._paymentRepository = paymentRepository;
           // this._medicinePatientClinicalConditionRepository = medicinePatientClinicalConditionRepository;
        }

        public async Task<List<Patient>> GetPatientById(long id)
            => await _patientRepository.GetPatientById(id);
        public async Task<List<PatientFilterDTO>> FilterPatient(PatientFilterDTO filter)
            => await _patientRepository.FilterPatient(filter);

        public async Task<List<DetailsPatientDTO>> DetailsPatient() =>
            await _patientRepository.DetailsPatient();
        public async Task<List<MedicineReminderDTO>> GetMedicineReminders() =>
            await _patientRepository.GetMedicineReminders();
        public async Task<ApiResponse> CreatePatient(CreatepatientDTO patient)
        {
            ApiResponse ret = new();
            try
            {
                if (patient.Id == 0)
                {
                    var newPatient = new Patient
                    {
                        Name = patient.Name,
                        Document = patient.Document,
                        Phone = patient.Phone,
                        BloodTypeId = patient.BooldType,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _patientRepository.CreatePatient(newPatient);

                    if (!string.IsNullOrWhiteSpace(patient.Responsible))
                    {
                        var responsible = new Responsible
                        {
                            PatientId = newPatient.Id,
                            Name = patient.Responsible
                        };
                        await _responsibleRepository.Create(responsible);
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
                    updatePatient.BloodTypeId = patient.IdBloodType != 0 ? patient.IdBloodType : updatePatient.BloodTypeId;
                    // optional fields if provided in DTO
                    if (patient.BirthDate != null) updatePatient.BirthDate = patient.BirthDate.Value;
                    if (!string.IsNullOrWhiteSpace(patient.Gender)) updatePatient.Gender = patient.Gender;
                    if (!string.IsNullOrWhiteSpace(patient.Cpf)) updatePatient.Cpf = patient.Cpf;
                    if (!string.IsNullOrWhiteSpace(patient.Observations)) updatePatient.Observations = patient.Observations;

                    await this._patientRepository.Update(updatePatient);
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
                var careServices = await _careServiceRepository.FindBy(c => c.PatientId == id);
                if (careServices.Any()) await _careServiceRepository.DeleteRange(careServices);

                // Find appointments for patient and delete related payments and care services
                var appointments = await _appointmentRepository.FindBy(a => a.PatientId == id);
                foreach (var ap in appointments)
                {
                    var payments = await _paymentRepository.FindBy(p => p.AppointmentId == ap.Id);
                    if (payments.Any()) await _paymentRepository.DeleteRange(payments);

                    var services = await _careServiceRepository.FindBy(c => c.AppointmentId == ap.Id);
                    if (services.Any()) await _careServiceRepository.DeleteRange(services);
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

                // Delete patient illnesses
                var patientIllnesses = await _patientIllnessRepository.FindBy(pi => pi.PatientId == id);
                if (patientIllnesses.Any()) await _patientIllnessRepository.DeleteRange(patientIllnesses);

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


