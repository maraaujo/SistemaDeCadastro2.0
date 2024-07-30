using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class DepartamentoApp : IDepartamentoApp
    {
        private readonly IDepartamentoRepository _departamentoRepository;

        public DepartamentoApp(IDepartamentoRepository departamentoRepository)
        {
            _departamentoRepository = departamentoRepository;
        }
        public async Task<DepartamentoDTO> CreateDepartamento(DepartamentoDTO departamentoDto)
        {
            await this._departamentoRepository.CreateDepartamento(departamentoDto);
            return departamentoDto;
        }
        public async Task<List<Departamento>> GetDepartamentoById(int id)
        {
            var ret = await this._departamentoRepository.GetDepartamentoById(id);
            return ret;
        }
    }
    
}
