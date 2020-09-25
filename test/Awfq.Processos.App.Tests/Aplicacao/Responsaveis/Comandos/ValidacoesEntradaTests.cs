using Xunit;
using FluentAssertions;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;

namespace Awfq.Processos.App.Tests.Aplicacao.Responsaveis.Comandos
{
    public class SituacaoTests
    {
        [Fact]
        public void MensagensErrosDevemTerIdsUnicos()
        {
            // Arrange
            var validacoes = MensagensErros.GetAll<MensagensErros>();

            // Assert
            validacoes
                .Should()
                .OnlyHaveUniqueItems(s => s.MensagemId);
        }
    }
}