


using Microsoft.AspNetCore.Identity;

namespace SistemaDeCadastro.Domain.Model
{
    public class IdosoFuncionario 
    {
        
        public int Id { get; set; }
        public int IdosoID { get; set; }
        public Idoso Idoso { get; set; }
        public int FuncionarioID { get; set; }
        public Funcionario Funcionario { get; set; }
    }
}
