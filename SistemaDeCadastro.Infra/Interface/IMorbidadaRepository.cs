using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMorbidadaRepository : IBaseRepository<Morbidade>
    {
        Task<List<Morbidade>> GetMorbidadeById(int code);
    }
}
