using SistemaDeCadastro.Domain.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Pageds
{
    public class PagedAcesseLog
    {
        public int Count { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int ItensPerPage { get => 15; }
        public List<AccessLogListDTO> AccessLogs { get; set; } = new();
    }
}
