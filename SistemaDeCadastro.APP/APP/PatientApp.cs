using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.APP.APP
{
    public class PatientApp : IPatientApp
    {
        private readonly IPatientRepository _patientRepository; 
        private readonly IMedicinePatientIllnessRepository _medicinePatientIllnessRepository;   
        public PatientApp(IPatientRepository patientRepository, IMedicinePatientIllnessRepository medicinePatientIllnessRepository)
        {
            this._patientRepository = patientRepository;
            this._medicinePatientIllnessRepository = medicinePatientIllnessRepository;
        }

        public async Task<List<Patient>> GetPatientById(long id)
            => await _patientRepository.GetPatientById(id);
        public async Task<List<PatientFilterDTO>> FilterPatient(PatientFilterDTO filter)
            => await _patientRepository.FilterPatient(filter);


        public async Task<ApiResponse> CreatePatient(PatientDTO patient)
        {

            ApiResponse ret = new();

            try
            {
                Patient newPatient = new();
                newPatient.Id = patient.Id;
                newPatient.Name = patient.Name;
                newPatient.Document = patient.Document;
                newPatient.Responsible = patient.Responsible;
                newPatient.Phone = patient.Phone;
                newPatient.IdBloodType = patient.IdBloodType;
                await this._patientRepository.CreatePatient(newPatient);

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
                Patient updatedPatient = (await _patientRepository.GetPatientById(patient.Id)).FirstOrDefault();

                if (updatedPatient != null)
                {
                    updatedPatient.Name = patient.Name;
                    updatedPatient.Document = patient.Document;
                    updatedPatient.Responsible = patient.Responsible;
                    updatedPatient.Phone = patient.Phone;
                    updatedPatient.IdBloodType = patient.IdBloodType;
                }
                await this._patientRepository.UpdatePatient(updatedPatient);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }
            return ret;
        }

        public async Task<ApiResponse> DeletePatient(PatientDTO patient)
        {
            ApiResponse ret = new();

            try
            {
                Patient deletePatient = (await _patientRepository.GetPatientById(patient.Id)).FirstOrDefault();
                await this._patientRepository.DeletePatient(deletePatient);
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
       

