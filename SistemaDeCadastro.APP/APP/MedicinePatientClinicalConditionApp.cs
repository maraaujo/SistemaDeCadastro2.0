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
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMedicationAdministrationRepository _medicationAdministrationRepository;
        private readonly ICurrentUserServiceContext _currentUserService;
        /// <summary>
        /// Recebe por injecao de dependencia os repositorios envolvidos no medicamento programado e o usuario logado.
        /// </summary>
        public MedicinePatientClinicalConditionApp(IMedicinePatientClinicalConditionRepository repo,
            IPatientRepository patientRepository, IMedicineRepository
            medicineRepository,
            IClinicalConditionRepository clinicalConditionRepository, IPatientClinicalConditionRepository patientClinicalConditionRepository,
            IEmployeeRepository employeeRepository,
            IMedicationAdministrationRepository medicationAdministrationRepository,
            ICurrentUserServiceContext currentUserService)
        {
            _repo = repo;
            _patientRepository = patientRepository;
            _medicineRepository = medicineRepository;
            _clinicalConditionRepository = clinicalConditionRepository;
            _patientClinicalConditionRepository = patientClinicalConditionRepository;
            _employeeRepository = employeeRepository;
            _medicationAdministrationRepository = medicationAdministrationRepository;
            _currentUserService = currentUserService;
        }

        // Os ids de medicamento, condicao clinica do acolhido e funcionario vem do
        // cliente: todos precisam ser da instituicao do usuario (os repositorios ja
        // aplicam o filtro de instituicao). Id 0/nulo = nao informado.
        private async Task<string?> ValidateReferences(long medicineId, long patientClinicalConditionId, long? responsibleEmployeeId)
        {
            if (medicineId != 0 && !await _medicineRepository.Any(m => m.Id == medicineId))
                return "Medicamento não encontrado para a instituição do usuário logado.";

            if (patientClinicalConditionId != 0 &&
                !await _patientClinicalConditionRepository.Any(p => p.Id == patientClinicalConditionId))
                return "Condição clínica do acolhido não encontrada para a instituição do usuário logado.";

            if (responsibleEmployeeId.HasValue && responsibleEmployeeId.Value != 0 &&
                !await _employeeRepository.Any(e => e.Id == responsibleEmployeeId.Value))
                return "Funcionário não encontrado para a instituição do usuário logado.";

            return null;
        }

        /// <summary>
        /// Devolve todos os medicamentos programados (medicamento + condicao clinica do acolhido) visiveis para a instituicao.
        /// </summary>
        public async Task<List<MedicinePatientClinicalCondition>> GetAll() => await _repo.GetAllReadOnly();

        /// <summary>
        /// Busca um medicamento programado pelo Id; devolve null se nao existir.
        /// </summary>
        public async Task<MedicinePatientClinicalCondition?> GetById(long id) => (await _repo.FindBy(m => m.Id == id)).FirstOrDefault();

        /// <summary>
        /// Programa um medicamento para um acolhido (dosagem, frequencia, horario, responsavel, inicio/fim).
        /// Valida antes se medicamento, condicao do acolhido, funcionario e acolhido sao da instituicao; usuario comum
        /// so pode usar condicao clinica ja cadastrada no catalogo. Depois: reaproveita ou cria o medicamento (por nome),
        /// reaproveita ou cria a condicao clinica e o vinculo acolhido + condicao, e bloqueia o mesmo medicamento no
        /// mesmo horario para o mesmo vinculo ("Ja existe esse medicamento cadastrado nesse horario."). Devolve o Id criado em Data.
        /// </summary>
        public async Task<ApiResponse> Create(CreateMedicinePatientClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                long medicineId;
                long patientClinicalConditionId;

                // Valida tudo antes de criar qualquer registro.
                var referenceError = await ValidateReferences(
                    entity.MedicineDTO.Id,
                    entity.PatientClinicalConditionDTO.Id,
                    entity.ResponsibleEmployeeId);
                if (referenceError == null && entity.PatientClinicalConditionDTO.Id == 0 &&
                    !await _patientRepository.Any(p => p.Id == entity.PatientId))
                    referenceError = "Acolhido não encontrado para a instituição do usuário logado.";

                // O catalogo de condicoes clinicas e unico para todas as instituicoes e so o
                // admin da plataforma inclui entradas nele: aqui so se reaproveita uma
                // condicao ja cadastrada (pelo id ou pelo nome).
                if (referenceError == null &&
                    entity.PatientClinicalConditionDTO.Id == 0 &&
                    entity.PatientClinicalConditionDTO.ClinicalConditionId == 0 &&
                    !_currentUserService.IsPlatformAdmin)
                {
                    var nomeInformado = (entity.ClinicalConditionDTO?.Name ?? string.Empty).Trim().ToLower();
                    if (!await _clinicalConditionRepository.Any(c => c.Name.ToLower() == nomeInformado))
                        referenceError = "Condição clínica não cadastrada. Peça ao administrador da plataforma para cadastrá-la.";
                }

                if (referenceError != null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = referenceError;
                    return ret;
                }

                // 1. Resolve o medicamento.
                if (entity.MedicineDTO.Id != 0)
                {
                    medicineId = entity.MedicineDTO.Id;
                }
                else
                {
                    // Evita duplicados: reaproveita um medicamento ja existente com o mesmo
                    // nome (ignora maiusc./minusc. e espacos). O filtro de instituicao ja e
                    // aplicado pelo contexto (HasQueryFilter em Medicine).
                    var nomeMedicamento = (entity.MedicineDTO.Name ?? string.Empty).Trim();
                    var nomeMedicamentoLower = nomeMedicamento.ToLower();

                    var medicamentoExistente = (await _medicineRepository
                        .FindBy(m => m.Name.ToLower() == nomeMedicamentoLower)).FirstOrDefault();

                    if (medicamentoExistente != null)
                    {
                        medicineId = medicamentoExistente.Id;
                    }
                    else
                    {
                        var medicine = new Medicine
                        {
                            Name = nomeMedicamento,
                            Dosage = entity.MedicineDTO.Dosage,
                            Description = entity.MedicineDTO.Description,
                            AdministrationRoute = entity.MedicineDTO.AdministrationRoute,
                            // Carimba a instituicao do usuario logado (mesmo criterio do MedicineApp).
                            // Sem isso, o medicamento fica com InstitutionId nulo e o filtro de tenant
                            // o exclui das telas (listagem e proximos medicamentos) para usuarios de instituicao.
                            InstitutionId = _currentUserService.InstitutionId
                        };

                        await _medicineRepository.Create(medicine);

                        medicineId = medicine.Id;
                    }
                }

                // 2. Se o vínculo acolhido + condição clínica não existir, cria
                if (entity.PatientClinicalConditionDTO.Id == 0)
                {
                    long clinicalConditionId;

                    // 2.1 Resolve a condição clínica.
                    if (entity.PatientClinicalConditionDTO.ClinicalConditionId != 0)
                    {
                        clinicalConditionId = entity.PatientClinicalConditionDTO.ClinicalConditionId;
                    }
                    else
                    {
                        // Evita duplicados: reaproveita condição clínica já existente com o
                        // mesmo nome (ignora maiúsc./minúsc. e espaços).
                        var nomeCondicao = (entity.ClinicalConditionDTO.Name ?? string.Empty).Trim();
                        var nomeCondicaoLower = nomeCondicao.ToLower();

                        var condicaoExistente = (await _clinicalConditionRepository
                            .FindBy(c => c.Name.ToLower() == nomeCondicaoLower)).FirstOrDefault();

                        if (condicaoExistente != null)
                        {
                            clinicalConditionId = condicaoExistente.Id;
                        }
                        else
                        {
                            var clinicalCondition = new ClinicalCondition
                            {
                                Name = nomeCondicao,
                                Description = entity.ClinicalConditionDTO.Description,
                                Type = entity.ClinicalConditionDTO.Type
                            };

                            await _clinicalConditionRepository.Create(clinicalCondition);

                            clinicalConditionId = clinicalCondition.Id;
                        }
                    }

                    // 2.2 Resolve o vínculo da condição com o acolhido, reaproveitando um
                    // vínculo já existente (mesmo acolhido + mesma condição) para não
                    // duplicar a cadeia condição → medicamento programado.
                    var vinculoExistente = (await _patientClinicalConditionRepository
                        .FindBy(l => l.PatientId == entity.PatientId && l.ClinicalConditionId == clinicalConditionId))
                        .FirstOrDefault();

                    if (vinculoExistente != null)
                    {
                        patientClinicalConditionId = vinculoExistente.Id;
                    }
                    else
                    {
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
                }
                else
                {
                    patientClinicalConditionId = entity.PatientClinicalConditionDTO.Id;
                }

                // Impede programar o mesmo medicamento no mesmo horario para o mesmo
                // acolhido/condicao (evita duplicados em "Proximos Medicamentos").
                var jaProgramado = await _repo.FindBy(m =>
                    m.PatientClinicalConditionId == patientClinicalConditionId &&
                    m.MedicineId == medicineId &&
                    m.AdministrationTime == entity.AdministrationTime);

                if (jaProgramado.Any())
                {
                    ret.Success = false;
                    ret.IsBusinessExcpetion = true;
                    ret.Message = "Já existe esse medicamento cadastrado nesse horário.";
                    ret.ErrorMessage = "Já existe esse medicamento cadastrado nesse horário.";
                    return ret;
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
                ret.ErrorMessage = ApiErrors.From(ex);
            }  
            return ret;
        }

        /// <summary>
        /// Atualiza um medicamento programado existente ("Medicamento programado nao encontrado." se nao existir),
        /// validando medicamento, condicao do acolhido e funcionario da instituicao (ids 0 nao sao validados).
        /// Diferente do Create, nao verifica duplicado no mesmo horario.
        /// </summary>
        public async Task<ApiResponse> Update(UpdateMedicinePatientClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {

                var existingEntity = (await _repo.FindBy(m => m.Id == entity.Id)).FirstOrDefault();
                if (existingEntity == null)
                    return new ApiResponse { Success = false, ErrorMessage = "Medicamento programado não encontrado." };

                var referenceError = await ValidateReferences(
                    entity.MedicineId,
                    entity.PatientClinicalConditionId,
                    entity.ResponsibleEmployeeId);
                if (referenceError != null)
                    return new ApiResponse { Success = false, ErrorMessage = referenceError };

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
                return new ApiResponse { Success = false, ErrorMessage = ApiErrors.From(ex) };
            }
            return ret;
        }

        /// <summary>
        /// Exclui o medicamento programado com o Id informado e o historico de administracoes dele, numa transacao.
        /// Retorna "nao encontrado" se o Id nao existir (ou for de outra instituicao).
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try
            {
                var e = (await _repo.FindBy(m => m.Id == id)).FirstOrDefault();
                if (e == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Medicamento programado não encontrado.";
                    return ret;
                }

                // As administracoes referenciam o medicamento programado (FK Restrict): apaga
                // as administracoes e depois a prescricao, na mesma transacao.
                await _patientRepository.ExecuteInTransaction(async () =>
                {
                    var administrations = await _medicationAdministrationRepository
                        .FindBy(a => a.MedicinePatientClinicalConditionId == id);
                    if (administrations.Any())
                        await _medicationAdministrationRepository.DeleteRange(administrations);

                    await _repo.Delete(e);
                });
                ret.Success = true;
            }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }

        // Limpeza global: remove TODAS as administracoes e TODOS os medicamentos
        // programados de TODAS as instituicoes. Operacao destrutiva e irreversivel,
        // exclusiva do administrador da plataforma.
        public async Task<ApiResponse> DeleteAllScheduledMedicinesGlobal()
        {
            var ret = new ApiResponse();
            try
            {
                // Defesa em profundidade: alem da policy SaasAdmin no controller,
                // reforca aqui que so o admin da plataforma executa a limpeza global.
                if (!_currentUserService.IsPlatformAdmin)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Apenas o administrador da plataforma pode executar esta operação.";
                    return ret;
                }

                var administracoesRemovidas = 0;
                var programadosRemovidos = 0;

                // As duas exclusoes rodam na mesma transacao: se a segunda falhar, a
                // primeira e desfeita (nao fica historico apagado com prescricao orfa).
                // Ordem importa: as administracoes referenciam os medicamentos
                // programados (FK Restrict), entao removemos as filhas primeiro.
                await _patientRepository.ExecuteInTransaction(async () =>
                {
                    administracoesRemovidas = await _medicationAdministrationRepository.DeleteAllGlobal();
                    programadosRemovidos = await _repo.DeleteAllGlobal();

                    // ExecuteDelete nao passa pela auditoria automatica do SaveChanges:
                    // registra explicitamente quem fez a limpeza e quanto foi apagado.
                    if (_currentUserService.UserId.HasValue)
                    {
                        await _repo.LogGlobalCleanup(
                            _currentUserService.UserId.Value,
                            _currentUserService.IpAddress,
                            programadosRemovidos,
                            administracoesRemovidas);
                    }
                });

                ret.Success = true;
                ret.Message = $"Exclusão concluída: {programadosRemovidos} medicamento(s) programado(s) e {administracoesRemovidas} administração(ões) removida(s).";
                ret.Data = new { programadosRemovidos, administracoesRemovidas };
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve os medicamentos programados de um vinculo acolhido + condicao clinica.
        /// </summary>
        public async Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId)
        {
            return await _repo.GetByPatientClinicalConditionId(patientClinicalConditionId);
        }

        //implemntar job para ficar atualizando a lista de lembretes de medicamentos, e enviar notificação para o paciente
     
       /// <summary>
       /// Repassa o filtro ao repositorio e devolve a lista paginada de medicamentos programados.
       /// </summary>
       public async Task<PagedMedicinePatientClinicalConditionDTO> GetMedicinePatientClinicalConditionByFilter(MedicinePatientClinicalConditionFilterDTO filter)
        {
            return await _repo.GetMedicinePatientClinicalConditionByFilter(filter);
        }
    }
}
