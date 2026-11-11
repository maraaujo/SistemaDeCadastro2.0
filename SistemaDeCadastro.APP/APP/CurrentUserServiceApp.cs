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

        /// <summary>
        /// Recebe por injecao de dependencia o acesso ao HttpContext da requisicao atual.
        /// </summary>
        public CurrentUserServiceApp(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Id do usuario logado, lido da claim "UserId" do token; null se nao houver claim valida.
        /// Atencao: quando a claim e valida, o getter retorna a propria propriedade UserId (recursao infinita),
        /// em vez da variavel userId lida.
        /// </summary>
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
        /// <summary>
        /// Id da instituicao do usuario logado, lido da claim "institutionId" do token; null se ausente ou invalida.
        /// </summary>
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

