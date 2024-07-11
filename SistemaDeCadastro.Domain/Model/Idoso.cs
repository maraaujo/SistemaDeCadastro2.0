using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Model
{
    public class Idoso
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public DateTime DataDeNascimento { get; set; }
        public string Cpf { get; set; }
        public string TipoSanguineo { get; set; }
        public string CodigoBarras { get; set; }
        public string RestricoesAlimentares { get; set; }
        public ICollection<Familia> Familias { get; set; }
        public ICollection<MedicamentoIdosoDoenca> MedicamentoIdosoDoencas { get; set; }
        public ICollection<IdosoDoenca> IdosoDoencas { get; set; }
        public ICollection<IdosoFuncionario> Funcionarios { get; set; }
        public ICollection<Ministracao> Ministracoes { get; set; }  // Relação com Ministrações
    }
}
