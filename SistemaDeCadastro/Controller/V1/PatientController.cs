
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IPatientApp _patientApp;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de acolhidos).
        /// </summary>
        public PatientController(IConfiguration configuration, IPatientApp patientApp)
        {
            this._configuration = configuration;
            this._patientApp = patientApp;
        }
        

        /// <summary>
        /// GET api/Patient/GetPatientById?id= - qualquer usuario autenticado.
        /// Retorna uma lista com o acolhido do id (vazia se nao existir ou for de outra instituicao).
        /// </summary>
        [HttpGet("GetPatientById")]
        public async Task<IActionResult> GetPatientById(long id)
        {
            var patient = await _patientApp.GetPatientById(id);
            return Ok(patient);
        }
        /// <summary>
        /// GET api/Patient/GetAllPatients - qualquer usuario autenticado.
        /// Retorna todos os acolhidos da instituicao do usuario logado.
        /// </summary>
        [HttpGet("GetAllPatients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientApp.GetAllPatients();
            return Ok(patients);
        }
        /// <summary>
        /// GET api/Patient/GetByAny?patient= - qualquer usuario autenticado.
        /// Chama a busca de acolhido por texto, mas descarta o resultado e sempre responde 200 sem corpo.
        /// </summary>
        [HttpGet("GetByAny")]
        public async Task<IActionResult> GetPatientByAny(string patient)
        {
            await _patientApp.GetPatientByAny(patient);
            return Ok();
        }
        /// <summary>
        /// POST api/Patient/GetPatientFilter - qualquer usuario autenticado.
        /// Retorna ApiResponse com a lista paginada de acolhidos que atendem ao filtro em Data.
        /// Em caso de erro, Success = false e a mensagem vai em Message (nao em ErrorMessage).
        /// </summary>
        [HttpPost("GetPatientFilter")]
        public async Task<IActionResult> GetPatientFilter(PatientFilterDTO filter)
        {
            ApiResponse response = new ApiResponse();

            try
            {
                response.Data = await this._patientApp.FilterPatient(filter);
                response.Success = true;
            }
            catch (Exception err)
            {
                response.Success = false;
                response.Message = ApiErrors.From(err);
            }

            return Ok(response);
        }
        /// <summary>
        /// GET api/Patient/GetPatientDetails/{id} - qualquer usuario autenticado.
        /// Retorna ApiResponse com os detalhes completos do acolhido em Data (null se nao existir
        /// ou for de outra instituicao). Erros vao em Message.
        /// </summary>
        [HttpGet("GetPatientDetails/{id}")]
        public async Task<IActionResult> DetailsPatient(long id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await this._patientApp.DetailsPatient(id);
                response.Success = true;
            }
            catch (Exception err)
            {
                response.Success = false;
                response.Message = ApiErrors.From(err);
            }

            return Ok(response);
        }
        /// <summary>
        /// POST api/Patient/CreatePatient - qualquer usuario autenticado.
        /// Cadastra o acolhido na instituicao do usuario junto com responsaveis, condicoes clinicas e
        /// medicamentos programados. Valida CPF (e unicidade) e telefones. Retorna ApiResponse.
        /// </summary>
        [HttpPost("CreatePatient")]
        public async Task<IActionResult> CreatePatient(CreatePatientDTO patient)
        {
            var ret = await _patientApp.CreatePatient(patient);
            return Ok(ret);
        }
        /// <summary>
        /// PUT api/Patient/Update/{id} - qualquer usuario autenticado (o id usado e o do DTO).
        /// Atualiza dados do acolhido, responsaveis, condicoes clinicas e medicamentos programados.
        /// Retorna ApiResponse ("Paciente nao encontrado." se nao existir).
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Updatepatient(PatientDTO patient)
        {
            var ret = await _patientApp.UpdatePatient(patient);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Patient/DeletePatient/{idPatient} - qualquer usuario autenticado.
        /// Exclui o acolhido da instituicao do usuario e seus dependentes (agendamentos, pagamentos,
        /// condicoes clinicas, responsaveis). Retorna ApiResponse ("Acolhido nao encontrado." se nao achar).
        /// </summary>
        [HttpDelete("DeletePatient/{idPatient}")]
        public async Task<IActionResult> DeletePatient(long idPatient)
        {
            var ret = await _patientApp.DeletePatient(idPatient);
            return Ok(ret);

            // ctrl + M + F = C M F = CADE MEU FILE
        }
        //[HttpGet("GetMedicinesToMinister")]
        //public async Task<IActionResult> GetMedicinesToMinister()
        //{
        //    var ret = await _patientApp.GetMedicinesToMinister();
        //    return Ok(ret);
        //}
    }
}
