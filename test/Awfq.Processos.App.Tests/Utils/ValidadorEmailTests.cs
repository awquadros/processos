using System;
using Xunit;
using FluentAssertions;
using Awfq.Processos.App.Utils;

namespace Awfq.Processos.App.Tests
{
    public class ValidadorEmailTests
    {
        [Theory]
        [InlineData("fulano_teste@")]
        [InlineData("fulano_teste.com.br")]
        [InlineData("fulano_teste@.com")]
        [InlineData("@.com")]
        [InlineData("fulano_@_@.com")]
        public void EmailDeveSerInvalido(String input)
        {
            // Arrange
            var resultado = ValidadorEmail.IsValidEmail(input);

            // Assert
            resultado
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("fulano_teste@host.com")]
        [InlineData("fulano_teste.@host.com.br")]
        [InlineData("ASDeadaEAd@google.com")]
        [InlineData("fulano.teste123@a.com")]
        public void EmailDeveSerValido(String input)
        {
            // Arrange
            var resultado = ValidadorEmail.IsValidEmail(input);

            // Assert
            resultado
                .Should()
                .BeTrue();
        }
    }
}