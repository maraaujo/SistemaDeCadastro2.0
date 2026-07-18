using System.Collections.Generic;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Domain.Pageds
{
    public class PagedCareServiceDTO
    {
        public int Count { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int ItensPerPage { get => 15; }
        public List<CareServiceListDTO> CareServices { get; set; } = new();
    }
}