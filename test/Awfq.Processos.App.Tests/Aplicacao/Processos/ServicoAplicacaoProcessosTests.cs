using Xunit;
using FluentAssertions;
using LanguageExt;
using System;
using Moq;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using Awfq.Comuns;
using Awfq.Processos.App.Utils;
using Awfq.Processos.App.Aplicacao.Processos;
using Microsoft.Extensions.Logging;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Aplicacao.Processos.Comandos;

namespace Awfq.Processos.App.Tests.Aplicacao.Responsaveis
{
    public class ServicoAplicacaoProcessosTests
    {
        private const string ProcessoUnificadoValidoFormatado = "3513038-00.2016.8.23.0023";
        private const string ProcessoUnificadoValido = "35130380020168230023";
        private const string PastaFisicaCliente = "Pas0014";
        private readonly Mock<ILogger> logger;
        private readonly Mock<ICriadorProcesso> criador;
        private readonly Mock<IGeradorIdentificadorProcesso> geradorIdentificador;
        private readonly Mock<IRemovedorProcesso> removedor;
        private readonly Mock<IEditorResponsavel> editor;
        private readonly Mock<IValidadorProcessoUnico> validadorProcessoUnico;
        private readonly IServicoAplicacaoProcessos servico;

        public ServicoAplicacaoProcessosTests()
        {
            this.logger = new Mock<ILogger>();
            this.criador = new Mock<ICriadorProcesso>();
            this.geradorIdentificador = new Mock<IGeradorIdentificadorProcesso>();
            this.removedor = new Mock<IRemovedorProcesso>();
            this.editor = new Mock<IEditorResponsavel>();
            this.validadorProcessoUnico = new Mock<IValidadorProcessoUnico>();

            this.servico = new ServicoAplicacaoProcessos(
                validadorProcessoUnico.Object,
                criador.Object,
                geradorIdentificador.Object,
                removedor.Object,
                editor.Object,
                logger.Object);
        }

        [Fact]
        public void NumeroProcessoUnificadoObrigatorio()
        {
            // Arrange
            var processoUnificadoInvalido = string.Empty;
            var dataDistribuicao = DateTime.Now;
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = string.Empty;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                processoUnificadoInvalido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(MensagensErros.NumeroProcessoUnificadoNaoInformado);
            });
        }

        [Theory]
        [InlineData("3")]
        [InlineData("3513038")]
        [InlineData("3513038-00.2016")]
        [InlineData("3513038-00.2016.8.23.0")]
        [InlineData("3513038-00.2016.8.23.00231")]
        public void NumeroProcessoUnificadoDeveTer20Caracteres(string processoUnifiicado)
        {
            // Arrange
            var dataDistribuicao = DateTime.Now;
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = string.Empty;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                processoUnifiicado,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(MensagensErros.NumeroProcessoUnificadoMalFormatado);
            });
        }

        [Fact]
        public void NumeroProcessoUnificadoDeveAceitar20Caracteres()
        {
            // Arrange
            var dataDistribuicao = DateTime.Now;
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = string.Empty;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsRight);
        }

        [Fact]
        public void NumeroProcessoUnificadoDevePodeEstarFormatado()
        {
            // Arrange
            var dataDistribuicao = DateTime.Now;
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = string.Empty;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValidoFormatado,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsRight);
        }

        [Fact]
        public void DataDistribuicaoInformadaNaoDeveSerMaiorDataAtual()
        {
            // Arrange
            var dataAtual = DateTime.Now;
            var dataDistribuicao = dataAtual.AddDays(1);
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = string.Empty;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(MensagensErros.DataDistribuicaoInvalida);
            });
        }

        [Fact]
        public void DataDistribuicaoInformadaPodeSerMenorDataAtual()
        {
            // Arrange
            var dataAtual = DateTime.Now;
            var dataDistribuicao = dataAtual.AddDays(-1);
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = string.Empty;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsRight);
        }

        [Fact]
        public void DataDistribuicaoInformadaPodeSerIgualDataAtual()
        {
            // Arrange
            var dataDistribuicao = DateTime.Today;
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = string.Empty;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsRight);
        }

        [Fact]
        public void PastaFisicaNaoDeveExceder50Caracteres()
        {
            // Arrange
            var dataDistribuicao = DateTime.Today;
            var pastaFisicaCliente = StringTestsUtils.RandomString(51);
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = string.Empty;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                pastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(MensagensErros.PastaFisicaExcedeuLimite);
            });
        }

        [Fact]
        public void PastaFisicaPodeTer50Caracteres()
        {
            // Arrange
            var dataDistribuicao = DateTime.Today;
            var pastaFisicaCliente = StringTestsUtils.RandomString(50);
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = string.Empty;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                pastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsRight);
        }

        [Fact]
        public void DescricaoNaoDeveExceder1000Caracteres()
        {
            // Arrange
            var dataDistribuicao = DateTime.Today;
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = StringTestsUtils.RandomString(1001); ;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(MensagensErros.DescricaoExcedeuLimiteMaximo);
            });
        }

        [Fact]
        public void DescricaoPodeTer1000Caracteres()
        {
            // Arrange
            var dataDistribuicao = DateTime.Today;
            var segredoJustica = false;
            var responsaveisIds = new string[] { "" };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = StringTestsUtils.RandomString(1000); ;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsRight);
        }

        [Fact]
        public void DeveHaverNoMinimoUmResponsavel()
        {
            // Arrange
            var dataDistribuicao = DateTime.Today;
            var segredoJustica = false;
            var responsaveisIds = new string[] { };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = StringTestsUtils.RandomString(1000); ;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(MensagensErros.ResponsavelNaoInformado);
            });
        }

        [Fact]
        public void DeveHaverNoMaximoTresResponsaveis()
        {
            // Arrange
            var dataDistribuicao = DateTime.Today;
            var segredoJustica = false;
            var responsaveisIds = new string[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = StringTestsUtils.RandomString(1000); ;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(MensagensErros.NumeroResponsaveisExcedeuLimite);
            });
        }

        [Fact]
        public void NaoDeveHaverResponsaveisDuplicados()
        {
            // Arrange
            var responsavelId = Guid.NewGuid().ToString();
            var dataDistribuicao = DateTime.Today;
            var segredoJustica = false;
            var responsaveisIds = new string[]
            {
                responsavelId,
                responsavelId
            };
            var situacaoId = Situacao.EmAndamento.SituacaoId;
            var descricao = StringTestsUtils.RandomString(1000); ;
            var paiId = string.Empty;
            var comando = new ComandoCriaProcesso(
                ProcessoUnificadoValido,
                dataDistribuicao,
                segredoJustica,
                PastaFisicaCliente,
                responsaveisIds,
                situacaoId,
                descricao,
                paiId);

            // Act
            var result = this.servico.CriaProcesso(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(MensagensErros.ResponsavelDuplicado);
            });
        }
    }
}