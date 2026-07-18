using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class AccessLogListDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserEmail { get; set; }
        public string Action { get; set; }
        public DateTime DateTime { get; set; }
        public string IpAddress { get; set; }
    }
}