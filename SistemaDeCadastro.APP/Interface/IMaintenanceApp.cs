using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    /// <summary>
    /// Manutencao de dados do administrador da plataforma: resumo do que existe e limpezas globais.
    /// </summary>
    public interface IMaintenanceApp
    {
        /// <summary>Devolve em Data a quantidade de registros de cada tabela operacional (todas as instituicoes).</summary>
        Task<ApiResponse> GetSummary();

        /// <summary>Apaga todos os acolhidos e seus dependentes, de todas as instituicoes.</summary>
        Task<ApiResponse> DeleteAllPatients();

        /// <summary>Zera os dados operacionais do sistema, mantendo contas de acesso, instituicoes, planos e cadastros de apoio.</summary>
        Task<ApiResponse> ResetSystemData();
    }
}
