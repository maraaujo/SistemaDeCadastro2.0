using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;
using Xunit;

namespace SistemaDeCadastroTeste
{
    public class PatientAppTetes
    {
        private DbContextOptions<SistemaCadastroContext>  context;
        [Fact]
        public async Task GetPatientById()
        {
            using var context = TestDbContextFactory.CreateDbContext();
            var patient = new Patient
            {
                Id = 1,
                Name = "Felipe",

            };
        }
    }
}