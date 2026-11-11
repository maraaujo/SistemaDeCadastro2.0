using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IInternalAssistantApp
    {
        /// <summary>Responde a pergunta do assistente interno (atrasados, medicamentos do acolhido, administracoes de hoje, resumo do dia).</summary>
        Task<ApiResponse> Ask(AskInternalAssistantDTO dto);
    }
}
