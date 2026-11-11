using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResponsibleController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IResponsibleApp _responsibleApp;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de responsaveis).
        /// </summary>
        public ResponsibleController(IConfiguration configuration, IResponsibleApp responsibleApp)
        {
            this._configuration = configuration;
            this._responsibleApp = responsibleApp;
        }

        /// <summary>
        /// GET api/Responsible/GetResponsibleById?id= - qualquer usuario autenticado.
        /// Retorna o responsavel (familiar/contato) do acolhido pelo id, ou null.
        /// </summary>
        [HttpGet("GetResponsibleById")]
        public async Task<IActionResult> GetResponsibleById(long id)
        {
            var responsible = await _responsibleApp.GetById(id);
            return Ok(responsible);
        }

        /// <summary>
        /// GET api/Responsible/GetAllResponsibles - qualquer usuario autenticado.
        /// Retorna todos os responsaveis visiveis para a instituicao do usuario.
        /// </summary>
        [HttpGet("GetAllResponsibles")]
        public async Task<IActionResult> GetAllResponsibles()
        {
            var responsibles = await _responsibleApp.GetAll();
            return Ok(responsibles);
        }

        /// <summary>
        /// POST api/Responsible/CreateResponsible - qualquer usuario autenticado.
        /// Cadastra um responsavel; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("CreateResponsible")]
        public async Task<IActionResult> CreateResponsible(CreateResponsibleDTO responsible)
        {
            var ret = await _responsibleApp.Create(responsible);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/Responsible/Update/{id} - qualquer usuario autenticado.
        /// Atualiza o responsavel com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateResponsible(UpdateResponsibleDTO responsible)
        {
            var ret = await _responsibleApp.Update(responsible);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Responsible/DeleteResponsible/{idResponsible} - qualquer usuario autenticado.
        /// Exclui o responsavel; retorna ApiResponse.
        /// </summary>
        [HttpDelete("DeleteResponsible/{idResponsible}")]
        public async Task<IActionResult> DeleteResponsible(long idResponsible)
        {
            var ret = await _responsibleApp.Delete(idResponsible);
            return Ok(ret);
        }
    }
}
