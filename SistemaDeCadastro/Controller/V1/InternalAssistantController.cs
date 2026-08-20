using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;


namespace SistemaDeCadastro.Controller.V1;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InternalAssistantController : ControllerBase
{
    private readonly IInternalAssistantApp _internalAssistantApp;

    public InternalAssistantController(IInternalAssistantApp internalAssistantApp)
    {
        _internalAssistantApp = internalAssistantApp;
    }

    [HttpPost("Ask")]
    public async Task<IActionResult> Ask(AskInternalAssistantDTO askInternalAssistantDTO)
    {
        var ret = await _internalAssistantApp.Ask(askInternalAssistantDTO);
        return Ok(ret);
    }
}