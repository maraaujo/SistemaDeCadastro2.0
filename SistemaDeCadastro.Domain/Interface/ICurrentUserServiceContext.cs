using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ICurrentUserServiceContext
    {
        long? InstitutionId { get; }
    }
}
