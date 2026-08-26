using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class InternalAssistantApp : IInternalAssistantApp
    {
        private readonly SistemaDeCadastroContext context;

        public InternalAssistantApp(SistemaDeCadastroContext _context)
        {
            context = _context;
        }
        public async Task<ApiResponse> Ask(AskInternalAssistantDTO dto)
        {
            var ret = new ApiResponse();

            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Question))
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Informe uma pergunta para o assistente.";
                    return ret;
                }
                //serve para padronizar textos em Unicode,
                //garantindo que caracteres equivalentes formados
                //por códigos diferentes tenham uma representação binária idêntica 
                var question = NormalizeText(dto.Question);
                string answer;
                //medicamento atrasado
                if (IsDelayedMedicinesQuestion(question))
                {
                    answer = await AnswerDelayedMedicinesQuestion(dto);
                }
                //medicamento paciente
                else if (dto.PatientId !=  0)
                {
                    answer = await AnswerPatientMedicineQuestion(dto);
                }
                //administração de remédios hoje
                else if (IsTodayAdministrationsQuestion(question))
                {
                    answer = await AnswerTodayAdministrationsQuestion(dto);
                }
                //resumo geral do dia 
                else if (IsDailySummaryQuestion(question))
                {
                    answer = await AnswerDailySummaryQuestion(dto);
                }
                else
                {
                    answer = "Ainda não consigo responder essa pergunta. Por enquanto, posso ajudar com medicamentos atrasados, medicamentos de um acolhido, administrações de hoje e resumo geral do dia.";
                }
                ret.Success = true;
                ret.Message = "Pergunta respondida com sucesso.";
                ret.Data = answer;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }

            return ret;
        }
        private bool IsDelayedMedicinesQuestion(string question)
        {
            return question.Contains("atrasado")
                || question.Contains("atrasados")
                || question.Contains("remedio atrasado")
                || question.Contains("medicamento atrasado");
        }
        //se encontrar alguma dessas palavras ele retorna true

        private bool IsPatientMedicineQuestion(string question)
        {
            return question.Contains("medicamentos do acolhido")
                || question.Contains("remedios do acolhido")
                || question.Contains("Quais medicamentos estão vinculados a este paciente")
                || question.Contains("medicamentos esse acolhido")
                || question.Contains("medicamentos esse paciente")
                || question.Contains("quais medicamentos ele usa")
                || question.Contains("quais remedios ele usa")
                || question.Contains("medicamentos usa");
        }

        private bool IsTodayAdministrationsQuestion(string question)
        {
            return question.Contains("administracoes feitas hoje")
                || question.Contains("administrados hoje")
                || question.Contains("administrado hoje")
                || question.Contains("o que foi administrado hoje");
        }

        private bool IsDailySummaryQuestion(string question)
        {
            return question.Contains("resumo do dia")
                || question.Contains("resumo geral")
                || question.Contains("como esta o dia")
                || question.Contains("situacao de hoje");
        }
        //metodo para pegar os remedios do dia 
        private async Task<string> AnswerDelayedMedicinesQuestion(AskInternalAssistantDTO dto)
        {
            var now = DateTime.Now;
            //foi passado o ReferenceDate
            var today = dto.ReferenceDate?.Date ?? now.Date;

            var scheduledMedicines = await (
                from mpcc in context.MedicinePatientClinicalConditions.AsNoTracking()
                join medicine in context.Medicines.AsNoTracking()
                on mpcc.MedicineId equals medicine.Id
                join pcc in context.PatientClinicalConditions.AsNoTracking()
                   on mpcc.PatientClinicalConditionId equals pcc.Id
                join patient in context.Patients.AsNoTracking()
                    on pcc.PatientId equals patient.Id

                join employee in context.Employees.AsNoTracking()
                    on mpcc.ResponsibleEmployeeId equals (long?)employee.Id into employeeGroup
                from employee in employeeGroup.DefaultIfEmpty()
                where mpcc.AdministrationTime != null
                      && (mpcc.StartDate == null || mpcc.StartDate <= today)
                      && (mpcc.EndDate == null || mpcc.EndDate >= today)
                select new
                {
                    PatientId = patient.Id,
                    PatientName = patient.Name,
                    MedicineId = medicine.Id,
                    MedicineName = medicine.Name,
                    Dosage = mpcc.PrescribedDosage,
                    Frequency = mpcc.Frequency,
                    AdministrationTime = mpcc.AdministrationTime,
                    ResponsibleEmployeeName = employee != null ? employee.Name : "Não informado"
                }).ToListAsync();
            //var atrasado pe
            var delayed = scheduledMedicines
                .Select(x => new
                {
                    x.PatientId,
                    x.PatientName,
                    x.MedicineId,
                    x.MedicineName,
                    x.Dosage,
                    x.Frequency,
                    x.AdministrationTime,
                    x.ResponsibleEmployeeName,
                    ScheduledDateTime = today.Add(x.AdministrationTime.Value)
                })
                .Where(x => x.ScheduledDateTime < now)
                .ToList();

            if (!delayed.Any())
                return "Não há medicamentos atrasados no momento.";


            var administrationsToday = await context.MedicationAdministrations
               .AsNoTracking()
               .Where(x => x.AdministeredDateTime != null
                           && x.AdministeredDateTime.Value.Date == today)
               .ToListAsync();

            delayed = delayed
                .Where(x => !administrationsToday.Any(a =>
                    a.PatientId == x.PatientId &&
                    a.ScheduledDateTime.Date == today &&
                    a.ScheduledDateTime.TimeOfDay == x.AdministrationTime.Value))
                .OrderBy(x => x.ScheduledDateTime)
                .ToList();
            if (!delayed.Any())
                return "Não há medicamentos atrasados no momento.";

            var sb = new StringBuilder();

            sb.AppendLine($"Foram encontrados {delayed.Count} medicamento(s) atrasado(s) hoje:");
            sb.AppendLine();

            var index = 1;

            foreach (var item in delayed)
            {
                var minutesLate = (int)(now - item.ScheduledDateTime).TotalMinutes;

                sb.AppendLine($"{index}. {item.PatientName} - {item.MedicineName} {item.Dosage}");
                sb.AppendLine($"Horário previsto: {item.AdministrationTime:hh\\:mm}");
                sb.AppendLine($"Atrasado há {minutesLate} minuto(s)");
                sb.AppendLine($"Responsável: {item.ResponsibleEmployeeName}");
                sb.AppendLine();

                index++;
            }

            return sb.ToString();
        }
        private async Task<string> AnswerPatientMedicineQuestion(AskInternalAssistantDTO dto)
        {
            if (!dto.PatientId.HasValue)
            {
                return "Selecione um acolhido para que eu possa consultar os medicamentos dele.";
            }
            var medicines = await (
                from mpcc in context.MedicinePatientClinicalConditions.AsNoTracking()

                join medicine in context.Medicines.AsNoTracking()
                    on mpcc.MedicineId equals medicine.Id

                join pcc in context.PatientClinicalConditions.AsNoTracking()
                    on mpcc.PatientClinicalConditionId equals pcc.Id

                join patient in context.Patients.AsNoTracking()
                    on pcc.PatientId equals patient.Id

                join condition in context.ClinicalConditions.AsNoTracking()
                    on pcc.ClinicalConditionId equals condition.Id

                join employee in context.Employees.AsNoTracking()
                    on mpcc.ResponsibleEmployeeId equals (long?)employee.Id into employeeGroup
                from employee in employeeGroup.DefaultIfEmpty()

                where patient.Id == dto.PatientId.Value

                select new
                {
                    PatientName = patient.Name,
                    MedicineName = medicine.Name,
                    Dosage = mpcc.PrescribedDosage,
                    Frequency = mpcc.Frequency,
                    AdministrationTime = mpcc.AdministrationTime,
                    ConditionName = condition.Name,
                    ResponsibleEmployeeName = employee != null ? employee.Name : "Não informado"
                }
            ).ToListAsync();
            if (!medicines.Any())
            {
                return "Não encontrei medicamentos vinculados a esse acolhido.";
            }
            var sb = new StringBuilder();

            sb.AppendLine($"Medicamentos vinculados ao acolhido {medicines.First().PatientName}:");
            sb.AppendLine();

            var index = 1;

            foreach (var item in medicines)
            {
                sb.AppendLine($"{index}. {item.MedicineName}");
                sb.AppendLine($"Dosagem: {item.Dosage}");
                sb.AppendLine($"Frequência: {item.Frequency}");
                sb.AppendLine($"Horário: {(item.AdministrationTime.HasValue ? item.AdministrationTime.Value.ToString(@"hh\:mm") : "Não informado")}");
                sb.AppendLine($"Condição clínica: {item.ConditionName}");
                sb.AppendLine($"Responsável: {item.ResponsibleEmployeeName}");
                sb.AppendLine();

                index++;
            }

            return sb.ToString();
        }
        private async Task<string> AnswerTodayAdministrationsQuestion(AskInternalAssistantDTO dto)
        {
            var today = dto.ReferenceDate?.Date ?? DateTime.Now.Date;

            var administrations = await (
                from administration in context.MedicationAdministrations.AsNoTracking()

                join patient in context.Patients.AsNoTracking()
                    on administration.PatientId equals patient.Id

                join mpcc in context.MedicinePatientClinicalConditions.AsNoTracking()
                    on administration.MedicinePatientClinicalConditionId equals mpcc.Id

                join medicine in context.Medicines.AsNoTracking()
                    on mpcc.MedicineId equals medicine.Id

                join employee in context.Employees.AsNoTracking()
                    on administration.EmployeeId equals (long?)employee.Id into employeeGroup
                from employee in employeeGroup.DefaultIfEmpty()

                where administration.AdministeredDateTime != null
                      && administration.AdministeredDateTime.Value.Date == today

                select new
                {
                    PatientName = patient.Name,
                    MedicineName = medicine.Name,
                    ScheduledDateTime = administration.ScheduledDateTime,
                    AdministeredDateTime = administration.AdministeredDateTime,
                    Status = administration.Status,
                    EmployeeName = employee != null ? employee.Name : "Não informado"
                }
            )
            .OrderBy(x => x.AdministeredDateTime)
            .ToListAsync();

            if (!administrations.Any())
            {
                return "Nenhuma administração de medicamento foi registrada hoje.";
            }

            var sb = new StringBuilder();

            sb.AppendLine($"Administrações registradas hoje: {administrations.Count}");
            sb.AppendLine();

            var index = 1;

            foreach (var item in administrations)
            {
                sb.AppendLine($"{index}. {item.PatientName} - {item.MedicineName}");
                sb.AppendLine($"Horário previsto: {item.ScheduledDateTime:HH:mm}");
                sb.AppendLine($"Horário administrado: {item.AdministeredDateTime:HH:mm}");
                sb.AppendLine($"Status: {item.Status}");
                sb.AppendLine($"Funcionário: {item.EmployeeName}");
                sb.AppendLine();

                index++;
            }

            return sb.ToString();
        }
        //resumo do dia
        private async Task<string> AnswerDailySummaryQuestion(AskInternalAssistantDTO dto)
        {
            var now = DateTime.Now;
            var today = dto.ReferenceDate?.Date ?? now.Date;

            var scheduledMedicines = await (
                from mpcc in context.MedicinePatientClinicalConditions.AsNoTracking()

                join medicine in context.Medicines.AsNoTracking()
                    on mpcc.MedicineId equals medicine.Id

                join pcc in context.PatientClinicalConditions.AsNoTracking()
                    on mpcc.PatientClinicalConditionId equals pcc.Id

                join patient in context.Patients.AsNoTracking()
                    on pcc.PatientId equals patient.Id

                where mpcc.AdministrationTime != null
                      && (mpcc.StartDate == null || mpcc.StartDate <= today)
                      && (mpcc.EndDate == null || mpcc.EndDate >= today)

                select new
                {
                    PatientName = patient.Name,
                    MedicineName = medicine.Name,
                    AdministrationTime = mpcc.AdministrationTime
                }
            ).ToListAsync();

            var administrationsToday = await context.MedicationAdministrations
                .AsNoTracking()
                .Where(x => x.AdministeredDateTime != null
                            && x.AdministeredDateTime.Value.Date == today)
                .ToListAsync();

            var appointmentsToday = await context.Appointments
                .AsNoTracking()
                .Where(x => x.DateTime.Date == today)
                .OrderBy(x => x.DateTime)
                .Take(5)
                .Select(x => new
                {
                    x.DateTime,
                    x.AppointmentType,
                    x.Responsible
                })
                .ToListAsync();

            var delayedCount = scheduledMedicines
                .Where(x => today.Add(x.AdministrationTime.Value) < now)
                .Count();

            var nextMedicines = scheduledMedicines
                .Where(x => today.Add(x.AdministrationTime.Value) >= now)
                .OrderBy(x => x.AdministrationTime)
                .Take(5)
                .ToList();

            var sb = new StringBuilder();

            sb.AppendLine("Resumo de hoje:");
            sb.AppendLine();
            sb.AppendLine($"- Medicamentos programados: {scheduledMedicines.Count}");
            sb.AppendLine($"- Medicamentos administrados: {administrationsToday.Count}");
            sb.AppendLine($"- Medicamentos atrasados: {delayedCount}");
            sb.AppendLine($"- Agendamentos de hoje: {appointmentsToday.Count}");
            sb.AppendLine();

            if (nextMedicines.Any())
            {
                sb.AppendLine("Próximos medicamentos:");

                var index = 1;

                foreach (var item in nextMedicines)
                {
                    sb.AppendLine($"{index}. {item.PatientName} - {item.MedicineName} às {item.AdministrationTime:hh\\:mm}");
                    index++;
                }

                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("Não há próximos medicamentos programados para hoje.");
                sb.AppendLine();
            }

            if (appointmentsToday.Any())
            {
                sb.AppendLine("Próximos agendamentos:");

                var index = 1;

                foreach (var item in appointmentsToday)
                {
                    sb.AppendLine($"{index}. {item.AppointmentType} às {item.DateTime:HH:mm} - Responsável: {item.Responsible}");
                    index++;
                }
            }
            else
            {
                sb.AppendLine("Não há agendamentos para hoje.");
            }

            return sb.ToString();
        }

        private string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToLowerInvariant();

            var normalized = text.Normalize(NormalizationForm.FormD);

            var chars = normalized
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
    }
