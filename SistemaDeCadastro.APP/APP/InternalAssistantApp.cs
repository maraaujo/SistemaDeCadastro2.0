using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System.Text;

namespace SistemaDeCadastro.APP.APP;

public class InternalAssistantApp : IInternalAssistantApp
{
    private readonly IInternalAssistantRepository _repo;

    public InternalAssistantApp(IInternalAssistantRepository repo)
    {
        _repo = repo;
    }

    public async Task<ApiResponse> Ask(AskInternalAssistantDTO dto)
    {
        var ret = new ApiResponse();

        try
        {
            var referenceDate = dto.ReferenceDate ?? DateTime.Now;
            var question = NormalizeText(dto.Question);

            if (!string.IsNullOrWhiteSpace(question))
            {
                if (IsPatientsQuestion(question))
                    return await AnswerPatientsQuestion();

                if (IsMedicinesQuestion(question))
                    return await AnswerMedicinesQuestion();

                if (IsAllAppointmentsQuestion(question))
                    return await AnswerAllAppointmentsQuestion();

                if (IsDelayedMedicinesQuestion(question))
                    return await AnswerDelayedMedicinesQuestion();

                if (IsPendingMedicinesQuestion(question))
                    return await AnswerPendingMedicinesQuestion();

                if (IsTodayAdministrationsQuestion(question))
                    return await AnswerTodayAdministrationsQuestion();

                if (IsTodayAppointmentsQuestion(question))
                    return await AnswerTodayAppointmentsQuestion();

                if (IsNextAppointmentsQuestion(question))
                    return await AnswerNextAppointmentsQuestion();

                if (IsDailySummaryQuestion(question))
                    return await AnswerDailySummaryQuestion();

                if (IsPatientsByMedicineQuestion(question))
                    return await AnswerPatientsByMedicineQuestion(question);

                if (IsPatientsByClinicalConditionQuestion(question))
                    return await AnswerPatientsByClinicalConditionQuestion(question);

                if (IsSinglePatientSummaryQuestion(question))
                    return await AnswerSinglePatientSummaryQuestion(question);

                ret.Success = true;
                ret.Data = new InternalAssistantAnswerDTO
                {
                    Answer = "Ainda não consegui entender essa pergunta. Você pode perguntar sobre pacientes, medicamentos, administrações ou agendamentos."
                };

                return ret;
            }

            if (dto.PatientId.HasValue && dto.PatientId.Value > 0)
            {
                return await AnswerPatientMedicationAdministrationsByDate(
                    dto.PatientId.Value,
                    referenceDate
                );
            }

            ret.Success = false;
            ret.ErrorMessage = "Informe uma pergunta ou um acolhido para consulta.";
            return ret;
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            return ret;
        }
    }
}