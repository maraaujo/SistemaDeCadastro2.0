using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ICurrentUserServiceContext
    {
        long? InstitutionId { get; }

        // Usuario logado (claim "UserId" do JWT = LoginAccount.UserId). Usado na
        // auditoria (log_acesso). Implementacao padrao null para nao quebrar fakes.
        long? UserId => null;

        // IP de origem da requisicao, gravado junto com cada alteracao auditada.
        string? IpAddress => null;

        // Papel do usuario logado (claim Role do JWT = LoginAccount.UserType).
        string? Role => null;

        // Administrador da plataforma: papel administrativo E sem instituicao no token.
        // Unico perfil que enxerga dados de todas as instituicoes.
        bool IsPlatformAdmin =>
            !InstitutionId.HasValue &&
            string.Equals(Role, "Administrador", StringComparison.OrdinalIgnoreCase);
    }
}
