using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;

namespace SistemaDeCadastro.APP.APP
{
    public class PatientClinicalConditionApp : IPatientClinicalConditionApp
    {
        private readonly IPatientClinicalConditionRepository _repo;
        private readonly IPatientRepository _patientRepo;
        private readonly IMedicinePatientClinicalConditionRepository _medicineRepo;
        private readonly IMedicationAdministrationRepository _medicationAdministrationRepo;
        /// <summary>
        /// Recebe por injecao de dependencia os repositorios de condicao do acolhido, acolhido, medicamento programado e administracao.
        /// </summary>
        public PatientClinicalConditionApp(
            IPatientClinicalConditionRepository repo,
            IPatientRepository patientRepo,
            IMedicinePatientClinicalConditionRepository medicineRepo,
            IMedicationAdministrationRepository medicationAdministrationRepo)
        {
            _repo = repo;
            _patientRepo = patientRepo;
            _medicineRepo = medicineRepo;
            _medicationAdministrationRepo = medicationAdministrationRepo;
        }
        /// <summary>
        /// Devolve as condicoes clinicas vinculadas ao acolhido informado (montadas no repositorio).
        /// </summary>
        public async Task<PatientClinicalConditionDTO> GetPatientClinicalConditionByPatientId(long id)
        => await _repo.GetPatientClinicalConditionByPatientId(id);
        /// <summary>
        /// Devolve todos os vinculos acolhido + condicao clinica visiveis para a instituicao.
        /// </summary>
        public async Task<List<PatientClinicalCondition>> GetAll() => await _repo.GetAllReadOnly();

        /// <summary>
        /// Busca um vinculo acolhido + condicao clinica pelo Id; devolve null se nao existir.
        /// </summary>
        public async Task<PatientClinicalCondition?> GetById(long id) => (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();

        /// <summary>
        /// Vincula uma condicao clinica a um acolhido (data do diagnostico e observacoes).
        /// Recusa PatientId invalido ("Invalid patient ID") ou acolhido de outra instituicao.
        /// </summary>
        public async Task<ApiResponse> Create(CreatePatientClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                if(entity.PatientId <= 0)
                {
                    ret.ErrorMessage = "Id do acolhido inválido.";
                    ret.Success = false;
                    return ret;
                }

                // O acolhido vem do cliente: precisa ser da instituicao do usuario.
                if (!await _patientRepo.Any(p => p.Id == entity.PatientId))
                {
                    ret.ErrorMessage = "Acolhido não encontrado para a instituição do usuário logado.";
                    ret.Success = false;
                    return ret;
                }

                var patientClinicalCondition = new PatientClinicalCondition
                {
                    PatientId = entity.PatientId,
                    ClinicalConditionId = entity.ClinicalConditionId,
                    DiagnosisDate = entity.DiagnosisDate,
                    Observations = entity.Observations
                };

                await _repo.Create(patientClinicalCondition);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.ErrorMessage = ApiErrors.From(ex);
                ret.Success = false;
            }
            return ret;
        }

        /// <summary>
        /// Atualiza acolhido, condicao clinica, data do diagnostico e observacoes de um vinculo existente.
        /// Retorna "Condicao clinica do acolhido nao encontrada." se nao existir e recusa trocar para acolhido de outra instituicao.
        /// </summary>
        public async Task<ApiResponse> Update(PatientClinicalCondition entity)
        {
            var ret = new ApiResponse();
            try
            {
                // Carrega o registro existente pelo filtro de instituicao em vez de
                // gravar a entidade recebida direto (que sobrescreveria qualquer id).
                var existing = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Condição clínica do acolhido não encontrada.";
                    return ret;
                }

                if (entity.PatientId != existing.PatientId &&
                    !await _patientRepo.Any(p => p.Id == entity.PatientId))
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Acolhido não encontrado para a instituição do usuário logado.";
                    return ret;
                }

                existing.PatientId = entity.PatientId;
                existing.ClinicalConditionId = entity.ClinicalConditionId;
                existing.DiagnosisDate = entity.DiagnosisDate;
                existing.Observations = entity.Observations;
                await _repo.Update(existing);
                ret.Success = true;
            }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }

        /// <summary>
        /// Exclui o vinculo acolhido + condicao clinica, apagando antes os medicamentos programados dele e as
        /// administracoes desses medicamentos. Retorna Success = true mesmo se o Id nao existir.
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try
            {
                var e = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();
                if (e != null)
                {
                    // Remove os dependentes antes, senão as FKs bloqueiam a exclusão:
                    // condição clínica <- medicamentos <- administrações de medicamento.
                    var medicines = await _medicineRepo.FindBy(m => m.PatientClinicalConditionId == id);
                    if (medicines.Any())
                    {
                        var medicineIds = medicines.Select(m => m.Id).ToList();
                        var administrations = await _medicationAdministrationRepo
                            .FindBy(a => medicineIds.Contains(a.MedicinePatientClinicalConditionId));
                        if (administrations.Any())
                            await _medicationAdministrationRepo.DeleteRange(administrations);

                        await _medicineRepo.DeleteRange(medicines);
                    }

                    await _repo.Delete(e);
                }
                ret.Success = true;
            }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }
        /// <summary>
        /// Repassa o filtro ao repositorio e devolve a lista paginada de condicoes clinicas dos acolhidos.
        /// </summary>
        public async Task<PagedPatientClinicalConditionDTO> GetPatientClinicalConditionByFilter(PatientClinicalConditionFilterDTO filter)
        {
            return await _repo.GetPatientClinicalConditionByFilter(filter);
        }
    }
}
