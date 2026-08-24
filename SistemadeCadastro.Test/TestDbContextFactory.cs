using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.SistemaCadastroContext;

namespace SistemadeCadastro.Test
{
    public static class TestDbContextFactory
    {
        public static SistemaDeCadastroContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SistemaDeCadastroContext>();

            optionsBuilder.UseMySql(
                new MySqlServerVersion(new Version(8, 4, 8)))
           ;

            return new SistemaDeCadastroContext(optionsBuilder.Options, new TestCurrentUserService());
        }
    }

    internal class TestCurrentUserService : SistemaDeCadastro.APP.Interface.ICurrentUserServiceContext
    {
        public long? InstitutionId => null;
    }
}