using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using Microsoft.EntityFrameworkCore;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PlanRepository : BaseRepository<Plan>, IPlanRepository
    {
        public PlanRepository(DbContext context) : base(context)
        {
        }
    }
}
