using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;

namespace SistemadeCadastro.Test
{
    // Limpezas globais do administrador da plataforma: apagar todos os acolhidos e
    // zerar os dados do sistema. Confere o que e apagado, o que e mantido e quem pode executar.
    public class MaintenanceTests
    {
        private class FakeUser : SistemaDeCadastro.APP.Interface.ICurrentUserServiceContext
        {
            public FakeUser(long? institutionId, long? userId, string? role)
            {
                InstitutionId = institutionId;
                UserId = userId;
                Role = role;
            }
            public long? InstitutionId { get; }
            public long? UserId { get; }
            public string? Role { get; }
        }

        private const long AdminUserId = 900;

        private static readonly FakeUser PlatformAdmin = new(null, AdminUserId, "Administrador");
        private static readonly FakeUser InstitutionAdmin = new(1, 901, "Administrador");

        // Contexto de preparacao/conferencia: enxerga tudo e nao grava auditoria (sem usuario).
        private static SistemaDeCadastroContext Unfiltered(string db) =>
            NewContext(db, new FakeUser(null, null, "Administrador"));

        private static SistemaDeCadastroContext NewContext(string db, FakeUser user) =>
            new(new DbContextOptionsBuilder<SistemaDeCadastroContext>().UseInMemoryDatabase(db).Options, user);

        private static string NewDb() => "maintenance_" + Guid.NewGuid();

        private static MaintenanceApp NewApp(SistemaDeCadastroContext ctx, FakeUser user) =>
            new(new MaintenanceRepository(ctx), user);

