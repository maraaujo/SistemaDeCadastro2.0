using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ICurrentUserService
    {
        /// <summary>Id do usuario logado (claim "UserId" do token), ou null se nao houver.</summary>
        long? UserId { get; }

        /// <summary>Id da instituicao do usuario logado (claim "institutionId" do token), ou null se nao houver.</summary>
        long? InstitutionId { get; }
    }
}
