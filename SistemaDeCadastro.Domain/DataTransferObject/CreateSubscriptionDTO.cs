using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateSubscriptionDTO
    {
        public long InstitutionId { get; set; }
        public long PlanId { get; set; }
        public string Status { get; set; }
        public string ExternalSubscriptionId { get; set; }
    }
}