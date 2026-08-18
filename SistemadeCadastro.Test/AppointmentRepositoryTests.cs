using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemadeCadastro.Test
{
    public class AppointmentRepositoryTests
    {
        private SistemaDeCadastroContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SistemaDeCadastroContext(options, new TestCurrentUserService());
        }

        [Fact]
        public async Task GetAllAppointment_Returns_MappedDtos()
        {
            using var context = CreateContext("appt_all");
            var patient = new Patient { Name = "P1", Document = "1", Phone = "0", Gender = "M", Cpf = "1", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            var appt = new Appointment { PatientId = patient.Id, UserId = 1, AppointmentType = "T", DateTime = System.DateTime.Now, Responsible = "R", Status = "S", Observations = "O" };
            context.Appointments.Add(appt);
            await context.SaveChangesAsync();

            var repo = new AppointmentRepository(context);
            var list = await repo.GetAllAppointment();

            Assert.Single(list);
            Assert.Equal("P1", list.First().PatientName);
        }

        [Fact]
        public async Task GetById_Returns_AppointmentDTO()
        {
            using var context = CreateContext("appt_byid");
            var patient = new Patient { Name = "P2", Document = "2", Phone = "0", Gender = "M", Cpf = "2", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            var appt = new Appointment { PatientId = patient.Id, UserId = 2, AppointmentType = "T2", DateTime = System.DateTime.Now, Responsible = "R2", Status = "S2", Observations = "O2" };
            context.Appointments.Add(appt);
            await context.SaveChangesAsync();

            var repo = new AppointmentRepository(context);
            var dto = await repo.GetById(appt.Id);

            Assert.NotNull(dto);
            Assert.Equal("P2", dto.PatientName);
        }
    }
}
