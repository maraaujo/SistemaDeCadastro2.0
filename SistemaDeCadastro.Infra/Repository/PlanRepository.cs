using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PlanRepository : BaseRepository<Plan>, IPlanRepository
    {
        private readonly SistemaDeCadastroContext _context;
        public PlanRepository(SistemaDeCadastroContext context) : base(context)
        {
          this._context = context; 
        }
    }
}
