using System;

namespace SistemaDeCadastro.Domain.Filters
{
    public class AccessLogFilterDTO
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string UserEmail { get; set; }

        public string Action { get; set; }
        public DateTime? DateTime { get; set; }
        public string IpAddress { get; set; }
        public int Page { get; set; } = 1;
    }
}