using System;
using System.Collections.Generic;
using System.Linq;
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
    // Testes de CRUD (criar, consultar, alterar, excluir) dos modulos do SaaS:
    // Instituicao, Plano, Assinatura, Pagamento de assinatura, Pagamento e Conta de acesso.
    // Cada teste usa a App real + repositorio real sobre um banco InMemory novo,
    // com um contexto para a acao e outro contexto novo para conferir o que foi gravado.
    public class CrudSaasModulesTests
    {
        private const long Inst = 1;
        private const long OtherInst = 2;

        private class FakeUser : SistemaDeCadastro.APP.Interface.ICurrentUserServiceContext
        {
            public FakeUser(long? institutionId, long? userId = null, string? role = null)
            {
                InstitutionId = institutionId;
                UserId = userId;
                Role = role;
            }
            public long? InstitutionId { get; }
            public long? UserId { get; }
            public string? Role { get; }
        }

        // Admin da plataforma (policy SaasAdmin): Administrador sem instituicao.
        private static readonly FakeUser PlatformAdmin = new(null, 1, "Administrador");

        // Usuario de instituicao (rotas sem policy SaasAdmin, ex.: Payment).
        private static readonly FakeUser InstitutionUser = new(Inst, 10, "Colaborador");

        // Usuario sem UserId: usado so para semear dados (nao gera auditoria e nao filtra instituicao).
        private static readonly FakeUser Seeder = new(null, null, "Administrador");

        private static SistemaDeCadastroContext CreateContext(string dbName, FakeUser user)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new SistemaDeCadastroContext(options, user);
        }

        private static string NewDb() => "crud_saas_" + Guid.NewGuid();

        // Semeia instituicoes 1 e 2, o admin da plataforma (id 1) e o usuario da instituicao (id 10).
        private static async Task SeedBase(string db)
        {
            using var ctx = CreateContext(db, Seeder);
            ctx.Institutions.AddRange(
                new Institution { Id = Inst, Name = "Inst 1", Cnpj = "11.111.111/0001-11", Email = "inst1@a.com", Phone = "11987654321", Active = true, CreatedAt = DateTime.Now },
                new Institution { Id = OtherInst, Name = "Inst 2", Cnpj = "22.222.222/0001-22", Email = "inst2@a.com", Phone = "11987654322", Active = true, CreatedAt = DateTime.Now });
            ctx.LoginAccounts.AddRange(
                new LoginAccount { Id = 1, UserId = 1, Name = "Root", Email = "root@saas.com", PasswordHash = "x", UserType = "Administrador" },
                new LoginAccount { Id = 10, UserId = 10, Name = "Colab", Email = "colab@inst1.com", PasswordHash = "x", UserType = "Colaborador", InstitutionId = Inst });
            await ctx.SaveChangesAsync();
        }

        private static async Task<long> SeedPlan(string db, string name = "Basico")
        {
            using var ctx = CreateContext(db, Seeder);
            var plan = new Plan { Name = name, Description = "d", MonthlyPrice = 10m, MaxPatients = 5, MaxUsers = 2, Active = true };
            ctx.Plans.Add(plan);
            await ctx.SaveChangesAsync();
            return plan.Id;
        }

        private static async Task<long> SeedSubscription(string db, long institutionId, long planId)
        {
            using var ctx = CreateContext(db, Seeder);
            var sub = new Subscription
            {
                InstitutionId = institutionId, PlanId = planId, StartDate = DateTime.Now.AddDays(-10), Status = "Active"
            };
            ctx.Subscriptions.Add(sub);
            await ctx.SaveChangesAsync();
            return sub.Id;
        }

        // Semeia acolhido + agendamento da instituicao (o pagamento e filtrado pela instituicao do agendamento).
        private static async Task<long> SeedAppointment(string db, long institutionId, string suffix)
        {
            using var ctx = CreateContext(db, Seeder);
            var patient = new Patient
            {
                Name = "P" + suffix, Gender = "F", Cpf = "cpf" + suffix, Observations = "",
                BirthDate = DateTime.Today, CreatedAt = DateTime.Now, InstitutionId = institutionId
            };
            ctx.Patients.Add(patient);
            await ctx.SaveChangesAsync();

            var appointment = new Appointment
            {
                PatientId = patient.Id, UserId = 10, AppointmentType = "Consulta", DateTime = DateTime.Now,
                Responsible = "R", Status = "Agendado", Observations = "", InstitutionId = institutionId
            };
            ctx.Appointments.Add(appointment);
            await ctx.SaveChangesAsync();
            return appointment.Id;
        }

        private static async Task<long> SeedPayment(string db, long appointmentId, string externalId)
        {
            using var ctx = CreateContext(db, Seeder);
            var payment = new Payment
            {
                AppointmentId = appointmentId, UserId = 10, TotalAmount = 50m, PaymentMethod = "Pix",
                PaymentDate = DateTime.Today, Status = "Pendente", ExternalTransactionId = externalId
            };
            ctx.Payments.Add(payment);
            await ctx.SaveChangesAsync();
            return payment.Id;
        }

        // =====================================================================
        // Instituicao (todas as rotas de escrita e o GetAll usam a policy SaasAdmin)
        // =====================================================================

        private static InstitutionApp InstitutionAppFor(SistemaDeCadastroContext ctx) => new(new InstitutionRepository(ctx));

        [Fact]
        public async Task Institution_Create_PersistsRow()
        {
            var db = NewDb();
            await SeedBase(db);
            long id;

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var ret = await InstitutionAppFor(ctx).Create(new CreateInstitutionDTO
                {
                    Name = "Casa Nova", Cnpj = "33.333.333/0001-33", Email = "casa@nova.com", Phone = "(11) 98765-4321", Active = true
                });
                Assert.True(ret.Success, ret.ErrorMessage);
                id = Assert.IsType<Institution>(ret.Data).Id;
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.Institutions.SingleAsync(i => i.Id == id);
            Assert.Equal("Casa Nova", saved.Name);
            Assert.Equal("33.333.333/0001-33", saved.Cnpj);
            Assert.Equal("casa@nova.com", saved.Email);
            Assert.Equal("11987654321", saved.Phone);
            Assert.True(saved.Active);
        }

        [Fact]
        public async Task Institution_Select_GetAllAndGetById()
        {
            var db = NewDb();
            await SeedBase(db);

            using var ctx = CreateContext(db, PlatformAdmin);
            var app = InstitutionAppFor(ctx);

            var all = await app.GetAll();
            Assert.True(all.Success, all.ErrorMessage);
            var list = Assert.IsAssignableFrom<IEnumerable<Institution>>(all.Data).ToList();
            Assert.Contains(list, i => i.Id == Inst && i.Name == "Inst 1");
            Assert.Contains(list, i => i.Id == OtherInst);

            var one = await app.GetById(Inst);
            Assert.True(one.Success, one.ErrorMessage);
            Assert.Equal("Inst 1", Assert.IsType<Institution>(one.Data).Name);

            var missing = await app.GetById(999);
            Assert.True(missing.Success, missing.ErrorMessage);
            Assert.Null(missing.Data);
        }

        [Fact]
        public async Task Institution_Update_PersistsChangedFields()
        {
            var db = NewDb();
            await SeedBase(db);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var ret = await InstitutionAppFor(ctx).Update(new UpdateInstitutionDTO
                {
                    Id = Inst, Name = "Inst 1 Renomeada", Cnpj = "44.444.444/0001-44", Email = "novo@inst1.com",
                    Phone = "(21) 3333-4444", Active = false
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.Institutions.SingleAsync(i => i.Id == Inst);
            Assert.Equal("Inst 1 Renomeada", saved.Name);
            Assert.Equal("44.444.444/0001-44", saved.Cnpj);
            Assert.Equal("novo@inst1.com", saved.Email);
            Assert.Equal("2133334444", saved.Phone);
            Assert.False(saved.Active);
        }

        [Fact]
        public async Task Institution_Update_Missing_ReturnsNotFound()
        {
            var db = NewDb();
            await SeedBase(db);

            using var ctx = CreateContext(db, PlatformAdmin);
            var ret = await InstitutionAppFor(ctx).Update(new UpdateInstitutionDTO { Id = 999, Name = "X", Active = true });
            Assert.False(ret.Success);
            Assert.Equal("Instituição não encontrada.", ret.ErrorMessage);
        }

        [Fact]
        public async Task Institution_Delete_RemovesRow_AndMissingReturnsNotFound()
        {
            var db = NewDb();
            await SeedBase(db);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = InstitutionAppFor(ctx);

                var ret = await app.Delete(OtherInst);
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Delete(999);
                Assert.False(missing.Success);
                Assert.Equal("Instituição não encontrada.", missing.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            Assert.False(await check.Institutions.AnyAsync(i => i.Id == OtherInst));
            Assert.True(await check.Institutions.AnyAsync(i => i.Id == Inst));
        }

        // =====================================================================
        // Plano (leitura: qualquer autenticado; escrita: policy SaasAdmin)
        // =====================================================================

        private static PlanApp PlanAppFor(SistemaDeCadastroContext ctx) => new(new PlanRepository(ctx));

        [Fact]
        public async Task Plan_Create_PersistsRow()
        {
            var db = NewDb();
            await SeedBase(db);
            long id;

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var ret = await PlanAppFor(ctx).Create(new CreatePlanDTO
                {
                    Name = "Premium", Description = "Plano completo", MonthlyPrice = 199.90m, MaxPatients = 100, MaxUsers = 20, Active = true
                });
                Assert.True(ret.Success, ret.ErrorMessage);
                id = Assert.IsType<Plan>(ret.Data).Id;
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.Plans.SingleAsync(p => p.Id == id);
            Assert.Equal("Premium", saved.Name);
            Assert.Equal("Plano completo", saved.Description);
            Assert.Equal(199.90m, saved.MonthlyPrice);
            Assert.Equal(100, saved.MaxPatients);
            Assert.Equal(20, saved.MaxUsers);
            Assert.True(saved.Active);
        }

        [Fact]
        public async Task Plan_Select_GetAllAndGetById()
        {
            var db = NewDb();
            await SeedBase(db);
            var planId = await SeedPlan(db);

            // Leitura de planos e liberada para usuario de instituicao.
            using var ctx = CreateContext(db, InstitutionUser);
            var app = PlanAppFor(ctx);

            var all = await app.GetAll();
            Assert.True(all.Success, all.ErrorMessage);
            Assert.Contains(Assert.IsAssignableFrom<IEnumerable<Plan>>(all.Data), p => p.Id == planId && p.Name == "Basico");

            var one = await app.GetById(planId);
            Assert.True(one.Success, one.ErrorMessage);
            Assert.Equal("Basico", Assert.IsType<Plan>(one.Data).Name);

            var missing = await app.GetById(999);
            Assert.True(missing.Success, missing.ErrorMessage);
            Assert.Null(missing.Data);
        }

        [Fact]
        public async Task Plan_Update_PersistsChangedFields()
        {
            var db = NewDb();
            await SeedBase(db);
            var planId = await SeedPlan(db);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = PlanAppFor(ctx);
                var ret = await app.Update(new UpdatePlanDTO
                {
                    Id = planId, Name = "Basico Plus", Description = "nova", MonthlyPrice = 25.5m, MaxPatients = 15, MaxUsers = 4, Active = false
                });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Update(new UpdatePlanDTO { Id = 999, Name = "X" });
                Assert.False(missing.Success);
                Assert.Equal("Plano não encontrado.", missing.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.Plans.SingleAsync(p => p.Id == planId);
            Assert.Equal("Basico Plus", saved.Name);
            Assert.Equal("nova", saved.Description);
            Assert.Equal(25.5m, saved.MonthlyPrice);
            Assert.Equal(15, saved.MaxPatients);
            Assert.Equal(4, saved.MaxUsers);
            Assert.False(saved.Active);
        }

        [Fact]
        public async Task Plan_Delete_RemovesRow_AndMissingReturnsNotFound()
        {
            var db = NewDb();
            await SeedBase(db);
            var planId = await SeedPlan(db);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = PlanAppFor(ctx);

                var ret = await app.Delete(planId);
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Delete(999);
                Assert.False(missing.Success);
                Assert.Equal("Plano não encontrado.", missing.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            Assert.False(await check.Plans.AnyAsync(p => p.Id == planId));
        }

        // =====================================================================
        // Assinatura (CRUD com policy SaasAdmin; GetActiveByInstitution para a propria instituicao)
        // =====================================================================

        private static SubscriptionApp SubscriptionAppFor(SistemaDeCadastroContext ctx) => new(new SubscriptionRepository(ctx));

        [Fact]
        public async Task Subscription_Create_PersistsRow()
        {
            var db = NewDb();
            await SeedBase(db);
            var planId = await SeedPlan(db);
            long id;

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var ret = await SubscriptionAppFor(ctx).Create(new CreateSubscriptionDTO
                {
                    InstitutionId = Inst, PlanId = planId, Status = "Active", ExternalSubscriptionId = "ext-sub-1"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
                id = Assert.IsType<Subscription>(ret.Data).Id;
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.Subscriptions.SingleAsync(s => s.Id == id);
            Assert.Equal(Inst, saved.InstitutionId);
            Assert.Equal(planId, saved.PlanId);
            Assert.Equal("ext-sub-1", saved.ExternalSubscriptionId);
            Assert.Null(saved.EndDate);
            Assert.False(string.IsNullOrWhiteSpace(saved.Status));
        }

        [Fact]
        public async Task Subscription_Select_GetAllAndGetById()
        {
            var db = NewDb();
            await SeedBase(db);
            var planId = await SeedPlan(db);
            var subId = await SeedSubscription(db, Inst, planId);

            using var ctx = CreateContext(db, PlatformAdmin);
            var app = SubscriptionAppFor(ctx);

            var all = await app.GetAll();
            Assert.True(all.Success, all.ErrorMessage);
            Assert.Contains(Assert.IsAssignableFrom<IEnumerable<Subscription>>(all.Data), s => s.Id == subId && s.InstitutionId == Inst);

            var one = await app.GetById(subId);
            Assert.True(one.Success, one.ErrorMessage);
            Assert.Equal(planId, Assert.IsType<Subscription>(one.Data).PlanId);

            var missing = await app.GetById(999);
            Assert.True(missing.Success, missing.ErrorMessage);
            Assert.Null(missing.Data);
        }

        // Assinatura criada pela propria App deve aparecer como a assinatura ativa da instituicao.
        [Fact]
        public async Task Subscription_Select_GetActiveByInstitution_ReturnsCreatedSubscription()
        {
            var db = NewDb();
            await SeedBase(db);
            var planId = await SeedPlan(db);
            long id;

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var created = await SubscriptionAppFor(ctx).Create(new CreateSubscriptionDTO
                {
                    InstitutionId = Inst, PlanId = planId, Status = "Active"
                });
                Assert.True(created.Success, created.ErrorMessage);
                id = ((Subscription)created.Data).Id;
            }

            using var read = CreateContext(db, InstitutionUser);
            var ret = await SubscriptionAppFor(read).GetActiveByInstitutionId(Inst);
            Assert.True(ret.Success, ret.ErrorMessage);
            var active = Assert.IsType<Subscription>(ret.Data);
            Assert.Equal(id, active.Id);
        }

        [Fact]
        public async Task Subscription_Update_PersistsChangedFields()
        {
            var db = NewDb();
            await SeedBase(db);
            var planId = await SeedPlan(db);
            var otherPlanId = await SeedPlan(db, "Premium");
            var subId = await SeedSubscription(db, Inst, planId);
            var endDate = DateTime.Today.AddMonths(6);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = SubscriptionAppFor(ctx);
                var ret = await app.Update(new UpdateSubscriptionDTO
                {
                    Id = subId, InstitutionId = Inst, PlanId = otherPlanId, StartDate = DateTime.Today.AddDays(-10),
                    EndDate = endDate, Status = "Cancelled", ExternalSubscriptionId = "ext-2"
                });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Update(new UpdateSubscriptionDTO { Id = 999, InstitutionId = Inst, PlanId = planId, Status = "X" });
                Assert.False(missing.Success);
                Assert.False(string.IsNullOrWhiteSpace(missing.ErrorMessage));
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.Subscriptions.SingleAsync(s => s.Id == subId);
            Assert.Equal(otherPlanId, saved.PlanId);
            Assert.Equal(endDate, saved.EndDate);
            Assert.Equal("Cancelled", saved.Status);
            Assert.Equal("ext-2", saved.ExternalSubscriptionId);
        }

        // Alterar a assinatura (ex.: trocar o plano) nao pode zerar a data de inicio para "agora":
        // deve gravar a data de inicio enviada no DTO.
        [Fact]
        public async Task Subscription_Update_KeepsStartDateFromDto()
        {
            var db = NewDb();
            await SeedBase(db);
            var planId = await SeedPlan(db);
            var subId = await SeedSubscription(db, Inst, planId);
            var startDate = new DateTime(2025, 1, 15);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var ret = await SubscriptionAppFor(ctx).Update(new UpdateSubscriptionDTO
                {
                    Id = subId, InstitutionId = Inst, PlanId = planId, StartDate = startDate, Status = "Active"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.Subscriptions.SingleAsync(s => s.Id == subId);
            Assert.Equal(startDate, saved.StartDate);
        }

        [Fact]
        public async Task Subscription_Delete_RemovesRow_AndMissingReturnsNotFound()
        {
            var db = NewDb();
            await SeedBase(db);
            var planId = await SeedPlan(db);
            var subId = await SeedSubscription(db, Inst, planId);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = SubscriptionAppFor(ctx);

                var ret = await app.Delete(subId);
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Delete(999);
                Assert.False(missing.Success);
                Assert.False(string.IsNullOrWhiteSpace(missing.ErrorMessage));
            }

            using var check = CreateContext(db, PlatformAdmin);
            Assert.False(await check.Subscriptions.AnyAsync(s => s.Id == subId));
        }

        // =====================================================================
        // Pagamento de assinatura (controller inteiro com policy SaasAdmin)
        // =====================================================================

        private static SubscriptionPaymentApp SubscriptionPaymentAppFor(SistemaDeCadastroContext ctx) =>
            new(new SubscriptionPaymentRepository(ctx));

        private static async Task<long> SeedSubscriptionPayment(string db, long subscriptionId)
        {
            using var ctx = CreateContext(db, Seeder);
            var sp = new SubscriptionPayment
            {
                SubscriptionId = subscriptionId, Amount = 10m, PaymentMethod = "Pix", Status = "Pendente",
                PaymentDate = DateTime.Today, ExternalPaymentId = "ext-sp-seed", Observations = "obs"
            };
            ctx.SubscriptionPayments.Add(sp);
            await ctx.SaveChangesAsync();
            return sp.Id;
        }

        [Fact]
        public async Task SubscriptionPayment_Create_PersistsRow()
        {
            var db = NewDb();
            await SeedBase(db);
            var subId = await SeedSubscription(db, Inst, await SeedPlan(db));
            var paidAt = DateTime.Today.AddDays(-1);
            long id;

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var ret = await SubscriptionPaymentAppFor(ctx).Create(new CreateSubscriptionPaymentDTO
                {
                    SubscriptionId = subId, Amount = 99.9m, PaymentMethod = "Cartao", Status = "Pago",
                    PaymentDate = paidAt, ExternalPaymentId = "ext-sp-1", Observations = "primeira mensalidade"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
                id = Assert.IsType<SubscriptionPayment>(ret.Data).Id;
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.SubscriptionPayments.SingleAsync(p => p.Id == id);
            Assert.Equal(subId, saved.SubscriptionId);
            Assert.Equal(99.9m, saved.Amount);
            Assert.Equal("Cartao", saved.PaymentMethod);
            Assert.Equal("Pago", saved.Status);
            Assert.Equal(paidAt, saved.PaymentDate);
            Assert.Equal("ext-sp-1", saved.ExternalPaymentId);
            Assert.Equal("primeira mensalidade", saved.Observations);
        }

        [Fact]
        public async Task SubscriptionPayment_Select_GetAllAndGetById()
        {
            var db = NewDb();
            await SeedBase(db);
            var subId = await SeedSubscription(db, Inst, await SeedPlan(db));
            var spId = await SeedSubscriptionPayment(db, subId);

            using var ctx = CreateContext(db, PlatformAdmin);
            var app = SubscriptionPaymentAppFor(ctx);

            var all = await app.GetAll();
            Assert.True(all.Success, all.ErrorMessage);
            Assert.Contains(Assert.IsAssignableFrom<IEnumerable<SubscriptionPayment>>(all.Data), p => p.Id == spId);

            var one = await app.GetById(spId);
            Assert.True(one.Success, one.ErrorMessage);
            Assert.Equal(subId, Assert.IsType<SubscriptionPayment>(one.Data).SubscriptionId);

            var missing = await app.GetById(999);
            Assert.True(missing.Success, missing.ErrorMessage);
            Assert.Null(missing.Data);
        }

        [Fact]
        public async Task SubscriptionPayment_Update_PersistsChangedFields()
        {
            var db = NewDb();
            await SeedBase(db);
            var subId = await SeedSubscription(db, Inst, await SeedPlan(db));
            var spId = await SeedSubscriptionPayment(db, subId);
            var paidAt = DateTime.Today;

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = SubscriptionPaymentAppFor(ctx);
                var ret = await app.Update(new UpdateSubscriptionPaymentDTO
                {
                    Id = spId, SubscriptionId = subId, Amount = 12.5m, PaymentMethod = "Boleto", Status = "Pago",
                    PaymentDate = paidAt, ExternalPaymentId = "ext-sp-2", Observations = "quitado"
                });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Update(new UpdateSubscriptionPaymentDTO { Id = 999, SubscriptionId = subId });
                Assert.False(missing.Success);
                Assert.Equal("Pagamento não encontrado.", missing.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.SubscriptionPayments.SingleAsync(p => p.Id == spId);
            Assert.Equal(12.5m, saved.Amount);
            Assert.Equal("Boleto", saved.PaymentMethod);
            Assert.Equal("Pago", saved.Status);
            Assert.Equal(paidAt, saved.PaymentDate);
            Assert.Equal("ext-sp-2", saved.ExternalPaymentId);
            Assert.Equal("quitado", saved.Observations);
        }

        [Fact]
        public async Task SubscriptionPayment_Delete_RemovesRow_AndMissingReturnsNotFound()
        {
            var db = NewDb();
            await SeedBase(db);
            var subId = await SeedSubscription(db, Inst, await SeedPlan(db));
            var spId = await SeedSubscriptionPayment(db, subId);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = SubscriptionPaymentAppFor(ctx);

                var ret = await app.Delete(spId);
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Delete(999);
                Assert.False(missing.Success);
                Assert.Equal("Pagamento não encontrado.", missing.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            Assert.False(await check.SubscriptionPayments.AnyAsync(p => p.Id == spId));
        }

        // =====================================================================
        // Pagamento (qualquer usuario autenticado; filtrado pela instituicao do agendamento)
        // =====================================================================

        private static PaymentApp PaymentAppFor(SistemaDeCadastroContext ctx) =>
            new(new PaymentRepository(ctx), new AppointmentRepository(ctx));

        [Fact]
        public async Task Payment_Create_PersistsRow()
        {
            var db = NewDb();
            await SeedBase(db);
            var appointmentId = await SeedAppointment(db, Inst, "a");

            using (var ctx = CreateContext(db, InstitutionUser))
            {
                var ret = await PaymentAppFor(ctx).Create(new CreatePaymentDTO
                {
                    AppointmentId = appointmentId, UserId = 10, TotalAmount = 150m, PaymentMethod = "Pix",
                    PaymentDate = DateTime.Today, Status = "Pago", ExternalTransactionId = "tx-create-1"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.Payments.SingleOrDefaultAsync(p => p.ExternalTransactionId == "tx-create-1");
            Assert.NotNull(saved);
            Assert.Equal(appointmentId, saved!.AppointmentId);
            Assert.Equal(150m, saved.TotalAmount);
            Assert.Equal("Pago", saved.Status);
        }

        [Fact]
        public async Task Payment_Select_GetAllAndGetById()
        {
            var db = NewDb();
            await SeedBase(db);
            var appointmentId = await SeedAppointment(db, Inst, "a");
            var otherAppointmentId = await SeedAppointment(db, OtherInst, "b");
            var paymentId = await SeedPayment(db, appointmentId, "tx-1");
            var otherPaymentId = await SeedPayment(db, otherAppointmentId, "tx-2");

            using var ctx = CreateContext(db, InstitutionUser);
            var app = PaymentAppFor(ctx);

            var all = await app.GetAll();
            Assert.Contains(all, p => p.Id == paymentId);
            Assert.DoesNotContain(all, p => p.Id == otherPaymentId);

            var one = await app.GetById(paymentId);
            Assert.NotNull(one);
            Assert.Equal("tx-1", one!.ExternalTransactionId);

            Assert.Null(await app.GetById(999));
            Assert.Null(await app.GetById(otherPaymentId));
        }

        [Fact]
        public async Task Payment_Update_PersistsChangedFields()
        {
            var db = NewDb();
            await SeedBase(db);
            var appointmentId = await SeedAppointment(db, Inst, "a");
            var secondAppointmentId = await SeedAppointment(db, Inst, "c");
            var paymentId = await SeedPayment(db, appointmentId, "tx-1");
            var paidAt = DateTime.Today.AddDays(-2);

            using (var ctx = CreateContext(db, InstitutionUser))
            {
                var app = PaymentAppFor(ctx);
                var ret = await app.Update(new UpdatePaymentDTO
                {
                    Id = paymentId, AppointmentId = secondAppointmentId, UserId = 10, TotalAmount = 80m,
                    PaymentMethod = "Dinheiro", PaymentDate = paidAt, Status = "Pago", ExternalTransactionId = "tx-1b"
                });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Update(new UpdatePaymentDTO { Id = 999, AppointmentId = appointmentId, UserId = 10 });
                Assert.False(missing.Success);
                Assert.Equal("Pagamento não encontrado.", missing.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.Payments.SingleAsync(p => p.Id == paymentId);
            Assert.Equal(secondAppointmentId, saved.AppointmentId);
            Assert.Equal(80m, saved.TotalAmount);
            Assert.Equal("Dinheiro", saved.PaymentMethod);
            Assert.Equal(paidAt, saved.PaymentDate);
            Assert.Equal("Pago", saved.Status);
            Assert.Equal("tx-1b", saved.ExternalTransactionId);
        }

        [Fact]
        public async Task Payment_Delete_RemovesRow()
        {
            var db = NewDb();
            await SeedBase(db);
            var appointmentId = await SeedAppointment(db, Inst, "a");
            var paymentId = await SeedPayment(db, appointmentId, "tx-1");

            using (var ctx = CreateContext(db, InstitutionUser))
            {
                var ret = await PaymentAppFor(ctx).Delete(paymentId);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            Assert.False(await check.Payments.AnyAsync(p => p.Id == paymentId));
        }

        [Fact]
        public async Task Payment_Delete_Missing_ReturnsNotFound()
        {
            var db = NewDb();
            await SeedBase(db);

            using var ctx = CreateContext(db, InstitutionUser);
            var ret = await PaymentAppFor(ctx).Delete(999);
            Assert.False(ret.Success);
            Assert.False(string.IsNullOrWhiteSpace(ret.ErrorMessage));
        }

        // =====================================================================
        // Conta de acesso (Create ja coberto em LoginAccountCreateAndDeleteTests)
        // =====================================================================

        private static LoginAccountApp LoginAccountAppFor(SistemaDeCadastroContext ctx, FakeUser user) =>
            new(new LoginAccountRepository(ctx), user);

        private static async Task SeedExtraAccounts(string db)
        {
            using var ctx = CreateContext(db, Seeder);
            ctx.LoginAccounts.AddRange(
                new LoginAccount { Id = 20, UserId = 20, Name = "Maria", Email = "maria@inst1.com", PasswordHash = "x", UserType = "Colaborador", InstitutionId = Inst, Active = true },
                new LoginAccount { Id = 30, UserId = 30, Name = "Joao", Email = "joao@inst2.com", PasswordHash = "x", UserType = "Colaborador", InstitutionId = OtherInst, Active = true });
            await ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task LoginAccount_Select_GetAllAndGetById()
        {
            var db = NewDb();
            await SeedBase(db);
            await SeedExtraAccounts(db);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = LoginAccountAppFor(ctx, PlatformAdmin);

                var all = await app.GetAll();
                Assert.Contains(all, a => a.Id == 20 && a.Name == "Maria");
                Assert.Contains(all, a => a.Id == 30);

                var one = await app.GetById(20);
                Assert.NotNull(one);
                Assert.Equal("maria@inst1.com", one.Email);
                Assert.Equal(Inst, one.InstitutionId);

                Assert.Null(await app.GetById(999));
            }

            // Admin de instituicao so ve contas da propria instituicao.
            var institutionAdmin = new FakeUser(Inst, 10, "Administrador");
            using (var ctx = CreateContext(db, institutionAdmin))
            {
                var app = LoginAccountAppFor(ctx, institutionAdmin);

                var all = await app.GetAll();
                Assert.Contains(all, a => a.Id == 20);
                Assert.DoesNotContain(all, a => a.Id == 30);

                Assert.NotNull(await app.GetById(20));
                Assert.Null(await app.GetById(30));
            }
        }

        [Fact]
        public async Task LoginAccount_Update_PersistsChangedFields()
        {
            var db = NewDb();
            await SeedBase(db);
            await SeedExtraAccounts(db);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = LoginAccountAppFor(ctx, PlatformAdmin);
                var ret = await app.Update(new UpdateLoginAccountDTO
                {
                    Id = 20, Name = " Maria Souza ", Email = "maria.souza@inst1.com", UserType = "Administrador", Active = false
                });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Update(new UpdateLoginAccountDTO { Id = 999, Name = "X" });
                Assert.False(missing.Success);
                Assert.Equal("Conta não encontrada.", missing.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            var saved = await check.LoginAccounts.SingleAsync(l => l.Id == 20);
            Assert.Equal("Maria Souza", saved.Name);
            Assert.Equal("maria.souza@inst1.com", saved.Email);
            Assert.Equal("Administrador", saved.UserType);
            Assert.False(saved.Active);
            Assert.Equal(20, saved.UserId);
            Assert.Equal(Inst, saved.InstitutionId);
        }

        [Fact]
        public async Task LoginAccount_Delete_RemovesRow_AndMissingReturnsNotFound()
        {
            var db = NewDb();
            await SeedBase(db);
            await SeedExtraAccounts(db);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = LoginAccountAppFor(ctx, PlatformAdmin);

                var ret = await app.Delete(20);
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Delete(999);
                Assert.False(missing.Success);
                Assert.Equal("Conta não encontrada.", missing.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            Assert.False(await check.LoginAccounts.AnyAsync(l => l.Id == 20));
            Assert.True(await check.LoginAccounts.AnyAsync(l => l.Id == 30));
        }
    }
}
