using System;
using System.Collections.Generic;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Xunit;
using FluentAssertions;

namespace Awfq.Processos.App.Tests.Dominio.Modelo.Processos
{
    public class SituacaoTests
    {
        [Fact]
        public void SituacoesNaoFinalizadas()
        {
            // Arrange
            var situacoes = new List<Situacao>()
            {
                Situacao.EmAndamento,
                Situacao.Desmembrado,
                Situacao.EmRecurso
            };

            // Assert
            situacoes
                .Should()
                .OnlyContain(s => s.EstaFinalizado.Equals("Não"))
                .And.OnlyHaveUniqueItems(s => s.SituacaoId);
        }

        [Fact]
        public void SituacoesFinalizadas()
        {
            // Arrange
            var situacoes = new List<Situacao>()
            {
                Situacao.Finalizado,
                Situacao.Arquivado
            };

            // Assert
            situacoes
                .Should()
                .OnlyContain(s => s.EstaFinalizado.Equals("Sim"))
                .And.OnlyHaveUniqueItems(s => s.SituacaoId);
        }
    }
}