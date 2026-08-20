using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateLoginAccountDTO
    {

            public string Nome { get; set; }
            public string Email { get; set; }

            public string Password { get; set; }

            public string UserType { get; set; }

            public long? InstitutionId { get; set; }

            public bool Active { get; set; } = true;
        }

    }

