using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;
using System.Text;

namespace SistemaDeCadastro.APP.APP;

public class InternalAssistantApp : IInternalAssistantApp
{
    private readonly IInternalAssistantRepository _repo;
    private readonly IMedicinePatientClinicalConditionRepository _medicinePatientClinicalConditionRepository;
    public InternalAssistantApp(IInternalAssistantRepository repo, IMedicinePatientClinicalConditionRepository medicinePatientClinicalConditionRepository)
    {
        _repo = repo;
        _medicinePatientClinicalConditionRepository = medicinePatientClinicalConditionRepository;
    }
    //public async Task<ApiResponse> Ask(AskInternalAssistantDTO dto)
    //{
    //    // Cria o objeto de resposta padrão da aplicação.
    //    // Esse objeto será retornado para o controller com sucesso, erro ou dados.
    //    var ret = new ApiResponse();

    //    try
    //    {
    //        // Define a data de referência da consulta.
    //        // Se o usuário enviou uma data no DTO, usa essa data.
    //        // Caso contrário, usa a data/hora atual.
    //        var referenceDate = dto.ReferenceDate ?? DateTime.Now;

    //        // Busca no repositório os registros de administração de medicamentos
    //        // relacionados ao paciente informado e à data de referência.
    //        //var administrations = await _repo.GetMedicationAdministrationContext(
    //        //    dto.PatientId,
    //        //    referenceDate
    //        //);

    //        // Verifica se não foram encontrados registros de administração
    //        // para o paciente na data informada.
    //        if (administrations == null || !administrations.Any())
    //        {
    //            // Mesmo sem registros, a requisição foi executada corretamente.
    //            // Por isso Success recebe true.
    //            ret.Success = true;

    //            // Retorna uma resposta amigável para o usuário.
    //            ret.Data = new InternalAssistantAnswerDTO
    //            {
    //                Answer = "Não encontrei registros de administração de medicamentos para este acolhido na data informada."
    //            };

    //            return ret;
    //        }

    //        // Obtém o nome do paciente a partir do primeiro registro encontrado.
    //        // Como todos os registros são do mesmo paciente, basta pegar o primeiro.
    //        var patientName = administrations.First().PatientName;

    //        // Filtra os medicamentos que foram administrados.
    //        // Aqui o sistema considera como administrado o status "Administered".
    //        var administered = administrations
    //            .Where(x => x.Status == "Administered")
    //            .ToList();

    //        // Filtra os medicamentos que não foram administrados.
    //        // Tudo que tiver status diferente de "Administered" entra nessa lista.
    //        var notAdministered = administrations
    //            .Where(x => x.Status != "Administered")
    //            .ToList();

    //        // Cria um StringBuilder para montar a resposta textual do assistente.
    //        // Ele é melhor do que concatenar várias strings manualmente.
    //        var answer = new StringBuilder();

    //        // Adiciona o título do resumo, mostrando o nome do acolhido
    //        // e a data de referência usada na consulta.
    //        answer.AppendLine($"Resumo do acolhido {patientName} em {referenceDate:dd/MM/yyyy}:");
    //        answer.AppendLine();

    //        // Se existir algum medicamento não administrado,
    //        // o assistente informa que nem todos foram administrados.
    //        if (notAdministered.Any())
    //        {
    //            answer.AppendLine("Nem todos os medicamentos foram administrados.");

    //            // Percorre os medicamentos que não foram administrados
    //            // e adiciona cada um deles na resposta.
    //            foreach (var item in notAdministered)
    //            {
    //                answer.AppendLine(
    //                    $"- {item.MedicineName} {item.PrescribedDosage}, previsto para {item.ScheduledDateTime:HH:mm}, status: {item.Status}."
    //                );
    //            }
    //        }
    //        else
    //        {
    //            // Se não houver medicamentos pendentes/não administrados,
    //            // significa que todos os registros encontrados estão como administrados.
    //            answer.AppendLine("Sim, todos os medicamentos registrados para essa data foram administrados.");
    //        }

    //        answer.AppendLine();

    //        // Adiciona uma seção com todos os registros encontrados,
    //        // tanto administrados quanto não administrados.
    //        answer.AppendLine("Registros encontrados:");

    //        // Percorre todos os registros de administração encontrados.
    //        foreach (var item in administrations)
    //        {
    //            // Verifica se existe horário de administração registrado.
    //            // Se existir, formata como HH:mm.
    //            // Se não existir, mostra "não registrado".
    //            var administeredText = item.AdministeredDateTime.HasValue
    //                ? item.AdministeredDateTime.Value.ToString("HH:mm")
    //                : "não registrado";

    //            // Adiciona na resposta os detalhes do medicamento:
    //            // nome, dosagem, horário previsto, horário administrado,
    //            // funcionário responsável e observações.
    //            answer.AppendLine(
    //                $"- {item.MedicineName} {item.PrescribedDosage}, previsto para {item.ScheduledDateTime:HH:mm}, administrado: {administeredText}, responsável: {item.EmployeeName}. Observação: {item.Observations}"
    //            );
    //        }

    //        // Define que a operação foi executada com sucesso.
    //        ret.Success = true;

    //        // Retorna a resposta montada dentro do DTO de resposta do assistente.
    //        ret.Data = new InternalAssistantAnswerDTO
    //        {
    //            Answer = answer.ToString()
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        // Caso ocorra algum erro, retorna Success false.
    //        ret.Success = false;

    //        // Retorna a mensagem da exceção interna, se existir.
    //        // Caso contrário, retorna a mensagem principal da exceção.
    //        ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
    //    }

