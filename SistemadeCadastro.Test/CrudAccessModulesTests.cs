using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;

namespace SistemadeCadastro.Test
{
    // CRUD dos modulos de acesso: AccessLog, AvailablePermission, UserPermission e
    // MedicinePatientIllnessHistoric (codigo morto: App/Repositorio existem, mas nao ha
    // controller, nao ha registro no Program.cs e a entidade nao esta no DbContext).
    // CareService e MedicinePatientIllness so tem interface (sem implementacao), entao nao ha o que testar.
    // Sempre: App real + repositorio real sobre o contexto InMemory, com contextos separados
    // para executar (act) e para conferir (assert).
    public class CrudAccessModulesTests
    {
        private const long Inst1 = 1;
        private const long Inst2 = 2;

        // UserId (LoginAccount.UserId) das contas semeadas.
        private const long AdminUserId = 1;
        private const long Inst1UserId = 10;
        private const long Inst2UserId = 20;

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

        // Policy "SaasAdmin": papel administrativo e sem instituicao no token.
        private static readonly FakeUser SaasAdmin = new(null, AdminUserId, "Administrador");
        // Usuarios de instituicao (rotas com [Authorize] simples, ex.: leitura de logs).
        private static readonly FakeUser Inst1User = new(Inst1, Inst1UserId, "Colaborador");
        private static readonly FakeUser Inst2User = new(Inst2, Inst2UserId, "Colaborador");
        // Usado apenas para semear dados, sem gerar auditoria.
        private static readonly FakeUser Seeder = new(null, null, "Administrador");

        private static SistemaDeCadastroContext CreateContext(string dbName, FakeUser user)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new SistemaDeCadastroContext(options, user);
        }

        private static string NewDb() => "crud_access_" + Guid.NewGuid();

        // Contas de acesso: admin da plataforma e um usuario em cada instituicao.
        private static async Task SeedAccounts(string db)
        {
            using var ctx = CreateContext(db, Seeder);
            ctx.LoginAccounts.AddRange(
                new LoginAccount { Id = 1, UserId = AdminUserId, Name = "Root", Email = "root@saas.com", PasswordHash = "x", UserType = "Administrador" },
                new LoginAccount { Id = 10, UserId = Inst1UserId, Name = "Ana", Email = "ana@inst1.com", PasswordHash = "x", UserType = "Colaborador", InstitutionId = Inst1 },
                new LoginAccount { Id = 20, UserId = Inst2UserId, Name = "Bruno", Email = "bruno@inst2.com", PasswordHash = "x", UserType = "Colaborador", InstitutionId = Inst2 });
            await ctx.SaveChangesAsync();
        }

        // ======================================================================
        // AccessLog (controller: leitura para qualquer autenticado; escrita so SaasAdmin)
        // ======================================================================

        private static AccessLogApp NewAccessLogApp(SistemaDeCadastroContext ctx) => new(new AccessLogRepository(ctx));

        private static CreateAccessLogDTO NewLog(long userId, string action) => new()
        {
            UserId = userId,
            Action = action,
            DateTime = new DateTime(2026, 11, 20, 10, 30, 0),
            IpAddress = "10.0.0.1",
            Observation = "obs inicial"
        };

        // Cria um log via App (como o SaasAdmin faria) e devolve o Id gravado.
        private static async Task<long> CreateLogViaApp(string db, long userId, string action)
        {
            using var ctx = CreateContext(db, SaasAdmin);
            var ret = await NewAccessLogApp(ctx).Create(NewLog(userId, action));
            Assert.True(ret.Success, ret.ErrorMessage);
            return (await ctx.AccessLogs.SingleAsync(a => a.Action == action)).Id;
        }

