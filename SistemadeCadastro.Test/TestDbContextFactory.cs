using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemadeCadastro.Test
{
    public class TestDbContextFactory
    {
        public static SistemaCadastroContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<SistemaCadastroContext>()
           .UseNpgsql("Host=localhost;Port=5432;Database=sistema_de_cadastro_test;Username=seu_usuario;Password=sua_senha;")
           .Options;

            var context = new SistemaCadastroContext(options);

            context.Database.EnsureCreated();
            return context;
        }
    }
}
