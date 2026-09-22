using SistemaDeCadastro.Domain.Validators;
using Xunit;

namespace SistemadeCadastro.Test
{
    public class ValidatorsTests
    {
        // ---------------------------------------------------------------
        // CPF
        // ---------------------------------------------------------------

        [Theory]
        [InlineData("111.444.777-35")]   // com máscara
        [InlineData("11144477735")]      // sem máscara
        public void Cpf_Valido_DeveAceitarComOuSemMascara(string cpf)
        {
            Assert.True(CpfValidator.IsValid(cpf));
        }

        [Theory]
        [InlineData("11111111111")]      // todos iguais
        [InlineData("00000000000")]      // todos iguais
        [InlineData("111.444.777-00")]   // dígitos verificadores errados
        [InlineData("123456789")]        // menos de 11 dígitos
        [InlineData("123456789012")]     // mais de 11 dígitos
        [InlineData("")]                 // vazio
        [InlineData(null)]               // nulo
        public void Cpf_Invalido_DeveRejeitar(string? cpf)
        {
            Assert.False(CpfValidator.IsValid(cpf));
        }

        [Fact]
        public void Cpf_Normalize_RemoveMascara()
        {
            Assert.Equal("11144477735", CpfValidator.Normalize("111.444.777-35"));
        }

        // ---------------------------------------------------------------
        // CEP
        // ---------------------------------------------------------------

        [Theory]
        [InlineData("70000-000")]        // com máscara (não zerado)
        [InlineData("70000000")]         // sem máscara
        public void Cep_Valido_DeveAceitarComOuSemMascara(string cep)
        {
            Assert.True(CepValidator.IsValid(cep));
        }

        [Theory]
        [InlineData("0000000")]          // 7 dígitos
        [InlineData("700000000")]        // 9 dígitos
        [InlineData("00000000")]         // todos zeros
        [InlineData("abc")]              // não numérico
        [InlineData(null)]
        public void Cep_Invalido_DeveRejeitar(string? cep)
        {
            Assert.False(CepValidator.IsValid(cep));
        }

        // ---------------------------------------------------------------
        // Telefone
        // ---------------------------------------------------------------

        [Theory]
        [InlineData("(61) 99999-9999")]  // celular com máscara
        [InlineData("61999999999")]      // celular sem máscara
        [InlineData("(61) 3333-3333")]   // fixo com máscara
        [InlineData("6133333333")]       // fixo sem máscara
        public void Telefone_Valido_DeveAceitarFixoECelular(string phone)
        {
            Assert.True(PhoneValidator.IsValid(phone));
        }

        [Theory]
        [InlineData("(61) 89999-9999")]  // celular deve começar com 9 após o DDD
        [InlineData("6199999999")]       // 10 dígitos mas começa com 9 (não é fixo válido)
        [InlineData("00999999999")]      // DDD inválido
        [InlineData("619999")]           // curto demais
        [InlineData("619999999999")]     // longo demais
        [InlineData(null)]
        public void Telefone_Invalido_DeveRejeitar(string? phone)
        {
            Assert.False(PhoneValidator.IsValid(phone));
        }

        [Fact]
        public void Telefone_Normalize_RemoveMascara()
        {
            Assert.Equal("61999999999", PhoneValidator.Normalize("(61) 99999-9999"));
        }
    }
}
