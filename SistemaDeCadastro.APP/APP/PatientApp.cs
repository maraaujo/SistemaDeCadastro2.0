using Microsoft.Data.SqlClient;
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
        public PatientApp(IPatientRepository patientRepository,
            IMedicinePatientIllnessRepository medicinePatientIllnessRepository
            )
        {
            this._patientRepository = patientRepository;
            this._medicinePatientIllnessRepository = medicinePatientIllnessRepository;

        }

        public async Task<List<Patient>> GetPatientById(long id)
            => await _patientRepository.GetPatientById(id);
        public async Task<List<PatientFilterDTO>> FilterPatient(PatientFilterDTO filter)
            => await _patientRepository.FilterPatient(filter);

        public async Task<List<DetailsPatientDTO>> DetailsPatient() =>
            await _patientRepository.DetailsPatient();

        public async Task<ApiResponse> GetMedicinesToMinister()
        {
            ApiResponse ret = new();
            //trazer todos os pacientes, horários, com sus respectivas doenças
            //medicamentos, dosagens e proximos horários
            try
            {
                //lista de opaciente com historico 
                var data = await this._patientRepository.GetMedicinesToMinister();
                //em cima dessa lista preencher a dto
                //foreach (var item in data)
                //{
                //    item.NextMedicineTime = item.MedicineHistoric.OrderByDescending(c => c.LastTime).First().LastTime;
                //    item.NextMedicineTime.AddHours(item.Time);

                //}
                ret.Data = data;
                return ret;

            }
            catch (Exception err)
            {

                throw;
            }
            return ret;
        }

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
                                Time = illness.Time
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

        public async Task<ApiResponse> DeletePatient(long id)
        {
            ApiResponse ret = new();

            try
            {

                var toDcelete = (await _medicinePatientIllnessRepository.FindBy(c => c.IdPatient == id)).ToList();
                await this._medicinePatientIllnessRepository.DeleteRange(toDcelete);


                Patient deletePatient = (await _patientRepository.GetPatientById(id)).FirstOrDefault();
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


