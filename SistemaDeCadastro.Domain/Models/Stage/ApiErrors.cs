using System;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    // Erros inesperados nao devem chegar ao cliente: a mensagem da excecao pode trazer
    // nomes de tabela, colunas e constraints do banco. O detalhe vai para o log do
    // servidor (stderr, coletado pelo Cloud Run) com um codigo; o cliente recebe uma
    // mensagem generica com o mesmo codigo, para o suporte achar o erro no log.
    public static class ApiErrors
    {
        public const string GenericMessage = "Não foi possível concluir a operação. Tente novamente mais tarde.";

        public static string From(Exception ex)
        {
            var code = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            Console.Error.WriteLine($"[ERRO {code}] {DateTime.UtcNow:O} {ex}");
            return $"{GenericMessage} (código {code})";
        }
    }
}
