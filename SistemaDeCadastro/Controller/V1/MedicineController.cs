using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicineController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IMedicineApp _app;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de medicamentos).
        /// </summary>
        public MedicineController(IConfiguration configuration, IMedicineApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        /// <summary>
        /// GET api/Medicine/GetMedicineById?id= - qualquer usuario autenticado.
        /// Retorna uma lista com o medicamento do id (vazia se nao existir ou for de outra instituicao).
        /// </summary>
        [HttpGet("GetMedicineById")]
        public async Task<IActionResult> GetMedicineById(long id)
        {
            var item = await _app.GetMedicineId(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/Medicine/GetAllMedicines - qualquer usuario autenticado.
        /// Retorna todos os medicamentos cadastrados na instituicao do usuario logado.
        /// </summary>
        [HttpGet("GetAllMedicines")]
        public async Task<IActionResult> GetAllMedicines()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// POST api/Medicine/CreateMedicine - qualquer usuario autenticado.
        /// Cadastra um medicamento na instituicao do usuario logado (falha se o usuario nao tiver instituicao).
        /// Retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("CreateMedicine")]
        public async Task<IActionResult> CreateMedicine(CreateMedicineDTO entity)
        {
            var ret = await _app.CreateMedicine(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/Medicine/Update/{id} - qualquer usuario autenticado.
        /// Atualiza nome, descricao, dosagem e via de administracao do medicamento. Se o corpo vier
        /// sem Id, usa o id da rota. Retorna ApiResponse ("Medicamento nao encontrado." se nao existir).
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateMedicine(long id, UpdateMedicineDTO entity)
        {
            // Garante o Id vindo da rota (o front pode mandar so na URL, nao no corpo).
            // Sem isso, entity.Id ficava 0 e o update "nao encontrava" o medicamento,
            // retornando 200 mas sem salvar nada.
            if (entity.Id == 0)
                entity.Id = id;

            var ret = await _app.UpdateMedicine(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Medicine/DeleteMedicine/{idMedicine} - qualquer usuario autenticado.
        /// Monta um MedicineDTO so com o id e pede a exclusao do medicamento; retorna ApiResponse.
        /// </summary>
        [HttpDelete("DeleteMedicine/{idMedicine}")]
        public async Task<IActionResult> DeleteMedicine(long idMedicine)
        {
            var dto = new MedicineDTO { Id = idMedicine };
            var ret = await _app.DeleteMedicine(dto);
            return Ok(ret);
        }
        /// <summary>
        /// POST api/Medicine/GetMedicineByFilter - qualquer usuario autenticado.
        /// Retorna a lista paginada de medicamentos que atendem ao filtro enviado no corpo.
        /// </summary>
        [HttpPost("GetMedicineByFilter")]
        public async Task<IActionResult> GetMedicineByFilter(MedicineFilterDTO filter)
        {
            var ret = await _app.GetMedicineByFilter(filter);
            return Ok(ret);
        }
    }
}