        // Duas instituicoes, cada uma com departamento, funcionario, medicamento e um acolhido
        // com responsavel, condicao clinica, medicamento programado, administracao, vinculo,
        // agendamento e pagamento. Mais uma conta de acesso e uma condicao clinica de apoio.
        private static async Task Seed(string db)
        {
            using var ctx = Unfiltered(db);

            ctx.LoginAccounts.Add(new LoginAccount
            {
                Id = AdminUserId, UserId = AdminUserId, Name = "Admin", Email = "admin@plataforma.com",
                PasswordHash = "x", UserType = "Administrador", Active = true
            });
            var clinicalCondition = new ClinicalCondition { Name = "Hipertensão", Type = "Crônica", Description = "" };
            ctx.ClinicalConditions.Add(clinicalCondition);
            await ctx.SaveChangesAsync();

            foreach (var institutionId in new long[] { 1, 2 })
            {
                var department = new Department { Name = "Enfermagem", Description = "", InstitutionId = institutionId };
                ctx.Departments.Add(department);
                var medicine = new Medicine
                {
                    Name = "Losartana", Dosage = "50mg", Description = "", AdministrationRoute = "Oral", InstitutionId = institutionId
                };
                ctx.Medicines.Add(medicine);
                var patient = new Patient
                {
                    Name = "Acolhido " + institutionId, Gender = "F", Cpf = "cpf" + institutionId, Observations = "",
                    BirthDate = DateTime.Today.AddYears(-80), CreatedAt = DateTime.Now, InstitutionId = institutionId
                };
                ctx.Patients.Add(patient);
                await ctx.SaveChangesAsync();

                var employee = new Employee
                {
                    Name = "Funcionário " + institutionId, Cpf = "func" + institutionId, Position = "Cuidador", Phone = "0",
                    Email = $"f{institutionId}@x.com", AdmissionDate = DateTime.Now, InstitutionId = institutionId,
                    DepartmentId = department.Id
                };
                ctx.Employees.Add(employee);
                ctx.Responsibles.Add(new Responsible
                {
                    PatientId = patient.Id, Name = "Filha", Phone = "11999999999", Relationship = "Filha", Address = "Rua"
                });
                var condition = new PatientClinicalCondition
                {
                    PatientId = patient.Id, ClinicalConditionId = clinicalCondition.Id, Observations = ""
                };
                ctx.PatientClinicalConditions.Add(condition);
                var appointment = new Appointment
                {
                    PatientId = patient.Id, UserId = AdminUserId, InstitutionId = institutionId, AppointmentType = "Consulta",
                    DateTime = DateTime.Now, Responsible = "R", Status = "Agendado", Observations = ""
                };
                ctx.Appointments.Add(appointment);
                await ctx.SaveChangesAsync();

                ctx.PatientEmployees.Add(new PatientEmployee
                {
                    PatientId = patient.Id, EmployeeId = employee.Id, ResponsibilityFunction = "Cuidador"
                });
                var scheduled = new MedicinePatientClinicalCondition
                {
                    PatientClinicalConditionId = condition.Id, MedicineId = medicine.Id, PrescribedDosage = "1 comp",
                    Frequency = "1x ao dia", AdministrationTime = new TimeSpan(8, 0, 0), Observations = "",
                    ResponsibleEmployeeId = employee.Id
                };
                ctx.MedicinePatientClinicalConditions.Add(scheduled);
                ctx.Payments.Add(new Payment
                {
                    AppointmentId = appointment.Id, UserId = AdminUserId, TotalAmount = 10, PaymentMethod = "Pix",
                    PaymentDate = DateTime.Now, Status = "Pago", ExternalTransactionId = "tx" + institutionId
                });
                await ctx.SaveChangesAsync();

                ctx.MedicationAdministrations.Add(new MedicationAdministration
                {
                    MedicinePatientClinicalConditionId = scheduled.Id, PatientId = patient.Id, EmployeeId = employee.Id,
                    ScheduledDateTime = DateTime.Today.AddHours(8), AdministeredDateTime = DateTime.Today.AddHours(8),
                    Status = "Administrado", Observations = "", CreatedAt = DateTime.Now
                });
                await ctx.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task Summary_CountsRecordsOfAllInstitutions()
        {
            var db = NewDb();
            await Seed(db);

            using var ctx = NewContext(db, PlatformAdmin);
            var ret = await NewApp(ctx, PlatformAdmin).GetSummary();

            Assert.True(ret.Success, ret.ErrorMessage);
            var counts = Assert.IsType<MaintenanceCountsDTO>(ret.Data);
            Assert.Equal(2, counts.Patients);
            Assert.Equal(2, counts.Responsibles);
            Assert.Equal(2, counts.ScheduledMedicines);
            Assert.Equal(2, counts.MedicationAdministrations);
            Assert.Equal(2, counts.Appointments);
            Assert.Equal(2, counts.Payments);
            Assert.Equal(2, counts.Employees);
            Assert.Equal(2, counts.Departments);
            Assert.Equal(2, counts.Medicines);
            Assert.Equal(22, counts.Total);
        }

        [Fact]
        public async Task DeleteAllPatients_RemovesPatientsAndDependents_KeepsStaffAndCatalogs()
        {
            var db = NewDb();
            await Seed(db);

            using (var ctx = NewContext(db, PlatformAdmin))
            {
                var ret = await NewApp(ctx, PlatformAdmin).DeleteAllPatients();

                Assert.True(ret.Success, ret.ErrorMessage);
                var removed = Assert.IsType<MaintenanceCountsDTO>(ret.Data);
                Assert.Equal(2, removed.Patients);
                Assert.Equal(0, removed.Employees);
                Assert.Equal(16, removed.Total);
            }

            using var check = Unfiltered(db);
            Assert.Empty(await check.Patients.ToListAsync());
            Assert.Empty(await check.Responsibles.ToListAsync());
            Assert.Empty(await check.PatientClinicalConditions.ToListAsync());
            Assert.Empty(await check.MedicinePatientClinicalConditions.ToListAsync());
            Assert.Empty(await check.MedicationAdministrations.ToListAsync());
            Assert.Empty(await check.Appointments.ToListAsync());
            Assert.Empty(await check.Payments.ToListAsync());
            Assert.Empty(await check.PatientEmployees.ToListAsync());

            // Funcionarios, departamentos, medicamentos e cadastros de apoio continuam.
            Assert.Equal(2, await check.Employees.CountAsync());
            Assert.Equal(2, await check.Departments.CountAsync());
            Assert.Equal(2, await check.Medicines.CountAsync());
            Assert.Equal(1, await check.ClinicalConditions.CountAsync());
            Assert.Equal(1, await check.LoginAccounts.CountAsync());
        }

        [Fact]
        public async Task ResetSystemData_RemovesOperationalData_KeepsAccountsAndCatalogs()
        {
            var db = NewDb();
            await Seed(db);

            using (var ctx = NewContext(db, PlatformAdmin))
            {
                var ret = await NewApp(ctx, PlatformAdmin).ResetSystemData();

                Assert.True(ret.Success, ret.ErrorMessage);
                Assert.Equal(22, Assert.IsType<MaintenanceCountsDTO>(ret.Data).Total);
            }

            using var check = Unfiltered(db);
            Assert.Empty(await check.Patients.ToListAsync());
            Assert.Empty(await check.MedicationAdministrations.ToListAsync());
            Assert.Empty(await check.MedicinePatientClinicalConditions.ToListAsync());
            Assert.Empty(await check.Appointments.ToListAsync());
            Assert.Empty(await check.Employees.ToListAsync());
            Assert.Empty(await check.Departments.ToListAsync());
            Assert.Empty(await check.Medicines.ToListAsync());

            // O que permite continuar usando o sistema e mantido.
            Assert.Equal(1, await check.LoginAccounts.CountAsync());
            Assert.Equal(1, await check.ClinicalConditions.CountAsync());
        }

        [Fact]
        public async Task Cleanup_IsRecordedInTheAuditTrail()
        {
            var db = NewDb();
            await Seed(db);

            using (var ctx = NewContext(db, PlatformAdmin))
                await NewApp(ctx, PlatformAdmin).ResetSystemData();

            using var check = Unfiltered(db);
            var log = await check.AccessLogs.SingleAsync(l => l.Action.StartsWith("Zerou os dados do sistema"));
            Assert.Equal(AdminUserId, log.UserId);
            Assert.Contains("2 acolhido(s)", log.Observation);
        }

        [Fact]
        public async Task InstitutionUser_CannotRunAnyMaintenanceOperation()
        {
            var db = NewDb();
            await Seed(db);

            using (var ctx = NewContext(db, InstitutionAdmin))
            {
                var app = NewApp(ctx, InstitutionAdmin);

                Assert.False((await app.GetSummary()).Success);
                Assert.False((await app.DeleteAllPatients()).Success);
                Assert.False((await app.ResetSystemData()).Success);
            }

            using var check = Unfiltered(db);
            Assert.Equal(2, await check.Patients.CountAsync());
            Assert.Equal(2, await check.Employees.CountAsync());
        }
    }
}
