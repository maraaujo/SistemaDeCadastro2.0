using SistemaDeCadastro.Domain.Context;
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
    public class MorbidadeApp
    {
        private readonly SistemaDeCadastroContext _context;
        private readonly IMedicamentoRepository _medicamentoRepository;
        private readonly IMedicamentoMorbidadeRepository _medicamentoMorbidadeRepository;
        private readonly IMorbidadaRepository _morbidadeRepository;

        public MorbidadeApp(SistemaDeCadastroContext context,
            IMedicamentoMorbidadeRepository medicamentoMorbidadeRepository, 
            IMedicamentoRepository medicamentoRepository, 
            IMorbidadaRepository morbidadaRepository)
        {
            context = _context;
            _medicamentoMorbidadeRepository = medicamentoMorbidadeRepository;
            _morbidadeRepository = morbidadaRepository;
            _medicamentoRepository = medicamentoRepository;
        }



        
    }
}
