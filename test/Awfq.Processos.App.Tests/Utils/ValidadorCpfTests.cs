using System;
using Xunit;
using FluentAssertions;
using Awfq.Processos.App.Utils;

namespace Awfq.Processos.App.Tests
{
    public class ValidadorCpfTests
    {
        [Theory]
        [InlineData("771.617.070-99")]
        [InlineData("717.509.550-11")]
        [InlineData("22899985099")]
        [InlineData("BB8999850AA")]
        [InlineData("ASFRRREADF")]
        [InlineData("")]
        [InlineData(null)]
        public void CpfSerInvalido(String input)
        {
            // Arrange
            var validador = new ValidadorCpf();
            var resultado = validador.CpfValido(input);

            // Assert
            resultado
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("86702824025")]
        [InlineData("11991487096")]
        [InlineData("900.610.570-88")]
        [InlineData("723.093.160-67")]
        [InlineData("38078202059")]
        public void CpfDeveSerValido(String input)
        {
            // Arrange
            var validador = new ValidadorCpf();
            var resultado = validador.CpfValido(input);

            // Assert
            resultado
                .Should()
                .BeTrue();
        }
    }
}