﻿using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IFuncionarioRepository : IBaseRepository<Funcionario>
    {
        Task<List<Funcionario>> GetFuncionairoByEmail(string email);
        Task<List<Funcionario>> GetFuncionairoById(int id);
        Task<FuncionarioDTO> CreateFuncionario(FuncionarioDTO funcionariodto);
    }
}
