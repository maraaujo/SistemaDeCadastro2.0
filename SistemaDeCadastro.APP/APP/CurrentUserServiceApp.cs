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
    public class CurrentUserServiceApp : ICurrentUserService
    {
        //objeto que tem os dados da requisição que chega no backend
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserServiceApp(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long? UserId
        {
            get
            {
                //ess
                var value = _httpContextAccessor.HttpContext?.User?
                    .FindFirst("UserId")?.Value;
                //o out é um modificador de parametro usado para passar uma variavel por referencia
                if (long.TryParse(value, out var userId))
                    return UserId;
                return null;
            }
        }
        public long? InstitutionId
        {

            get
            {
                var value = _httpContextAccessor.HttpContext?.User ?
                       .FindFirst("institutionId")?.Value;
                if (long.TryParse(value, out var institutionId))
                    return institutionId;

                return null;
            }
        }
    }
    }

