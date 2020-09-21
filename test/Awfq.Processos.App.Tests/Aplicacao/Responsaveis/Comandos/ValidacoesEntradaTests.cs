using System;
using System.Collections.Generic;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Xunit;
using FluentAssertions;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;

namespace Awfq.Processos.App.Tests.Aplicacao.Responsaveis.Comandos
{
    public class SituacaoTests
    {
        [Fact]
        public void ValidacoesEntradaDevemTerIdsUnicos()
        {
            // Arrange
            var validacoes = ValidacoesEntrada.GetAll<ValidacoesEntrada>();

            // Assert
            validacoes
                .Should()
                .OnlyHaveUniqueItems(s => s.ValidacaoEntradaId);
        }
    }
}