        [Fact]
        public async Task AccessLog_Create_ReturnsSuccess_AndPersists()
        {
            var db = NewDb();
            await SeedAccounts(db);

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewAccessLogApp(ctx).Create(NewLog(Inst1UserId, "Acao manual"));
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, SaasAdmin);
            var log = await check.AccessLogs.SingleAsync(a => a.Action == "Acao manual");
            Assert.Equal(Inst1UserId, log.UserId);
            Assert.Equal(new DateTime(2026, 11, 20, 10, 30, 0), log.DateTime);
            Assert.Equal("10.0.0.1", log.IpAddress);
            Assert.Equal("obs inicial", log.Observation);
        }

        [Fact]
        public async Task AccessLog_Select_GetAllGetByIdAndFilter_ReturnCreatedRow_MissingIdIsNull()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var id = await CreateLogViaApp(db, Inst1UserId, "Acao listada");

            using var ctx = CreateContext(db, SaasAdmin);
            var app = NewAccessLogApp(ctx);

            var all = await app.GetAll();
            Assert.Contains(all, a => a.Id == id && a.Action == "Acao listada");

            var byId = await app.GetById(id);
            Assert.NotNull(byId);
            Assert.Equal("Acao listada", byId!.Action);

            var paged = await app.GetAccessLogsByFilter(new AccessLogFilterDTO { Action = "listada" });
            var item = Assert.Single(paged.AccessLogs);
            Assert.Equal(id, item.Id);
            Assert.Equal("ana@inst1.com", item.UserEmail);

            var missing = await app.GetById(999_999);
            Assert.Null(missing);
        }

        [Fact]
        public async Task AccessLog_Update_ReturnsSuccess_AndPersistsChanges()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var id = await CreateLogViaApp(db, Inst1UserId, "Acao original");

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewAccessLogApp(ctx).Update(new UpdateAccessLogDTO
                {
                    Id = id,
                    UserId = Inst1UserId,
                    Action = "Acao editada",
                    DateTime = new DateTime(2026, 11, 21, 8, 0, 0),
                    IpAddress = "10.0.0.2",
                    Observation = "obs editada"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, SaasAdmin);
            var log = await check.AccessLogs.SingleAsync(a => a.Id == id);
            Assert.Equal("Acao editada", log.Action);
            Assert.Equal(new DateTime(2026, 11, 21, 8, 0, 0), log.DateTime);
            Assert.Equal("10.0.0.2", log.IpAddress);
            Assert.Equal("obs editada", log.Observation);
        }

        [Fact]
        public async Task AccessLog_Update_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            await SeedAccounts(db);

            using var ctx = CreateContext(db, SaasAdmin);
            var ret = await NewAccessLogApp(ctx).Update(new UpdateAccessLogDTO
            {
                Id = 999_999, UserId = Inst1UserId, Action = "x", IpAddress = "x", Observation = "x"
            });

            Assert.False(ret.Success);
            // Nao encontrado precisa vir com mensagem (como nos outros modulos), nao vazio.
            Assert.False(string.IsNullOrWhiteSpace(ret.ErrorMessage));
        }

        [Fact]
        public async Task AccessLog_Delete_ReturnsSuccess_AndRemovesRow()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var id = await CreateLogViaApp(db, Inst1UserId, "Acao a excluir");

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewAccessLogApp(ctx).Delete(id);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, SaasAdmin);
            Assert.False(await check.AccessLogs.AnyAsync(a => a.Id == id));
        }

        [Fact]
        public async Task AccessLog_Delete_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            await SeedAccounts(db);

            using var ctx = CreateContext(db, SaasAdmin);
            var ret = await NewAccessLogApp(ctx).Delete(999_999);

            Assert.False(ret.Success);
            Assert.False(string.IsNullOrWhiteSpace(ret.ErrorMessage));
        }

        [Fact]
        public async Task AccessLog_Isolation_Inst2UserCannotSeeOrChangeInst1Log()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var id = await CreateLogViaApp(db, Inst1UserId, "Acao da inst1");

            // Controle positivo: o usuario da propria instituicao enxerga o log.
            using (var ctx1 = CreateContext(db, Inst1User))
            {
                var app1 = NewAccessLogApp(ctx1);
                Assert.NotNull(await app1.GetById(id));
                Assert.Contains(await app1.GetAll(), a => a.Id == id);
            }

            using (var ctx2 = CreateContext(db, Inst2User))
            {
                var app2 = NewAccessLogApp(ctx2);
                Assert.Null(await app2.GetById(id));
                Assert.DoesNotContain(await app2.GetAll(), a => a.Id == id);
                var paged = await app2.GetAccessLogsByFilter(new AccessLogFilterDTO());
                Assert.DoesNotContain(paged.AccessLogs, a => a.Id == id);

                // Mesmo chamando a App diretamente (a rota exige SaasAdmin), nada muda.
                await app2.Update(new UpdateAccessLogDTO
                {
                    Id = id, UserId = Inst2UserId, Action = "Hackeado", IpAddress = "x", Observation = "x"
                });
                await app2.Delete(id);
            }

            using var check = CreateContext(db, SaasAdmin);
            var log = await check.AccessLogs.SingleAsync(a => a.Id == id);
            Assert.Equal("Acao da inst1", log.Action);
            Assert.Equal(Inst1UserId, log.UserId);
        }

        // ======================================================================
        // AvailablePermission (catalogo global; leitura qualquer autenticado, escrita SaasAdmin)
        // ======================================================================

        private static AvailablePermissionApp NewPermissionApp(SistemaDeCadastroContext ctx) => new(new AvailablePermissionRepository(ctx));

        private static async Task<long> CreatePermissionViaApp(string db, string name)
        {
            using var ctx = CreateContext(db, SaasAdmin);
            var ret = await NewPermissionApp(ctx).Create(new CreateAvailablePermissionDTO
            {
                Name = name, Description = "desc " + name, Module = "Acolhidos", Active = true
            });
            Assert.True(ret.Success, ret.ErrorMessage);
            return (await ctx.AvailablePermissions.SingleAsync(p => p.Name == name)).Id;
        }

        [Fact]
        public async Task AvailablePermission_Create_ReturnsSuccess_AndPersists()
        {
            var db = NewDb();
            await SeedAccounts(db);

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewPermissionApp(ctx).Create(new CreateAvailablePermissionDTO
                {
                    Name = "VerAcolhidos", Description = "Pode ver acolhidos", Module = "Acolhidos", Active = true
                });
                Assert.True(ret.Success, ret.ErrorMessage);
                var data = Assert.IsType<AvailablePermission>(ret.Data);
                Assert.NotEqual(0, data.Id);
            }

            using var check = CreateContext(db, SaasAdmin);
            var p = await check.AvailablePermissions.SingleAsync(x => x.Name == "VerAcolhidos");
            Assert.Equal("Pode ver acolhidos", p.Description);
            Assert.Equal("Acolhidos", p.Module);
            Assert.True(p.Active);
        }

        [Fact]
        public async Task AvailablePermission_Select_GetAllAndGetById_ReturnCreatedRow_MissingIdIsNull()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var id = await CreatePermissionViaApp(db, "EditarAcolhidos");

            // Leitura e liberada para qualquer autenticado: usa um usuario de instituicao.
            using var ctx = CreateContext(db, Inst1User);
            var app = NewPermissionApp(ctx);

            Assert.Contains(await app.GetAll(), p => p.Id == id && p.Name == "EditarAcolhidos");

            var byId = await app.GetById(id);
            Assert.NotNull(byId);
            Assert.Equal("EditarAcolhidos", byId!.Name);

            Assert.Null(await app.GetById(999_999));
        }

        [Fact]
        public async Task AvailablePermission_Update_ReturnsSuccess_AndPersistsChanges()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var id = await CreatePermissionViaApp(db, "PermOriginal");

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewPermissionApp(ctx).Update(new UpdateAvailablePermissionDTO
                {
                    Id = id, Name = "PermEditada", Description = "nova desc", Module = "Funcionarios", Active = false
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, SaasAdmin);
            var p = await check.AvailablePermissions.SingleAsync(x => x.Id == id);
            Assert.Equal("PermEditada", p.Name);
            Assert.Equal("nova desc", p.Description);
            Assert.Equal("Funcionarios", p.Module);
            Assert.False(p.Active);
        }

        [Fact]
        public async Task AvailablePermission_Update_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            await SeedAccounts(db);

            using var ctx = CreateContext(db, SaasAdmin);
            var ret = await NewPermissionApp(ctx).Update(new UpdateAvailablePermissionDTO
            {
                Id = 999_999, Name = "x", Description = "x", Module = "x"
            });

            Assert.False(ret.Success);
            Assert.False(string.IsNullOrWhiteSpace(ret.ErrorMessage));
        }

        [Fact]
        public async Task AvailablePermission_Delete_ReturnsSuccess_AndRemovesRow()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var id = await CreatePermissionViaApp(db, "PermExcluir");

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewPermissionApp(ctx).Delete(id);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, SaasAdmin);
            Assert.False(await check.AvailablePermissions.AnyAsync(p => p.Id == id));
        }

        [Fact]
        public async Task AvailablePermission_Delete_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            await SeedAccounts(db);

            using var ctx = CreateContext(db, SaasAdmin);
            var ret = await NewPermissionApp(ctx).Delete(999_999);

            Assert.False(ret.Success);
            Assert.False(string.IsNullOrWhiteSpace(ret.ErrorMessage));
        }

        // ======================================================================
        // UserPermission (controller inteiro com policy SaasAdmin; tabela sem filtro por instituicao)
        // ======================================================================

        private static UserPermissionApp NewUserPermissionApp(SistemaDeCadastroContext ctx) => new(new UserPermissionRepository(ctx));

        // Semeia duas permissoes do catalogo e devolve os Ids.
        private static async Task<(long p1, long p2)> SeedPermissions(string db)
        {
            using var ctx = CreateContext(db, Seeder);
            var p1 = new AvailablePermission { Name = "P1", Description = "d1", Module = "M", Active = true };
            var p2 = new AvailablePermission { Name = "P2", Description = "d2", Module = "M", Active = true };
            ctx.AvailablePermissions.AddRange(p1, p2);
            await ctx.SaveChangesAsync();
            return (p1.Id, p2.Id);
        }

        private static async Task<long> CreateUserPermissionViaApp(string db, long userId, long permissionId, DateTime grantedAt)
        {
            using var ctx = CreateContext(db, SaasAdmin);
            var ret = await NewUserPermissionApp(ctx).Create(new CreateUserPermissionDTO
            {
                UserId = userId, PermissionId = permissionId, GrantedAt = grantedAt
            });
            Assert.True(ret.Success, ret.ErrorMessage);
            return (await ctx.UserPermissions.SingleAsync(u => u.UserId == userId && u.PermissionId == permissionId)).Id;
        }

        [Fact]
        public async Task UserPermission_Create_ReturnsSuccess_AndPersists()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var (p1, _) = await SeedPermissions(db);
            var granted = new DateTime(2026, 11, 1, 9, 0, 0);

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                // O front envia Id = 0 (gerado pelo banco).
                var ret = await NewUserPermissionApp(ctx).Create(new CreateUserPermissionDTO
                {
                    Id = 0, UserId = Inst1UserId, PermissionId = p1, GrantedAt = granted
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, SaasAdmin);
            var up = await check.UserPermissions.SingleAsync();
            Assert.NotEqual(0, up.Id);
            Assert.Equal(Inst1UserId, up.UserId);
            Assert.Equal(p1, up.PermissionId);
            Assert.Equal(granted, up.GrantedAt);
        }

        [Fact]
        public async Task UserPermission_Select_GetAllAndGetById_ReturnCreatedRow_MissingIdIsNull()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var (p1, _) = await SeedPermissions(db);
            var id = await CreateUserPermissionViaApp(db, Inst1UserId, p1, new DateTime(2026, 11, 1));

            using var ctx = CreateContext(db, SaasAdmin);
            var app = NewUserPermissionApp(ctx);

            Assert.Contains(await app.GetAll(), u => u.Id == id && u.UserId == Inst1UserId);

            var byId = await app.GetById(id);
            Assert.NotNull(byId);
            Assert.Equal(p1, byId!.PermissionId);

            Assert.Null(await app.GetById(999_999));
        }

        [Fact]
        public async Task UserPermission_Update_ReturnsSuccess_AndPersistsChanges()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var (p1, p2) = await SeedPermissions(db);
            var id = await CreateUserPermissionViaApp(db, Inst1UserId, p1, new DateTime(2026, 11, 1));
            var newGranted = new DateTime(2026, 11, 15, 14, 0, 0);

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewUserPermissionApp(ctx).Update(new UpdateUserPermissionDTO
                {
                    Id = id, UserId = Inst2UserId, PermissionId = p2, GrantedAt = newGranted
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, SaasAdmin);
            var up = await check.UserPermissions.SingleAsync(u => u.Id == id);
            Assert.Equal(Inst2UserId, up.UserId);
            Assert.Equal(p2, up.PermissionId);
            Assert.Equal(newGranted, up.GrantedAt);
        }

        [Fact]
        public async Task UserPermission_Update_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var (p1, _) = await SeedPermissions(db);

            using var ctx = CreateContext(db, SaasAdmin);
            var ret = await NewUserPermissionApp(ctx).Update(new UpdateUserPermissionDTO
            {
                Id = 999_999, UserId = Inst1UserId, PermissionId = p1, GrantedAt = DateTime.Today
            });

            Assert.False(ret.Success);
            Assert.False(string.IsNullOrWhiteSpace(ret.ErrorMessage));
        }

        [Fact]
        public async Task UserPermission_Delete_ReturnsSuccess_AndRemovesRow()
        {
            var db = NewDb();
            await SeedAccounts(db);
            var (p1, _) = await SeedPermissions(db);
            var id = await CreateUserPermissionViaApp(db, Inst1UserId, p1, new DateTime(2026, 11, 1));

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewUserPermissionApp(ctx).Delete(id);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, SaasAdmin);
            Assert.False(await check.UserPermissions.AnyAsync(u => u.Id == id));
        }

        [Fact]
        public async Task UserPermission_Delete_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            await SeedAccounts(db);

            using var ctx = CreateContext(db, SaasAdmin);
            var ret = await NewUserPermissionApp(ctx).Delete(999_999);

            Assert.False(ret.Success);
            Assert.False(string.IsNullOrWhiteSpace(ret.ErrorMessage));
        }

        // ======================================================================
        // MedicinePatientIllnessHistoric: codigo morto (sem controller, sem registro no
        // Program.cs, entidade fora do DbContext). Nao e testado alem disso: a API nao tem como chegar nele.
        // ======================================================================

        [Fact]
        public void MedicinePatientIllnessHistoric_EntityIsNotMappedInDbContext_DeadCode()
        {
            // Documenta o motivo de ser codigo morto: o EF nao conhece a entidade.
            using var ctx = CreateContext(NewDb(), SaasAdmin);
            Assert.Null(ctx.Model.FindEntityType(typeof(MedicinePatientIllnessHistoric)));
            Assert.Null(ctx.Model.FindEntityType(typeof(MedicinePatientIllness)));
        }

    }
}
