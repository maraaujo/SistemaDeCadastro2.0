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
               "server=127.0.0.1;port=3306;database=sistema_cadastro_tcc;user=sistema_user;password=irmaos03;",
                new MySqlServerVersion(new Version(8, 0, 46))
            );

            return new SistemaDeCadastroContext(optionsBuilder.Options);
        }
    }
}