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

        // Usuario logado: claim "UserId" do JWT (= LoginAccount.UserId, que e a
        // chave referenciada pelo log_acesso.id_usuario).
        public long? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

                if (long.TryParse(value, out var userId))
                    return userId;

                return null;
            }
        }

        // IP de origem. Atras de proxy/Cloud Run o IP real vem no X-Forwarded-For
        // (o primeiro da lista); senao usa o IP da conexao.
        public string? IpAddress
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    return null;

                var forwarded = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                var remote = httpContext.Connection.RemoteIpAddress;
                if (remote != null && remote.IsIPv4MappedToIPv6)
                    remote = remote.MapToIPv4();

                var ip = !string.IsNullOrWhiteSpace(forwarded)
                    ? forwarded.Split(',')[0].Trim()
                    : remote?.ToString();

                if (string.IsNullOrWhiteSpace(ip))
                    return null;

                // Coluna ip_acesso tem 45 caracteres.
                return ip.Length > 45 ? ip[..45] : ip;
            }
        }
    }
}
