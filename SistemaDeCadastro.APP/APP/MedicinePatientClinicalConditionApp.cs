using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;

namespace SistemaDeCadastro.APP.APP
{
    public class MedicinePatientClinicalConditionApp : IMedicinePatientClinicalConditionApp
    {
        private readonly IMedicinePatientClinicalConditionRepository _repo;
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IClinicalConditionRepository _clinicalConditionRepository;
        private readonly IPatientClinicalConditionRepository _patientClinicalConditionRepository;
        public MedicinePatientClinicalConditionApp(IMedicinePatientClinicalConditionRepository repo,
            IPatientRepository patientRepository, IMedicineRepository 
            medicineRepository, 
            IClinicalConditionRepository clinicalConditionRepository, IPatientClinicalConditionRepository patientClinicalConditionRepository)
        {
            _repo = repo;
            _patientRepository = patientRepository;
            _medicineRepository = medicineRepository;
            _clinicalConditionRepository = clinicalConditionRepository;
            _patientClinicalConditionRepository = patientClinicalConditionRepository;
        }

        public async Task<List<MedicinePatientClinicalCondition>> GetAll() => await _repo.GetAll();

        public async Task<MedicinePatientClinicalCondition?> GetById(long id) => (await _repo.FindBy(m => m.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateMedicinePatientClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                long medicineId;
                long patientClinicalConditionId;

                // 1. Se o medicamento não existir, cria um novo
                if (entity.MedicineDTO.Id == 0)
                {
                    var medicine = new Medicine
                    {
                        Name = entity.MedicineDTO.Name,
                        Dosage = entity.MedicineDTO.Dosage,
                        Description = entity.MedicineDTO.Description,
                        AdministrationRoute = entity.MedicineDTO.AdministrationRoute
                    };

                    await _medicineRepository.Create(medicine);

                    medicineId = medicine.Id;
                }
                else
                {
                    medicineId = entity.MedicineDTO.Id;
                }

                // 2. Se o vínculo acolhido + condição clínica não existir, cria
                if (entity.PatientClinicalConditionDTO.Id == 0)
                {
                    long clinicalConditionId;

                    // 2.1 Se a condição clínica não existir, cria uma nova
                    if (entity.PatientClinicalConditionDTO.ClinicalConditionId == 0)
                    {
                        var clinicalCondition = new ClinicalCondition
                        {
                            Name = entity.ClinicalConditionDTO.Name,
                            Description = entity.ClinicalConditionDTO.Description,
                            Type = entity.ClinicalConditionDTO.Type
                        };

                        await _clinicalConditionRepository.Create(clinicalCondition);

                        clinicalConditionId = clinicalCondition.Id;
                    }
                    else
                    {
                        clinicalConditionId = entity.PatientClinicalConditionDTO.ClinicalConditionId;
                    }

                    // 2.2 Cria o vínculo da condição com o acolhido
                     var patientClinicalCondition = new PatientClinicalCondition
                    {
                        PatientId = entity.PatientId,
                        ClinicalConditionId = clinicalConditionId,
                        DiagnosisDate = DateTime.Now,
                        Observations = entity.PatientClinicalConditionDTO.Observations
                    };

                    await _patientClinicalConditionRepository.Create(patientClinicalCondition);

                    patientClinicalConditionId = patientClinicalCondition.Id;
                }
                else
                {
                    patientClinicalConditionId = entity.PatientClinicalConditionDTO.Id;
                }

                // 3. Cria o medicamento programado
                var medicinePatientClinicalCondition = new MedicinePatientClinicalCondition
                {
                    MedicineId = medicineId,
                    PatientClinicalConditionId = patientClinicalConditionId,
                    PrescribedDosage = entity.PrescribedDosage,
                    Frequency = entity.Frequency,
                    AdministrationTime = entity.AdministrationTime,
                    ResponsibleEmployeeId = entity.ResponsibleEmployeeId,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    Observations = entity.Observations
                };

                await _repo.Create(medicinePatientClinicalCondition);

                ret.Success = true;
                ret.Data = medicinePatientClinicalCondition.Id;
                ret.Message = "Medicamento programado cadastrado com sucesso.";
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }  
            return ret;
        }

        public async Task<ApiResponse> Update(UpdateMedicinePatientClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {

                var existingEntity = (await _repo.FindBy(m => m.Id == entity.Id)).FirstOrDefault();
                existingEntity.MedicineId = entity.MedicineId;
                existingEntity.PatientClinicalConditionId = entity.PatientClinicalConditionId;
                existingEntity.PrescribedDosage = entity.PrescribedDosage;
                existingEntity.Frequency = entity.Frequency;
                existingEntity.AdministrationTime = entity.AdministrationTime;
                existingEntity.ResponsibleEmployeeId = entity.ResponsibleEmployeeId;
                existingEntity.StartDate = entity.StartDate;
                existingEntity.EndDate = entity.EndDate;
                existingEntity.Observations = entity.Observations;
                await _repo.Update(existingEntity);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, ErrorMessage = ex.Message };
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(m => m.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId)
        {
            return await _repo.GetByPatientClinicalConditionId(patientClinicalConditionId);
        }

        //implemntar job para ficar atualizando a lista de lembretes de medicamentos, e enviar notificação para o paciente
     
       public async Task<PagedMedicinePatientClinicalConditionDTO> GetMedicinePatientClinicalConditionByFilter(MedicinePatientClinicalConditionFilterDTO filter)
        {
            return await _repo.GetMedicinePatientClinicalConditionByFilter(filter);
        }
    }
}
