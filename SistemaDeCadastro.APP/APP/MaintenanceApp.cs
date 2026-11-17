using System;
using System.Threading.Tasks;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    // Limpezas globais (todas as instituicoes). Sao operacoes destrutivas e irreversiveis,
    // exclusivas do administrador da plataforma: alem da policy SaasAdmin no controller,
    // cada metodo confere aqui o perfil do usuario logado (defesa em profundidade).
    public class MaintenanceApp : IMaintenanceApp
    {
        private const string NotAllowedMessage = "Apenas o administrador da plataforma pode executar esta operação.";

        private readonly IMaintenanceRepository _repo;
        private readonly ICurrentUserServiceContext _currentUser;

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de manutencao e o usuario logado.
        /// </summary>
        public MaintenanceApp(IMaintenanceRepository repo, ICurrentUserServiceContext currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Devolve em Data quantos registros existem em cada tabela operacional, somando todas as instituicoes.
        /// </summary>
        public Task<ApiResponse> GetSummary() =>
            Run(async ret =>
            {
                ret.Data = await _repo.GetSummary();
            });

        /// <summary>
        /// Apaga todos os acolhidos e tudo o que depende deles, de todas as instituicoes.
        /// Devolve em Data quantos registros foram apagados de cada tabela.
        /// </summary>
        public Task<ApiResponse> DeleteAllPatients() =>
            Run(async ret =>
            {
                var removed = await _repo.DeleteAllPatients(_currentUser.UserId, _currentUser.IpAddress);

                ret.Data = removed;
                ret.Message = $"Exclusão concluída: {removed.Patients} acolhido(s) e {removed.Total - removed.Patients} registro(s) dependente(s) removido(s).";
            });

        /// <summary>
        /// Zera os dados operacionais do sistema (acolhidos e dependentes, funcionarios, departamentos e
        /// medicamentos). Devolve em Data quantos registros foram apagados de cada tabela.
        /// </summary>
        public Task<ApiResponse> ResetSystemData() =>
            Run(async ret =>
            {
                var removed = await _repo.ResetSystemData(_currentUser.UserId, _currentUser.IpAddress);

                ret.Data = removed;
                ret.Message = $"Dados do sistema zerados: {removed.Total} registro(s) removido(s).";
            });

        // Parte comum a todas as operacoes: confere o perfil, executa e trata o erro.
        private async Task<ApiResponse> Run(Func<ApiResponse, Task> operation)
        {
            var ret = new ApiResponse();
            try
            {
                if (!_currentUser.IsPlatformAdmin)
                {
                    ret.Success = false;
                    ret.ErrorMessage = NotAllowedMessage;
                    return ret;
                }

                await operation(ret);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.Data = null;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }
    }
}
