using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class MedicamentoApp
    {
        private readonly SistemaDeCadastroContext _context;
        private readonly IMedicamentoRepository _medicamentoRepository;
        private readonly IMedicamentoMorbidadeRepository _medicamentoMorbidadeRepository;
        private readonly IMorbidadaRepository _morbidadeRepository;
        public MedicamentoApp(SistemaDeCadastroContext context, 
            IMedicamentoRepository medicamentoRepository,
            IMedicamentoMorbidadeRepository medicamentoMorbidadeRepository, 
            IMorbidadaRepository morbidadaRepository)
        {
            this._context = context;
            this._medicamentoMorbidadeRepository = medicamentoMorbidadeRepository ;
            this._medicamentoRepository = medicamentoRepository;
            this._morbidadeRepository = morbidadaRepository;
        }

        //Estou criando um medicamento e relacioando a uma morbidade e a tabela intermediaria delas 
        public async Task Save(MedicamentoDTO med)
        {
            try
            {
                Morbidade morb = null;
                if (med.MorbidadeCod == 0)
                {
                    morb = new Morbidade()
                    {
                        Nome = med.Morbidade.Nome,
                        Cod = med.Morbidade.Cod, // Verifique se Cod é gerado automaticamente
                                                 // Decidir o que fazer com a propriedade PessoaCod
                    };
                    await _morbidadeRepository.Create(morb);
                    med.MorbidadeCod = morb.Cod; // Atualiza o MorbidadeCod com o novo código
                }

                Medicamento medicamento = new()
                {
                    Cod = med.Cod, // Verificar se Cod já foi gerado ou deve ser gerado aqui
                    Nome = med.Nome,
                    LaboratorioCod = med.LaboratorioCod,
                };
                await _medicamentoRepository.Create(medicamento);

                MedicamentoMorbidade medicamentoMorbidade = new()
                {
                    MorbidadeCod = med.MorbidadeCod != 0 ? med.MorbidadeCod : morb.Cod,
                    MedicamentoCod = medicamento.Cod // Usar o Cod do medicamento criado
                };
                await _medicamentoMorbidadeRepository.Create(medicamentoMorbidade);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro: {ex.Message}");
            }
        }

        public async Task Update(Medicamento medicamento)
        {
          await this._medicamentoRepository.Update(medicamento);
        }
       
        public async Task<List<Medicamento>> GetAllMedicamentos()
        {
            return await this._medicamentoRepository.GetAll();
        }
        public async Task<List<Medicamento>> GetMedicamentoPorLaboratorio(int laboratorioId)
        {
            return await this._medicamentoRepository.GetMedicamentoPorLaboratorio(laboratorioId);
        }

        public async Task<List<MedicamentoPosologiaDTO>> GetMorbidadePosologiaPorMedicamento(int medicamentoId)
        {
            return await this._medicamentoRepository.GetMorbidadePosologiaPorMedicamento(medicamentoId);
        }


    }
}
