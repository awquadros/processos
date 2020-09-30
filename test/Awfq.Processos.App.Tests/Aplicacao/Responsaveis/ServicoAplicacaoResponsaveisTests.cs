using Xunit;
using FluentAssertions;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;
using Awfq.Processos.App.Aplicacao.Responsaveis;
using LanguageExt;
using System;
using Moq;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using Awfq.Comuns;
using Awfq.Processos.App.Utils;
using Microsoft.Extensions.Logging;

namespace Awfq.Processos.App.Tests.Aplicacao.Responsaveis
{
    public class ServicoAplicacaoResponsaveisTests
    {
        private const string CPF_VALIDO = "42795207044";

        public class ServicoComValidadoresSempreVerdadeiros
        {
            readonly ServicoAplicacaoResponsaveis servico;

            public ServicoComValidadoresSempreVerdadeiros()
            {
                var repo = new Mock<IRepositorioResponsaveis>();
                var removedor = new Mock<IRemovedorResponsavel>().Object;
                var editor = new Mock<IEditorResponsavel>().Object;
                var validadorEmail = new Mock<IValidadorEmail>();
                var validadorCpf = new Mock<IValidadorCpf>();
                var validadorRemocao = new Mock<IValidadorRemocaoResponsavel>();
                var logger = new Mock<ILogger>();

                repo
                    .Setup(o => o.Salva(It.IsAny<Responsavel>()))
                    .Returns((Responsavel a) => a);

                repo
                    .Setup(o => o.CpfJaCadastrado(It.IsAny<string>()))
                    .Returns(false);

                validadorEmail
                    .Setup(v => v.EmailValido(It.IsAny<string>()))
                    .Returns(true);

                validadorCpf
                    .Setup(v => v.CpfValido(It.IsAny<string>()))
                    .Returns(true);

                validadorRemocao
                    .Setup(v => v.FazParteDeProcesso(It.IsAny<Guid>()))
                    .Returns(false);

                this.servico = new ServicoAplicacaoResponsaveis(
                    repo.Object, removedor, editor, validadorEmail.Object, 
                    validadorCpf.Object, validadorRemocao.Object, logger.Object);
            }

            [Fact]
            public void CpfValido()
            {
                // Arrange
                var nome_incompleto = "Fulano de Tal";
                var cpf = "38078202059";
                var email = "fulano_prestes_123@host.com";
                var comando = new ComandoCriaResponsavel(nome_incompleto, cpf, email);

                // Act
                var result = servico.CriaResponsavel(comando);

                // Assert
                result.State.Should().Be(EitherStatus.IsRight);
            }

            [Fact]
            public void EmailObrigatorio()
            {
                // Arrange
                var nomeIncompleto = "Fulano de Tal";
                var cpf = CPF_VALIDO;
                var email = String.Empty;
                var comando = new ComandoCriaResponsavel(nomeIncompleto, cpf, email);

                // Act
                var result = servico.CriaResponsavel(comando);

                // Assert
                result.State.Should().Be(EitherStatus.IsLeft);
                result.IfLeft(e =>
                {
                    e.Should().NotBeEmpty().And.Contain(MensagensErros.EmailNaoInformado);
                });
            }

            [Fact]
            public void EmailDeveSuportar400Caracteres()
            {
                // Arrange
                var nome_incompleto = "Fulano de Tal";
                var cpf = CPF_VALIDO;
                var emailTamanhoMaximo = $"{StringTestsUtils.RandomString(350)}@{StringTestsUtils.RandomString(45)}.com";
                var comandoValido = new ComandoCriaResponsavel(nome_incompleto, cpf, emailTamanhoMaximo);

                // Act
                var resultadoComEmailValido = servico.CriaResponsavel(comandoValido);

                // Assert
                resultadoComEmailValido.State.Should().Be(EitherStatus.IsRight);
                resultadoComEmailValido.IfRight(e =>
                {
                    e.Email.Should().Be(emailTamanhoMaximo);
                });
            }

            [Fact]
            public void EmailDeveTerNoMaximo400Caracteres()
            {
                // Arrange
                var nome_incompleto = "Fulano de Tal";
                var cpf = CPF_VALIDO;
                var emailTamanhoMaximoExcedido = $"{StringTestsUtils.RandomString(350)}@${StringTestsUtils.RandomString(45)}.com.br";
                var comandoInvalido = new ComandoCriaResponsavel(nome_incompleto, cpf, emailTamanhoMaximoExcedido);

                // Act
                var resultadoComEmailInvalido = servico.CriaResponsavel(comandoInvalido);

                // Assert
                resultadoComEmailInvalido.State.Should().Be(EitherStatus.IsLeft);
                resultadoComEmailInvalido.IfLeft(e =>
                {
                    e.Should().NotBeEmpty().And.Contain(MensagensErros.EmailExcedeuLimiteMaximo);
                });
            }

            [Fact]
            public void CpfObrigatorio()
            {
                // Arrange
                var nome_incompleto = "Fulano de Tal";
                var cpf = "";
                var email = "fulano_prestes_123@host.com";
                var comando = new ComandoCriaResponsavel(nome_incompleto, cpf, email);

                // Act
                var result = servico.CriaResponsavel(comando);

                // Assert
                result.State.Should().Be(EitherStatus.IsLeft);
                result.IfLeft(e =>
                {
                    e.Should().NotBeEmpty().And.Contain(MensagensErros.CpfNaoInformado);
                });
            }

