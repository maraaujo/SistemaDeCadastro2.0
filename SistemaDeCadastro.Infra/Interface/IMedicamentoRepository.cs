using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicamentoRepository : IBaseRepository<Medicamento>
    {
        Task<List<MedicamentoPosologiaDTO>> GetMorbidadePosologiaPorMedicamento(int medicamentoId);
        Task UpdateMedicamento(Medicamento updateMedicamento);
        Task<List<Medicamento>> GetMedicamentoById(int id);
        Task CreateMedicmaento(Medicamento medicamento);
        Task<List<Medicamento>> GetAllMedicamentos();
        Task<List<Medicamento>> GetMedicamentoPorLaboratorio(int laboratorioId);
        Task Create(MedicamentoDTO medicamento);
    }
}
