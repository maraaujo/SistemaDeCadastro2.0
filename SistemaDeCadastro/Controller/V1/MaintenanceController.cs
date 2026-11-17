using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;

namespace SistemaDeCadastro.Controller.V1
{
    // Manutencao de dados do SaaS: limpezas globais (todas as instituicoes).
    // Acesso restrito ao administrador da plataforma pela policy "SaasAdmin"
    // (usuario autenticado, com papel administrativo e sem vinculo a uma instituicao).
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "SaasAdmin")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceApp _app;

        /// <summary>
        /// Recebe a camada de aplicacao de manutencao por injecao.
        /// </summary>
        public MaintenanceController(IMaintenanceApp app)
        {
            _app = app;
        }

        /// <summary>
        /// GET api/Maintenance/Summary - somente admin da plataforma (policy "SaasAdmin").
        /// Retorna quantos registros existem hoje em cada tabela operacional, somando todas as instituicoes.
        /// </summary>
        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary()
        {
            var ret = await _app.GetSummary();
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Maintenance/DeleteAllPatients - somente admin da plataforma.
        /// Apaga TODOS os acolhidos e seus dependentes (responsaveis, condicoes clinicas, medicamentos
        /// programados, administracoes, agendamentos e pagamentos) de TODAS as instituicoes. Irreversivel.
        /// </summary>
        [HttpDelete("DeleteAllPatients")]
        public async Task<IActionResult> DeleteAllPatients()
        {
            var ret = await _app.DeleteAllPatients();
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Maintenance/ResetSystemData - somente admin da plataforma.
        /// Zera os dados operacionais de TODAS as instituicoes: acolhidos e dependentes, funcionarios,
        /// departamentos e medicamentos. Mantem contas de acesso, instituicoes, planos, assinaturas,
        /// cadastros de apoio e a trilha de auditoria. Irreversivel.
        /// </summary>
        [HttpDelete("ResetSystemData")]
        public async Task<IActionResult> ResetSystemData()
        {
            var ret = await _app.ResetSystemData();
            return Ok(ret);
        }
    }
}
