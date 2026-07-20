using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController : ControllerBase
    {
        private readonly IPlanApp _app;

        public PlanController(IPlanApp app)
        {
            _app = app;
        }

        [HttpGet("GetPlanById")]
        public async Task<IActionResult> GetPlanById(long id)
        {
            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        [HttpGet("GetAllPlans")]
        public async Task<IActionResult> GetAllPlans()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        [HttpPost("CreatePlan")]
        public async Task<IActionResult> CreatePlan(CreatePlanDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdatePlan")]
        public async Task<IActionResult> UpdatePlan(UpdatePlanDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeletePlan/{idPlan}")]
        public async Task<IActionResult> DeletePlan(long idPlan)
        {
            var ret = await _app.Delete(idPlan);
            return Ok(ret);
        }
    }
}
