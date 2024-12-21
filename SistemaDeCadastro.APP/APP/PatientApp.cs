using Microsoft.Data.SqlClient;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;
using System.Security.AccessControl;


namespace SistemaDeCadastro.APP.APP
{
    public class PatientApp : IPatientApp
    {
        private readonly IPatientRepository _patientRepository; 
        
        private readonly IMedicinePatientIllnessRepository _medicinePatientIllnessRepository;   
        private readonly IMedicinePatientIllnessHistoricRepository _medicinePatientIllnessHistoricRepository;
        private PatientRepository patientRepository;
        private object value;

        public PatientApp(IPatientRepository patientRepository,
            IMedicinePatientIllnessRepository medicinePatientIllnessRepository,
            IMedicinePatientIllnessHistoricRepository medicinePatientIllnessHistoricRepository
            )
        {
            this._patientRepository = patientRepository;
            this._medicinePatientIllnessRepository = medicinePatientIllnessRepository;
            this._medicinePatientIllnessHistoricRepository = medicinePatientIllnessHistoricRepository;
            
        }

        public PatientApp(PatientRepository patientRepository, object value)
        {
            this.patientRepository = patientRepository;
            this.value = value;
        }

        public async Task<List<Patient>> GetPatientById(long id)
            => await _patientRepository.GetPatientById(id);
        public async Task<List<PatientFilterDTO>> FilterPatient(PatientFilterDTO filter)
            => await _patientRepository.FilterPatient(filter);

        public async Task<List<DetailsPatientDTO>> DetailsPatient()=>
            await _patientRepository.DetailsPatient();

        public async Task<ApiResponse> CreatePatient(CreatepatientDTO patient)
        {

            ApiResponse ret = new();
           
            try
            {
                Patient newPatient = null;
                if (patient.Id == 0)
                {
                    newPatient = new Patient
                    {
                        Id = patient.Id,
                        Name = patient.Name,
                        IdBloodType = patient.BooldType,
                        Phone = patient.Phone,
                        Responsible = patient.Responsible,
                    };
                    await this._patientRepository.CreatePatient(newPatient);
                    foreach (var illness in patient.MedicinePatientIllnesses)
                    {
                        if (illness.IdIllness != 0)
                        {
                            var medicinePatientIllness = new MedicinePatientIllness
                            {
                               IdIllness = illness.IdIllness,
                                IdPatient = newPatient.Id, 
                                IdMedicine = illness.IdMedicine,
                                Dosage = illness.Dosage,
                                Time = illness.Time,
                            };
                            await this._medicinePatientIllnessRepository.CreateMedicinePatientIllness(medicinePatientIllness);
                           
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

        public async Task<ApiResponse> CreateNewDosageInterval(MedicinePatientIllnessHistoricDTO historicDTO)
        {
            ApiResponse ret = new();
            try
            {
                if (historicDTO.IdMedicinePatientIllness != 0)
                {
                    var medicinePatientIllnessHistoric = new MedicinePatientIllnessHistoric
                    {
                        Id = historicDTO.Id,
                        IdMedicinePatientIllness = historicDTO.IdMedicinePatientIllness,
                        LastTime = historicDTO.LastTime,
                    };
                    await this._medicinePatientIllnessHistoricRepository.Create(medicinePatientIllnessHistoric);
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
                Patient updatePatient = (await this._patientRepository.GetPatientById(patient.Id)).FirstOrDefault();

                if (updatePatient.Id != 0)
                {
                    updatePatient.Name = patient.Name;
                    updatePatient.Document = patient.Document;
                    updatePatient.Responsible = patient.Responsible;
                    updatePatient.Phone = patient.Phone;
                    updatePatient.IdBloodType = patient.IdBloodType;
                }
                await this._patientRepository.UpdatePatient(updatePatient);
              
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
            //SqlTransaction tran = .BeginTransaction();
            try
            {
                
                //MedicinePatientIllness deletemedicinePatientIllness = await _medicinePatientIllnessRepository.FindBy(c => c.IdPatient == id).FirstOrDefault();
                ////var list = await this._medicinePatientIllnessRepository.FindBy(c => c.IdPatient == id);
                //await this._medicinePatientIllnessRepository.DeleteRange(id);
                    
                //IMPLEMENTAR DELETE RANGE 

                Patient deletePatient = (await _patientRepository.GetPatientById(id)).FirstOrDefault();
                await this._patientRepository.DeletePatient(deletePatient);
                //tran.Commit();
            }
            catch (Exception err)
            {
                //tran.Rollback();
                
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }
        public async Task GetPatientByAny(string patient) =>
            await this._patientRepository.GetPatientByAny(patient);

    }
}

       

