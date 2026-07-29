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

            var administrations = await _repo.GetMedicationAdministrationContext(
                dto.PatientId,
                referenceDate
            );

            if (administrations == null || !administrations.Any())
            {
                ret.Success = true;
                ret.Data = new InternalAssistantAnswerDTO
                {
                    Answer = "Não encontrei registros de administração de medicamentos para este acolhido na data informada."
                };

                return ret;
            }

            var patientName = administrations.First().PatientName;

            var administered = administrations
                .Where(x => x.Status == "Administered")
                .ToList();

            var notAdministered = administrations
                .Where(x => x.Status != "Administered")
                .ToList();

            var answer = new StringBuilder();

            answer.AppendLine($"Resumo do acolhido {patientName} em {referenceDate:dd/MM/yyyy}:");
            answer.AppendLine();

            if (notAdministered.Any())
            {
                answer.AppendLine("Nem todos os medicamentos foram administrados.");

                foreach (var item in notAdministered)
                {
                    answer.AppendLine(
                        $"- {item.MedicineName} {item.PrescribedDosage}, previsto para {item.ScheduledDateTime:HH:mm}, status: {item.Status}."
                    );
                }
            }
            else
            {
                answer.AppendLine("Sim, todos os medicamentos registrados para essa data foram administrados.");
            }

            answer.AppendLine();

            answer.AppendLine("Registros encontrados:");

            foreach (var item in administrations)
            {
                var administeredText = item.AdministeredDateTime.HasValue
                    ? item.AdministeredDateTime.Value.ToString("HH:mm")
                    : "não registrado";

                answer.AppendLine(
                    $"- {item.MedicineName} {item.PrescribedDosage}, previsto para {item.ScheduledDateTime:HH:mm}, administrado: {administeredText}, responsável: {item.EmployeeName}. Observação: {item.Observations}"
                );
            }

            ret.Success = true;
            ret.Data = new InternalAssistantAnswerDTO
            {
                Answer = answer.ToString()
            };
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
        }

        return ret;
    }
}