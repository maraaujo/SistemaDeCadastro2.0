using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using Microsoft.EntityFrameworkCore;

namespace SistemaDeCadastro.Infra.Repository
{
    public class InstitutionRepository : BaseRepository<Institution>, IInstitutionRepository
    {
        public InstitutionRepository(DbContext context) : base(context)
        {
        }
    }
}
