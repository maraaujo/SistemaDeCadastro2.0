using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ICurrentUserService
    {
        long? UserId { get; }

        long? InstitutionId { get; }
    }
}
