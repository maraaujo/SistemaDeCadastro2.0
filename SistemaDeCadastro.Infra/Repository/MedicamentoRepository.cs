using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using System.Runtime.CompilerServices;


namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicamentoRepository : BaseRepository<Medicamento>
    {
        public MedicamentoRepository(SistemaDeCadastroContext context) : base(context)
        {
        }

        public async Task<List<Medicamento>> GetAllMedicamentos()
        {
            try
            {
                var ret = await this.GetAll();
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar informações: {ex.Message}");
            }
        }
        public async Task CreateMedicmaento(Medicamento medicamento)
        {
            await this.Create(medicamento);
        }
        public async Task<List<Medicamento>> GetMedicamentoById(int id)
        {
          return  await this.FindBy(c => c.Cod == id);  
        }

        public async Task UpdateMedicamento(Medicamento updateMedicamento)
        {
            try
            {
                await this.Update(updateMedicamento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar informações : {ex.Message}");
            }
        }
        //associar medicmaento às morbidades e posologias
        public async Task<List<MedicamentoPosologiaDTO>> GetMorbidadePosologiaPorMedicamento( int medicamentoId)
        {
            var med = await (from medi in this._context.Medicamentos
                             join poso in this._context.Posologias
                             on medi.Cod equals poso.MedicamentoCod
                             where medi.Cod == medicamentoId
                             select new MedicamentoPosologiaDTO
                             {
                                 MedicamentoCod = medi.Cod,
                                 MedicamentoNome = medi.Nome,
                                 PosologiaDataInicial = poso.DataInicial,
                                 PosologiaDataFinal = poso.DataFinal,
                                 PosologiaDose = poso.Dose
                             }).ToListAsync();

            return med;

        }

        public async Task<List<Medicamento>> GetMedicamentoPorLAboratorio( int laboratorioId)
        {
            var med = await (from medi in this._context.Medicamentos
                             join lab in this._context.Laboratorios
                             on medi.LaboratorioCod equals lab.Cod
                             where medi.LaboratorioCod == laboratorioId
                             select new Medicamento
                             {
                                 Cod = medi.Cod,
                                 Nome = medi.Nome,
                                 LaboratorioCod = medi.LaboratorioCod,

                             }).ToListAsync();
            return med;
        }
        //buscar medicamento por nome, laboratorio

        
    }
}
