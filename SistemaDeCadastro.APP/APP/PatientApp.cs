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
        private readonly IEmployeeRepository _employeeRepository;
        /// <summary>
        /// Recebe por injecao de dependencia os repositorios do acolhido e dos seus dependentes e o usuario logado.
        /// </summary>
        public PatientApp(
            IPatientRepository patientRepository,
            IResponsibleRepository responsibleRepository,
            IPatientEmployeeRepository patientEmployeeRepository,
            IPatientClinicalConditionRepository patientClinicalConditionRepository,
            IAppointmentRepository appointmentRepository,
            IPaymentRepository paymentRepository,
            IMedicinePatientClinicalConditionRepository medicinePatientClinicalConditionRepository,
            IMedicineRepository _medicineRepository,
            ICurrentUserServiceContext currentUserService,
            IEmployeeRepository employeeRepository
            )
        {
            this._employeeRepository = employeeRepository;
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
    

        // Medicamento e funcionario responsavel dos medicamentos programados vem do
        // cliente: precisam ser da instituicao do usuario (os repositorios ja aplicam o
        // filtro de instituicao). Retorna a mensagem de erro ou null se estiver ok.
        private async Task<string?> ValidateMedicineReferences(IEnumerable<(long MedicineId, long? ResponsibleEmployeeId)>? items)
        {
            if (items == null)
                return null;

            foreach (var (medicineId, employeeId) in items)
            {
                if (medicineId != 0 && !await _medicineRepository.Any(m => m.Id == medicineId))
                    return "Medicamento não encontrado para a instituição do usuário logado.";

                if (employeeId is > 0 && !await _employeeRepository.Any(e => e.Id == employeeId.Value))
                    return "Funcionário não encontrado para a instituição do usuário logado.";
            }

            return null;
        }

        /// <summary>
        /// Devolve todos os acolhidos visiveis para a instituicao do usuario.
        /// </summary>
        public async Task<List<Patient>> GetAllPatients()
            => await _patientRepository.GetAllReadOnly();

        /// <summary>
        /// Devolve uma lista com o acolhido do Id informado (vazia se nao existir).
        /// </summary>
        public async Task<List<Patient>> GetPatientById(long id)
            => await _patientRepository.GetPatientById(id);
        /// <summary>
        /// Repassa o filtro ao repositorio e devolve a lista paginada de acolhidos.
        /// </summary>
        public async Task<PagedPatientDTO> FilterPatient(PatientFilterDTO filter)
            => await _patientRepository.FilterPatient(filter);

        /// <summary>
        /// Devolve a ficha completa do acolhido (dados, responsaveis, condicoes, medicamentos etc.) ou null se nao existir.
        /// </summary>
        public async Task<DetailsPatientDTO?> DetailsPatient(long id) =>
            await _patientRepository.DetailsPatient(id);

        /// <summary>
        /// Cadastra um acolhido novo (Id deve ser 0) na instituicao do usuario, junto com responsaveis, condicoes clinicas
        /// e medicamentos programados. Valida CPF (e recusa CPF ja usado na instituicao), telefones dos responsaveis e se
        /// medicamentos/funcionarios sao da instituicao. Medicamento sem condicao correspondente vai para a primeira condicao
        /// criada; sem nenhuma condicao, retorna erro (o acolhido ja gravado nao e desfeito, pois nao ha transacao).
        /// </summary>
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

                var referenceError = await ValidateMedicineReferences(
                    patient.ScheduledMedicines?.Select(m => (m.MedicineId, m.ResponsibleEmployeeId)));
                if (referenceError != null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = referenceError;
                    return ret;
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
                                ResponsibleEmployeeId = medicineDto.ResponsibleEmployeeId is > 0 ? medicineDto.ResponsibleEmployeeId : null,
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
                ret.ErrorMessage = ApiErrors.From(err);
                ret.Success = false;
            }

            return ret;
        }


        /// <summary>
        /// Atualiza o acolhido e seus dependentes ("Paciente nao encontrado." se nao existir). So altera os campos
        /// preenchidos no DTO; valida telefones, CPF e se medicamentos/funcionarios sao da instituicao. Responsaveis,
        /// condicoes e medicamentos com Id existente sao atualizados, os demais sao criados (medicamento sem Id igual a
        /// um ja existente, mesmo medicamento e horario, e atualizado em vez de duplicado; o novo vai para a condicao
        /// informada). Responsaveis, condicoes e medicamentos que o acolhido tem e nao vieram na lista (removidos na tela) sao
        /// excluidos, com as administracoes dos medicamentos excluidos. Tudo e gravado em uma unica transacao.
        /// Nao checa CPF duplicado na edicao.
        /// </summary>
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

                var referenceError = await ValidateMedicineReferences(
                    patient.ScheduledMedicines?.Select(m => (m.MedicineId, m.ResponsibleEmployeeId)));
                if (referenceError != null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = referenceError;
                    return ret;
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

                // Condicoes e medicamentos que o acolhido ja tem (os criados nesta
                // requisicao sao acrescentados a estas listas conforme sao gravados).
                var patientConditions = updatePatient.PatientClinicalConditions.ToList();
                var patientMedicines = patientConditions
                    .SelectMany(x => x.Medicines ?? Enumerable.Empty<MedicinePatientClinicalCondition>())
                    .ToList();

                // O front sempre envia as listas completas: o que o acolhido tem no banco e
                // nao veio na requisicao foi removido na tela. Lista null = nao mexe no grupo.
                var responsiblesToRemove = patient.Responsibles == null
                    ? new List<Responsible>()
                    : updatePatient.Responsibles
                        .Where(r => patient.Responsibles.All(d => d.Id != r.Id))
                        .ToList();

                var conditionsToRemove = patient.ClinicalConditions == null
                    ? new List<PatientClinicalCondition>()
                    : patientConditions
                        .Where(c => patient.ClinicalConditions.All(d => d.Id != c.Id))
                        .ToList();

                // Medicamento sem id que bate com um existente (mesmo medicamento e horario)
                // e tratado como esse existente no laco abaixo, entao nao e removido.
                var medicinesToRemove = patient.ScheduledMedicines == null
                    ? new List<MedicinePatientClinicalCondition>()
                    : patientMedicines
                        .Where(m => patient.ScheduledMedicines.All(d =>
                            d.Id != m.Id &&
                            !(d.Id == 0 && d.MedicineId == m.MedicineId && d.AdministrationTime == m.AdministrationTime)))
                        .ToList();

                var removedConditionIds = conditionsToRemove.Select(c => c.Id).ToHashSet();
                var keptMedicinesOfRemovedConditions = patientMedicines
                    .Where(m => removedConditionIds.Contains(m.PatientClinicalConditionId) && !medicinesToRemove.Contains(m))
                    .ToList();

                if (patient.ScheduledMedicines != null && keptMedicinesOfRemovedConditions.Any())
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Remova os medicamentos programados da condição clínica antes de removê-la.";
                    return ret;
                }

                // Sem lista de medicamentos na requisicao, os da condicao removida saem junto com ela.
                medicinesToRemove.AddRange(keptMedicinesOfRemovedConditions);

                // Daqui em diante so contam as condicoes e medicamentos que continuam.
                patientConditions.RemoveAll(c => removedConditionIds.Contains(c.Id));
                patientMedicines.RemoveAll(m => medicinesToRemove.Contains(m));

                // Valida os medicamentos ANTES de gravar qualquer coisa.
                if (patient.ScheduledMedicines != null && patient.ScheduledMedicines.Any())
                {
                    if (patient.ScheduledMedicines.Any(m =>
                            m.Id != 0 && patientMedicines.All(x => x.Id != m.Id)))
                    {
                        ret.Success = false;
                        ret.ErrorMessage = "Medicamento programado não encontrado para este acolhido.";
                        return ret;
                    }

                    var hasCondition = patientConditions.Any()
                        || (patient.ClinicalConditions != null && patient.ClinicalConditions.Any());
                    if (!hasCondition)
                    {
                        ret.Success = false;
                        ret.ErrorMessage = "Não é possível cadastrar medicamento sem uma condição clínica associada.";
                        return ret;
                    }
                }

                // Dados do acolhido, responsaveis, condicoes e medicamentos sao gravados
                // numa unica transacao: se uma etapa falhar, nenhuma alteracao permanece.
                await _patientRepository.ExecuteInTransaction(async () =>
                {
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
                                patientConditions.Add(newCondition);
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
                            // Com id: e a prescricao que ja existe (validada acima). Sem id:
                            // se o acolhido ja tem o mesmo medicamento no mesmo horario,
                            // atualiza esse registro em vez de criar uma prescricao duplicada.
                            var existingMedicine = medicineDto.Id != 0
                                ? patientMedicines.FirstOrDefault(x => x.Id == medicineDto.Id)
                                : patientMedicines.FirstOrDefault(x =>
                                    x.MedicineId == medicineDto.MedicineId &&
                                    x.AdministrationTime == medicineDto.AdministrationTime);

                            if (existingMedicine != null)
                            {
                                if (medicineDto.MedicineId != 0)
                                    existingMedicine.MedicineId =
                                        medicineDto.MedicineId;

                                // O front sempre manda o estado completo: 0 ou vazio = sem funcionario responsavel.
                                    existingMedicine.ResponsibleEmployeeId =
                                        medicineDto.ResponsibleEmployeeId is > 0 ? medicineDto.ResponsibleEmployeeId : null;

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
                                // Condicao clinica da nova prescricao: a informada pelo
                                // cliente (id do vinculo ou da condicao); se nao bater com
                                // nenhuma do acolhido, usa a primeira (ha ao menos uma,
                                // garantido pela validacao no inicio do metodo).
                                var targetCondition =
                                    patientConditions.FirstOrDefault(c =>
                                        medicineDto.PatientClinicalConditionId != 0 &&
                                        c.Id == medicineDto.PatientClinicalConditionId)
                                    ?? patientConditions.FirstOrDefault(c =>
                                        c.ClinicalConditionId == medicineDto.ClinicalConditionId)
                                    ?? patientConditions.First();

                                var newMedicine =
                                    new MedicinePatientClinicalCondition
                                    {
                                        PatientClinicalConditionId = targetCondition.Id,

                                        MedicineId =
                                            medicineDto.MedicineId,

                                        ResponsibleEmployeeId =
                                            medicineDto.ResponsibleEmployeeId is > 0 ? medicineDto.ResponsibleEmployeeId : null,

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
                                patientMedicines.Add(newMedicine);
                            }
                        }
                    }
                    // Por ultimo, exclui o que foi removido na tela (e as administracoes dos medicamentos).
                    await _patientRepository.RemovePatientItems(responsiblesToRemove, conditionsToRemove, medicinesToRemove);
                });

                ret.Success = true;
            }
            catch (Exception err)
            {
                ret.ErrorMessage = ApiErrors.From(err);
                ret.Success = false;
            }

            return ret;
        }
        /// <summary>
        /// Exclui o acolhido e seus dependentes em uma unica transacao: administracoes, medicamentos programados,
        /// condicoes clinicas, agendamentos (e pagamentos ligados a eles), vinculos com funcionarios e
        /// responsaveis, e por fim o proprio acolhido. Retorna "Acolhido nao encontrado." se ele nao for da instituicao.
        /// </summary>
        public async Task<ApiResponse> DeletePatient(long id)
        {
            ApiResponse ret = new();

            try
            {
                // Busca o acolhido pelo filtro de instituicao ANTES de apagar qualquer
                // dependente: se ele for de outra instituicao (ou nao existir), nada e apagado.
                var deletePatient = (await _patientRepository.FindBy(p => p.Id == id)).FirstOrDefault();
                if (deletePatient == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Acolhido não encontrado.";
                    return ret;
                }

                // Apaga o acolhido e tudo o que depende dele (administracoes, medicamentos
                // programados, condicoes clinicas, pagamentos, agendamentos, vinculos e
                // responsaveis) numa unica transacao: se algo falhar, nada e apagado.
                await _patientRepository.DeletePatientWithDependencies(deletePatient);
                ret.Success = true;

            }
            catch (Exception err)
            {
                ret.ErrorMessage = ApiErrors.From(err);
                ret.Success = false;
            }

            return ret;
        }
        /// <summary>
        /// Chama a busca de acolhidos por texto no repositorio, mas descarta o resultado (o metodo nao devolve nada).
        /// </summary>
        public async Task GetPatientByAny(string patient) =>
            await this._patientRepository.GetPatientByAny(patient);

    }
}


