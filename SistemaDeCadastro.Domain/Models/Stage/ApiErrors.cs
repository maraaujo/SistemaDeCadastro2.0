using System;
using Microsoft.Extensions.Logging;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    // Erros inesperados nao devem chegar ao cliente: a mensagem da excecao pode trazer
    // nomes de tabela, colunas e constraints do banco. O detalhe vai para o log do
    // servidor (coletado pelo Cloud Run) com um codigo; o cliente recebe uma mensagem
    // generica com o mesmo codigo, para o suporte achar o erro no log.
    public static class ApiErrors
    {
        public const string GenericMessage = "Não foi possível concluir a operação. Tente novamente mais tarde.";

        // Logger da aplicacao, configurado uma vez na inicializacao (Program.cs). Com
        // ele o erro sai como log estruturado, com nivel "Error" e a excecao completa.
        // Sem ele (ex.: testes) o erro continua indo para o stderr.
        public static ILogger? Logger { get; set; }

        /// <summary>
        /// Gera um codigo curto aleatorio (8 caracteres), registra a excecao completa no log (ILogger; stderr se nao houver logger) com esse
        /// codigo e devolve a mensagem generica para o usuario, ex.: "... (codigo AB12CD34)".
        /// Usado nos catch das Apps para preencher ApiResponse.ErrorMessage.
        /// </summary>
        public static string From(Exception ex)
        {
            var code = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

            if (Logger != null)
                Logger.LogError(ex, "Erro inesperado (código {ErrorCode})", code);
            else
                Console.Error.WriteLine($"[ERRO {code}] {DateTime.UtcNow:O} {ex}");

            return $"{GenericMessage} (código {code})";
        }
    }
}
