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

    /// <summary>
    /// Recebe a camada de aplicacao do assistente interno por injecao.
    /// </summary>
    public InternalAssistantController(IInternalAssistantApp internalAssistantApp)
    {
        _internalAssistantApp = internalAssistantApp;
    }

    /// <summary>
    /// POST api/InternalAssistant/Ask - qualquer usuario autenticado.
    /// Envia uma pergunta ao assistente interno (baseado em regras, sem IA externa), que responde
    /// sobre medicamentos atrasados, medicamentos de um acolhido, administracoes de hoje ou resumo do dia.
    /// Retorna ApiResponse com a resposta.
    /// </summary>
    [HttpPost("Ask")]
    public async Task<IActionResult> Ask(AskInternalAssistantDTO askInternalAssistantDTO)
    {
        var ret = await _internalAssistantApp.Ask(askInternalAssistantDTO);
        return Ok(ret);
    }
}