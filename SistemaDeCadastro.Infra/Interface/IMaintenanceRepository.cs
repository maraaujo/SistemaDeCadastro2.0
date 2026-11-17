using SistemaDeCadastro.Domain.DataTransferObject;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    /// <summary>
    /// Operacoes de manutencao em massa, globais (todas as instituicoes). Uso exclusivo do
    /// administrador da plataforma: a restricao e feita no controller (policy SaasAdmin) e no App.
    /// </summary>
    public interface IMaintenanceRepository
    {
        /// <summary>Conta os registros existentes hoje em cada tabela operacional, somando todas as instituicoes.</summary>
        Task<MaintenanceCountsDTO> GetSummary();

        /// <summary>
        /// Apaga todos os acolhidos e tudo o que depende deles (administracoes, medicamentos programados,
        /// condicoes clinicas, pagamentos, agendamentos, vinculos e responsaveis), em uma unica transacao.
        /// Devolve quantos registros foram apagados de cada tabela.
        /// </summary>
        Task<MaintenanceCountsDTO> DeleteAllPatients(long? userId, string? ipAddress);

        /// <summary>
        /// Zera os dados operacionais do sistema: tudo o que DeleteAllPatients apaga e tambem
        /// funcionarios, departamentos e medicamentos. Contas de acesso, instituicoes, planos,
        /// assinaturas, cadastros de apoio e a trilha de auditoria sao mantidos.
        /// </summary>
        Task<MaintenanceCountsDTO> ResetSystemData(long? userId, string? ipAddress);
    }
}
