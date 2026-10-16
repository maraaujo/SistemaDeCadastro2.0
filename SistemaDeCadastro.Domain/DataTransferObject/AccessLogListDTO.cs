using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class AccessLogListDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserEmail { get; set; }
        public string Action { get; set; }
        public string UserType { get; set; }
        public DateTime DateTime { get; set; }
        public string IpAddress { get; set; }
        public string UserName { get; set; }

        // Detalhe da auditoria: campos alterados (valor antigo -> novo) ou o nome
        // do registro criado/excluido.
        public string Observation { get; set; }
    }
}