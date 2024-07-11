using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.Model;


namespace SistemaDeCadastro.Data.Repository
{
    public class MedicamentoRepository : BaseRepository<Medicamento>
    {
        public MedicamentoRepository(SistemaDeCadastroContext context) : base(context)
        {
        }

        public async Task<List<Medicamento>> GetMedicamentoById(int id)
        {
          return  await this.FindBy(c => c.Id == id);  
        }
       
    }
}
