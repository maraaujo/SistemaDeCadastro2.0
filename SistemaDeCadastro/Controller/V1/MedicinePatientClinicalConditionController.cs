using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicinePatientClinicalConditionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IMedicinePatientClinicalConditionApp _app;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de medicamentos programados).
        /// </summary>
        public MedicinePatientClinicalConditionController(IConfiguration configuration, IMedicinePatientClinicalConditionApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        /// <summary>
        /// GET api/MedicinePatientClinicalCondition/GetById?id= - qualquer usuario autenticado.
        /// Retorna o medicamento programado (medicamento + condicao clinica do acolhido + horario) pelo id, ou null.
        /// </summary>
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/MedicinePatientClinicalCondition/GetAll - qualquer usuario autenticado.
        /// Retorna todos os medicamentos programados visiveis para a instituicao do usuario.
        /// </summary>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// POST api/MedicinePatientClinicalCondition/Create - qualquer usuario autenticado.
        /// Programa um medicamento para um acolhido: reaproveita ou cria o medicamento e o vinculo
        /// acolhido + condicao clinica, valida as referencias da instituicao e recusa o mesmo
        /// medicamento no mesmo horario. Retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateMedicinePatientClinicalConditionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/MedicinePatientClinicalCondition/Update/{id} - qualquer usuario autenticado.
        /// Atualiza o medicamento programado; o id da rota sempre sobrescreve o Id do corpo. Retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(long id, UpdateMedicinePatientClinicalConditionDTO entity)
        {
            entity.Id = id;

            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/MedicinePatientClinicalCondition/Delete/{id} - qualquer usuario autenticado.
        /// Exclui o medicamento programado; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var ret = await _app.Delete(id);
            return Ok(ret);
        }

        // Limpeza global: apaga TODOS os medicamentos programados e as administracoes
        // vinculadas, de TODAS as instituicoes. Restrito ao administrador da plataforma
        // (policy SaasAdmin); o App reforca a mesma checagem.
        [Authorize(Policy = "SaasAdmin")]
        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var ret = await _app.DeleteAllScheduledMedicinesGlobal();
            return Ok(ret);
        }

        /// <summary>
        /// GET api/MedicinePatientClinicalCondition/GetByPatientClinicalConditionId?patientClinicalConditionId= - usuario autenticado.
        /// Retorna os medicamentos programados de uma condicao clinica especifica de um acolhido.
        /// </summary>
        [HttpGet("GetByPatientClinicalConditionId")]
        public async Task<IActionResult> GetByPatientClinicalConditionId(long patientClinicalConditionId)
        {
            var ret = await _app.GetByPatientClinicalConditionId(patientClinicalConditionId);
            return Ok(ret);
        }

        
        /// <summary>
        /// POST api/MedicinePatientClinicalCondition/GetMedicinePatientClinicalConditionByFilter - usuario autenticado.
        /// Retorna a lista paginada de medicamentos programados que atendem ao filtro enviado no corpo.
        /// </summary>
        [HttpPost("GetMedicinePatientClinicalConditionByFilter")]
        public async Task<IActionResult> GetMedicinePatientClinicalConditionByFilter(MedicinePatientClinicalConditionFilterDTO filter)
        {
            var ret = await _app.GetMedicinePatientClinicalConditionByFilter(filter);
            return Ok(ret);
        }
    }
}
