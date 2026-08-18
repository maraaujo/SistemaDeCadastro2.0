using Microsoft.AspNetCore.Http;
using SistemaDeCadastro.APP.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class CurrentUserServiceContext : ICurrentUserServiceContext
    {
        //objeto que tem os dados da requisição que chega no backend
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserServiceContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long? InstitutionId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                var value =
                    user?.FindFirst("institutionId")?.Value ??
                    user?.FindFirst("InstitutionId")?.Value ??
                    user?.FindFirst("id_instituicao")?.Value;

                if (long.TryParse(value, out var institutionId))
                    return institutionId;

                return null;
            }
        }
    }
}
