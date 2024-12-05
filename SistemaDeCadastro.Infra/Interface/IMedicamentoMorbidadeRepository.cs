using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicamentoMorbidadeRepository : IBaseRepository<MedicamentoMorbidade>
    {
        Task<List<MedicamentoMorbidadeDTO>> GetMedicmaentoByMorbidade(int code);
        Task<List<MedicamentoMorbidadeDTO>> GetMorbidadeByMedicamento(int code);
    }
}
