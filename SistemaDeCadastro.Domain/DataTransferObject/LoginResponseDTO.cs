using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class LoginResponseDTO
    {
        public long UserId { get; set; }

        public string Email { get; set; }

        public string UserType { get; set; }

        public string Token { get; set; }
    }
}