            [Fact]
            public void NomeObrigatorio()
            {
                // Arrange
                var nome_incompleto = "Fulano";
                var cpf = "51666109037";
                var email = "fulano_prestes_123@host.com";
                var comando = new ComandoCriaResponsavel(nome_incompleto, cpf, email);

                // Act
                var result = servico.CriaResponsavel(comando);

                // Assert
                result.State.Should().Be(EitherStatus.IsLeft);
                result.IfLeft(e =>
                {
                    e.Should().NotBeEmpty().And.Contain(MensagensErros.NomeCompletoNaoInformado);
                });
            }

            [Fact]
            public void NomeAceitarAte150Caracteres()
            {
                var nomeTamanhoMaximo = $"{StringTestsUtils.RandomString(50)} {StringTestsUtils.RandomString(99)}";
                var cpf = "51666109037";
                var email = "fulano_prestes_123@host.com";
                var comandoNomeValido = new ComandoCriaResponsavel(nomeTamanhoMaximo, cpf, email);

                // Act
                var resultadoComNomeValido = servico.CriaResponsavel(comandoNomeValido);

                // Assert
                resultadoComNomeValido.State.Should().Be(EitherStatus.IsRight);
                resultadoComNomeValido.IfRight(e =>
                {
                    e.Nome.Should().Be(nomeTamanhoMaximo);
                });
            }

            [Fact]
            public void NomeDeveTerNoMaximo150Caracteres()
            {
                var nomeTamanhoMaximoExcedido = $"{StringTestsUtils.RandomString(50)} {StringTestsUtils.RandomString(100)}";
                var cpf = "51666109037";
                var email = "fulano_prestes_123@host.com";
                var comandoNomeInvalido = new ComandoCriaResponsavel(nomeTamanhoMaximoExcedido, cpf, email);

                // Act
                var resultadoComNomeInvalido = servico.CriaResponsavel(comandoNomeInvalido);

                // Assert
                resultadoComNomeInvalido.State.Should().Be(EitherStatus.IsLeft);
                resultadoComNomeInvalido.IfLeft(e =>
                {
                    e.Should().NotBeEmpty().And.Contain(MensagensErros.NomeExcedeuLimiteMaximo);
                });
            }

        }

        public class ServicoComConfiguracao
        {
            private readonly Mock<IRepositorioResponsaveis> repo;
            private readonly Mock<IRemovedorResponsavel> removedor;
            private readonly Mock<IValidadorEmail> validadorEmail;
            private readonly Mock<IValidadorCpf> validadorCpf;
            private readonly Mock<IValidadorRemocaoResponsavel> validadorRemocao;
            private readonly ServicoAplicacaoResponsaveis servico;
            private readonly Mock<IEditorResponsavel> editor;
            private readonly Mock<ILogger> logger;

            public ServicoComConfiguracao()
            {
                this.repo = new Mock<IRepositorioResponsaveis>();
                this.removedor = new Mock<IRemovedorResponsavel>();
                this.editor = new Mock<IEditorResponsavel>();
                this.validadorEmail = new Mock<IValidadorEmail>();
                this.validadorCpf = new Mock<IValidadorCpf>();
                this.validadorRemocao = new Mock<IValidadorRemocaoResponsavel>();
                this.logger = new Mock<ILogger>();
                this.servico = new ServicoAplicacaoResponsaveis(
                    repo.Object, removedor.Object, editor.Object, validadorEmail.Object, 
                    validadorCpf.Object, validadorRemocao.Object, logger.Object);
            }

            [Fact]
            public void EmailDeveSerValido()
            {
                // Arrange
                var nome_incompleto = "Fulano de Tal";
                var cpf = CPF_VALIDO;
                var email = "fulano_deTal@teste.com.br";
                var comando = new ComandoCriaResponsavel(nome_incompleto, cpf, email);

                validadorEmail
                    .Setup(v => v.EmailValido(It.IsAny<string>()))
                    .Returns(true);

                validadorCpf
                    .Setup(v => v.CpfValido(It.IsAny<string>()))
                    .Returns(true);

                repo
                    .Setup(o => o.Salva(It.IsAny<Responsavel>()))
                    .Returns((Responsavel a) => a);

                repo
                    .Setup(o => o.CpfJaCadastrado(It.IsAny<string>()))
                    .Returns(false);

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
            public void CpfInvalido()
            {
                // Arrange
                var nome_incompleto = "Fulano de Tal";
                var cpf = "";
                var email = "fulano_prestes_123@host.com";
                var comando = new ComandoCriaResponsavel(nome_incompleto, cpf, email);

                this.validadorCpf
                    .Setup(v => v.CpfValido(It.IsAny<string>()))
                    .Returns(false);

                // Act
                var result = servico.CriaResponsavel(comando);

                // Assert
                result.State.Should().Be(EitherStatus.IsLeft);
                result.IfLeft(e =>
                {
                    e.Should().NotBeEmpty().And.Contain(MensagensErros.CpfNaoInformado);
                });
            }

            [Fact]
            public void Remocao_DeveFalharSeIdentificadorNaoExistir()
            {
                var nome = "dbb656e9-3452-44ac-b33c-4b15ccee9277";
                var comando = new ComandoRemoveResponsavel(nome);
                Responsavel responsavel = null;

                removedor
                    .Setup(o => o.Remove(It.Is<Guid>(x => x.ToString().Equals(nome))))
                    .Returns(responsavel);

                // Act
                var resultadoIdInexistente = servico.RemoveResponsavel(comando);

                // Assert
                resultadoIdInexistente.State.Should().Be(EitherStatus.IsLeft);
                resultadoIdInexistente.IfLeft(e =>
                {
                    e.Should().NotBeEmpty().And.Contain(MensagensErros.RecursoNaoEncontrado);
                });
            }
        }
    }
}