    //    // Retorna o resultado final da operação.
    //    return ret;
    //}
    //esse metodo percorre a pergunta e 
    private bool IsPatientMedicationSummaryQuestion(string question)
    {
        return question.Contains("resumo")
            || question.Contains("administracoes do paciente")
            || question.Contains("historico de medicacao")
            || question.Contains("medicamentos administrados");
    }

    private bool IsDelayedMedicinesQuestion(string question)
    {
        return question.Contains("medicamentos atrasados")
            || question.Contains("remedios atrasados")
            || question.Contains("medicacoes atrasadas")
            || question.Contains("atrasados hoje");
    }

    private bool IsTodayAdministrationsQuestion(string question)
    {
        return question.Contains("administracoes realizadas hoje")
            || question.Contains("medicamentos administrados hoje")
            || question.Contains("administrados hoje");
    }

    private bool IsDailySummaryQuestion(string question)
    {
        return question.Contains("resumo geral do dia")
            || question.Contains("resumo do dia")
            || question.Contains("visao geral do dia");
    }
    private async Task<ApiResponse> AnswerDelayedMedicinesQuestion()
    {

        var ret = new ApiResponse();

        //aqui pega os horarios de aminsitração dos remedios 
        var reminders = await _medicinePatientClinicalConditionRepository.GetMedicineReminders();


        var delayed = reminders
            .Where(x => x.MinutesRemaining < 0)
            .OrderBy(x => x.MinutesRemaining)
            .ToList();

        if (!delayed.Any())
        {
            ret.Success = true;
            ret.Data = new InternalAssistantAnswerDTO
            {
                Answer = "Não há medicamentos atrasados no momento."
            };

            return ret;
        }
        var answer = new StringBuilder();

        answer.AppendLine($"Existem {delayed.Count} medicamento(s) atrasado(s):");
        answer.AppendLine();
        foreach (var item in delayed.Take(10))
        {
            answer.AppendLine(
                $"- {item.PatientName}: {item.MedicineName} {item.Dosage}, horário {item.AdministrationTime:hh\\:mm}, {item.AlertText}. Responsável: {item.ResponsibleEmployeeName}."
            );
        }
        ret.Success = true;
        ret.Data = new InternalAssistantAnswerDTO
        {
            Answer = answer.ToString()
        };

        return ret;
    }
    //private async Task<ApiResponse> AnswerPatientMedicationAdministrationsByDate(long patientId, DateTime referenceDate)
    //{
    //    var ret = new ApiResponse();
      
    //    var administracao = await _repo.GetMedicationAdministrationContext(patientId, referenceDate);

    //    if (administracao == null || !administracao.Any())
    //    {
    //        ret.Success = true;
    //        ret.Data = new InternalAssistantAnswerDTO
    //        {
    //            Answer = "Não encontrei registros de administração de medicamentos para este acolhido na data informada."
    //        };

    //        return ret;
    //    }

    //    //procurar o nome do paciente
    //    var pacienteNome = administracao.First().PatientName;

    //    var administrado = administracao
    //        .Where(x => x.Status == "Administrado")
    //        .ToList();

    //    var naoAdministrado = administracao
    //        .Where(x => x.Status != "Administrado")
    //        .ToList();

    //    //montar a resposta
    //    var answer = new StringBuilder();
    //    answer.AppendLine($"Resumo do acolhido {pacienteNome} em {referenceDate:dd/MM/yyyy}:");
    //    answer.AppendLine();
    //    if (naoAdministrado.Any())
    //    {
    //        answer.AppendLine("Nem todos os medicamentos foram administrados.");

    //        foreach (var item in naoAdministrado)
    //        {
    //            answer.AppendLine(
    //                $"- {item.MedicineName} {item.PrescribedDosage}, previsto para {item.ScheduledDateTime:HH:mm}, status: {item.Status}."
    //            );
    //        }
    //    }
    //    else
    //    {
    //        //o appendLine é usado para adicionar uma nova
    //        //linha ao final da string, então não é necessário usar o
    //        answer.AppendLine("Todos os medicamentos foram administrados.");
    //    }
    //    answer.AppendLine();
    //    answer.AppendLine("Registros encontrados:");

    //    foreach (var item in administracao)
    //    {
    //        var textoDeAdministracao = item.AdministeredDateTime.HasValue
    //            ? $"administrado em {item.AdministeredDateTime:HH:mm}"
    //            : "não administrado";

    //        answer.AppendLine(
    //            $"- {item.MedicineName} {item.PrescribedDosage}, " +
    //            $"previsto para {item.ScheduledDateTime:HH:mm}, " +
    //            $"status: {item.Status}, {textoDeAdministracao}."
    //        );
    //    }

    //    // Primeiro termina de percorrer todos os medicamentos.
    //    // Depois monta a resposta final.
    //    ret.Success = true;
    //    ret.Data = new InternalAssistantAnswerDTO
    //    {
    //        Answer = answer.ToString()
    //    };

    //    return ret;
    //}
    //private string NormalizeText(string text)
    //{
    //    if (string.IsNullOrWhiteSpace(text))
    //        return string.Empty;

    //    text = text.ToLower().Trim();

    //    var normalized = text.Normalize(System.Text.NormalizationForm.FormD);

    //    var chars = normalized.Where(c =>
    //        System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
    //        System.Globalization.UnicodeCategory.NonSpacingMark);

    //    return new string(chars.ToArray()).Normalize(System.Text.NormalizationForm.FormC);
    //}
}
