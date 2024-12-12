using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.SistemaCadastroContext;


namespace SistemaDeCadastroTeste
{
    public class TestDbContextFactory
    {
        public static SistemaCadastroContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<SistemaCadastroContext>()
            .UseNpgsql("Host=35.198.34.142;Database=tlcentral_dev;Username=tlpgsql;Password=f254afd0b0fb9ed7f66d142367981c37")
            .Options;

            var context = new SistemaCadastroContext(options);

            
            context.Database.EnsureCreated();
            return context;
        }
    }
}
