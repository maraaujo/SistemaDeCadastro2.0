using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Domain.DataTransferObject;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Data.Interface;

namespace SistemaDeCadastro.Infra.Repository
{
    public class AdminRepository : BaseRepository<Idoso>, IAdminRepository
    {
        private readonly IFuncionarioRepository _funcionarioRepository;
        public AdminRepository(SistemaDeCadastroContext context) : base(context)
        {
        }

        public async Task<List<Idoso>> GetAllIdoso()
        {
            try
            {
               var ret = await this.GetAll();
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao colocar informações do  idoso: {ex.Message}");
            }   
        }

        public async Task<List<Idoso>> GetIdosoById(int id)
        {
            try
            {
               return await this.FindBy(c => c.Id == id);
                
            }
            catch (Exception err) 
            {
                throw new Exception($"Erro ao obter idoso por ID: {err.Message}"); 
            }
        }

        public async Task<PagedIdosoDTO> GetIdoso(IdosoFilterDTO filter)
        {
            var iRet = _context.Idosos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Nome))
            {
                var lowerName = filter.Nome.ToLower();
                iRet = iRet.Where(c => c.Nome.ToLower().StartsWith(lowerName) || c.Nome.ToLower().EndsWith(lowerName));
            }
            if (!string.IsNullOrWhiteSpace(filter.Sobrenome))
            {
                var lowerSobrenome = filter.Sobrenome.ToLower();
                iRet = iRet.Where(c => c.Sobrenome.ToLower().StartsWith(lowerSobrenome) || c.Sobrenome.ToLower().EndsWith(lowerSobrenome));
            }
            if (!string.IsNullOrWhiteSpace(filter.Cpf))
            {
                var lowerCpf = filter.Cpf.ToLower();
                iRet = iRet.Where(c => c.Cpf.ToLower().StartsWith(lowerCpf) || c.Cpf.ToLower().EndsWith(lowerCpf));
                //}
                //if (filter.Doenca != null && !string.IsNullOrEmpty(filter.Doenca.Nome))
                //{
                //    iRet = iRet.Where(c => c.Doenca.Nome.ToLower() == filter.Doenca.Nome.ToLower());
                //}
                //if (filter.Familia != null && !string.IsNullOrEmpty(filter.Familia.Nome))
                //{
                //    iRet = iRet.Where(c => c.Familia.Nome.ToLower() == filter.Familia.Nome.ToLower());
                //}
                //if (filter.Medicamento != null && !string.IsNullOrEmpty(filter.Medicamento.Nome))
                //{
                //    iRet = iRet.Where(c => c.Medicamento.Nome.ToLower() == filter.Medicamento.Nome.ToLower());
                //}
            }
            PagedIdosoDTO ret = new()
            {
                Page = filter.Page,
                Count = await iRet.CountAsync()
            };

            ret.TotalPages = ret.Count % 10 > 0 ? (ret.Count / 10) + 1 : ret.Count / 10;

            int page = filter.Page - 1;

            ret.Idoso = await iRet.OrderByDescending(c => c.Id)
                                  .Skip(page * ret.ItensPerPage)
                                  .Take(ret.ItensPerPage)
                                  .Select(c => new IdosoDTO
                                  {
                                      Nome = c.Nome,
                                      Sobrenome = c.Sobrenome,
                                      Cpf = c.Cpf,
                                      //Doenca = c.Doenca,
                                      //Medicamento = c.Medicamento,
                                      //Familia = c.Familia,
                                  }).ToListAsync();

            return ret;
        }
        public async Task<List<ElderlyMedicineDTO>> GetElderlyMedicine(int id) //recebi o parametro
        {
            var resultado = from mid in _context.MedicamentoIdosoDoencas
                            join medicamento in _context.Medicamentos on mid.Id equals medicamento.Id
                            join idoso in _context.Idosos on mid.Id equals idoso.Id
                            where medicamento.Id == id
                            select new ElderlyMedicineDTO
                            {
                                IdosoNome = idoso.Nome,
                                IdosoSobrenome = idoso.Sobrenome,
                                Medicamentos = medicamento.Nome,
                                MedicamentoDosagem = mid.MedicamentoDosagem
                            };



            return await resultado.ToListAsync();
        }

        public async Task<Funcionario> ValidateFuncionario(string email,string senha)
        {
            
            try
            {
                if ((string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha)))
                {
                    throw new Exception("Campo obrigatório");

                }
               var funcionario = await _context.Funcionarios.FirstOrDefaultAsync( f  => f.Email == email && f.Senha == senha);

                if (funcionario == null) {
                    throw new Exception("Funcionario não encontrado ou senha incorreta");
                    }

                return funcionario;
            }
            catch (Exception ex) 
            {
                throw new Exception($"Erro ao validar funcionário: {ex.Message}", ex);
            }
        }
    }
}

