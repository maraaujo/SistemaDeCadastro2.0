using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Context
{
    public class UsuarioContext : IdentityDbContext<Usuario>
    {
        public UsuarioContext(DbContextOptions<UsuarioContext> options)
            : base(options)
        {
            
        }
    }
}
