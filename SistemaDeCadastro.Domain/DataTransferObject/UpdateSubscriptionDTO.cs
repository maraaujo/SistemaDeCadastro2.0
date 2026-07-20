using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UpdateSubscriptionDTO
    {
        public long Id { get; set; }
        public long InstitutionId { get; set; }
        public long PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public string ExternalSubscriptionId { get; set; }
    }
}