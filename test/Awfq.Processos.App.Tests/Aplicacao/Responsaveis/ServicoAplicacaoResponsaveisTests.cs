using Xunit;
using FluentAssertions;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;
using Awfq.Processos.App.Aplicacao.Responsaveis;
using LanguageExt;
using System;
using Moq;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;

namespace Awfq.Processos.App.Tests.Aplicacao.Responsaveis
{
    public class ServicoAplicacaoResponsaveisTests
    {
        private const string CPF_VALIDO = "51666109037";

        [Fact]
        public void EmailObrigatorio()
        {
            // Arrange
            var repo = new Mock<IRepositorioResponsaveis>();
            var nome_incompleto = "Fulano de Tal";
            var cpf = CPF_VALIDO;
            var email = String.Empty;
            var servico = new ServicoAplicacaoResponsaveis(repo.Object);
            var comando = new ComandoCriaResponsavel(nome_incompleto, cpf, email);

            // Act
            var result = servico.CriaResponsavel(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(ValidacoesEntrada.EmailNaoInformado);
            });
        }

        [Fact]
        public void EmailDeveSerValido()
        {
            // Arrange
            var repo = new Mock<IRepositorioResponsaveis>();
            var nome_incompleto = "Fulano de Tal";
            var cpf = CPF_VALIDO;
            var email = "fulano_deTal@teste.com.br";
            var servico = new ServicoAplicacaoResponsaveis(repo.Object);
            var comando = new ComandoCriaResponsavel(nome_incompleto, cpf, email);

            // Act
            var result = servico.CriaResponsavel(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsRight);
            result.IfRight(e =>
            {
                e.Email.Should().Be(email);
            });
        }

        [Fact]
        public void EmailDeveTerNoMaximo400Caracteres()
        {
            // Arrange
            var repo = new Mock<IRepositorioResponsaveis>();
            var nome_incompleto = "Fulano de Tal";
            var cpf = CPF_VALIDO;
            var emailTamanhoMaximo = $"{StringTestsUtils.RandomString(350)}@{StringTestsUtils.RandomString(45)}.com";
            var emailTamanhoMaximoExcedido = $"{StringTestsUtils.RandomString(350)}@${StringTestsUtils.RandomString(45)}.com.br";
            var servico = new ServicoAplicacaoResponsaveis(repo.Object);
            var comandoValido = new ComandoCriaResponsavel(nome_incompleto, cpf, emailTamanhoMaximo);
            var comandoInvalido = new ComandoCriaResponsavel(nome_incompleto, cpf, emailTamanhoMaximoExcedido);

            // Act
            var resultadoComEmailValido = servico.CriaResponsavel(comandoValido);
            var resultadoComEmailInvalido = servico.CriaResponsavel(comandoInvalido);

            // Assert
            resultadoComEmailValido.State.Should().Be(EitherStatus.IsRight);
            resultadoComEmailValido.IfRight(e =>
            {
                e.Email.Should().Be(emailTamanhoMaximo);
            });
            resultadoComEmailInvalido.State.Should().Be(EitherStatus.IsLeft);
            resultadoComEmailInvalido.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(ValidacoesEntrada.EmailExcedeuLimiteMaximo);
            });
        }

        [Fact]
        public void CpfObrigatorio()
        {
            // Arrange
            var repo = new Mock<IRepositorioResponsaveis>();
            var nome_incompleto = "Fulano de Tal";
            var cpf = "";
            var email = "fulano_prestes_123@host.com";
            var servico = new ServicoAplicacaoResponsaveis(repo.Object);
            var comando = new ComandoCriaResponsavel(nome_incompleto, cpf, email);

            // Act
            var result = servico.CriaResponsavel(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(ValidacoesEntrada.CpfNaoInformado);
            });
        }

        [Fact]
        public void NomeObrigatorio()
        {
            // Arrange
            var repo = new Mock<IRepositorioResponsaveis>();
            var nome_incompleto = "Fulano";
            var cpf = "51666109037";
            var email = "fulano_prestes_123@host.com";
            var servico = new ServicoAplicacaoResponsaveis(repo.Object);
            var comando = new ComandoCriaResponsavel(nome_incompleto, cpf, email);

            // Act
            var result = servico.CriaResponsavel(comando);

            // Assert
            result.State.Should().Be(EitherStatus.IsLeft);
            result.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(ValidacoesEntrada.NomeCompletoNaoInformado);
            });
        }

        [Fact]
        public void NomeDeveTerNoMaximo150Caracteres()
        {
            var repo = new Mock<IRepositorioResponsaveis>();
            var nomeTamanhoMaximo = $"{StringTestsUtils.RandomString(50)} {StringTestsUtils.RandomString(99)}";
            var nomeTamanhoMaximoExcedido = $"{StringTestsUtils.RandomString(50)} {StringTestsUtils.RandomString(100)}";
            var cpf = "51666109037";
            var email = "fulano_prestes_123@host.com";
            var servico = new ServicoAplicacaoResponsaveis(repo.Object);
            var comandoNomeValido = new ComandoCriaResponsavel(nomeTamanhoMaximo, cpf, email);
            var comandoNomeInvalido = new ComandoCriaResponsavel(nomeTamanhoMaximoExcedido, cpf, email);

            // Act
            var resultadoComNomeValido = servico.CriaResponsavel(comandoNomeValido);
            var resultadoComNomeInvalido = servico.CriaResponsavel(comandoNomeInvalido);

            // Assert
            resultadoComNomeValido.State.Should().Be(EitherStatus.IsRight);
            resultadoComNomeInvalido.State.Should().Be(EitherStatus.IsLeft);
            resultadoComNomeValido.IfRight(e =>
            {
                e.Nome.Should().Be(nomeTamanhoMaximo);
            });
            resultadoComNomeInvalido.IfLeft(e =>
            {
                e.Should().NotBeEmpty().And.Contain(ValidacoesEntrada.NomeExcedeuLimiteMaximo);
            });
        }
    }
}