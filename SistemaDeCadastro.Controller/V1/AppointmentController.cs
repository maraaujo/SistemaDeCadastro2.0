using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IAppointmentApp _app;

        public AppointmentController(IConfiguration configuration, IAppointmentApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetAppointmentById")]
        public async Task<IActionResult> GetAppointmentById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllAppointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreateAppointment")]
        public async Task<IActionResult> CreateAppointment(Appointment entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateAppointment")]
        public async Task<IActionResult> UpdateAppointment(Appointment entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteAppointment/{idAppointment}")]
        public async Task<IActionResult> DeleteAppointment(long idAppointment)
        {
            var ret = await _app.Delete(idAppointment);
            return Ok(ret);
        }
    }
}
