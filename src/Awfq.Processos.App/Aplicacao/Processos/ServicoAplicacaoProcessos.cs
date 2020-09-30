using System;
using System.Collections.Generic;
using System.Linq;
using Awfq.Processos.App.Aplicacao.Processos.Comandos;
using Awfq.Processos.App.Aplicacao.Processos.Dados;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using LanguageExt;
using Microsoft.Extensions.Logging;

using static LanguageExt.Prelude;

namespace Awfq.Processos.App.Aplicacao.Processos
{
    /// <sumary>
    /// Implementa um Serviço de Aplicação dedicada à manipular o Agregado <see cref="Processo"/>
    /// </sumary>
    public class ServicoAplicacaoProcessos : IServicoAplicacaoProcessos
    {
        private readonly ILogger logger;
        private readonly ICriadorProcesso criador;
        private readonly IGeradorIdentificadorProcesso geradorIdentificador;
        private readonly IRemovedorProcesso removedor;
        private readonly IEditorProcesso editor;
        private readonly IValidadorProcessoUnico validadorProcessoUnico;
        private readonly IValidadorProcessoPai validadorProcessoPai;
        private readonly IValidadorSituacaoRemocao validadorSituacaoRemocao;
        private readonly IObtentorResponsavel obtentorResponsavel;
        private readonly INotificadorResponsavel notificadorResponsavel;
        private readonly IObtendorProcessoPorId obtendorProcessoPorId;
        private readonly IEnumerable<PassoValidacao> pipelineValidacaoCriacao;
        private readonly IEnumerable<PassoValidacao> pipelineValidacaoEdicao;

        delegate (IEnumerable<MensagensErros>, IComandoCriaEditaProcesso) PassoValidacao(
            (IEnumerable<MensagensErros> erros, IComandoCriaEditaProcesso cmd) fluxo);

        /// <sumary>
        /// Inicia uma nova isntância da classe <see cref="ServicoAplicacaoProcessos" />
        /// </sumary>
        public ServicoAplicacaoProcessos(
            IValidadorProcessoUnico umValidadorProcessoUnico,
            IValidadorProcessoPai umValidadorProcessoPai,
            IValidadorSituacaoRemocao umValidadorSituacaoRemocao,
            INotificadorResponsavel umNotificadorResponsavel,
            ICriadorProcesso umCriador,
            IGeradorIdentificadorProcesso umGeradorIdentificador,
            IObtentorResponsavel umObtentorResponsavel,
            IRemovedorProcesso umRemovedor,
            IEditorProcesso umEditor,
            IObtendorProcessoPorId umObtendorProcessoPorId,
            ILogger umLogger)
        {
            this.validadorProcessoUnico = umValidadorProcessoUnico;
            this.validadorProcessoPai = umValidadorProcessoPai;
            this.validadorSituacaoRemocao = umValidadorSituacaoRemocao;
            this.notificadorResponsavel = umNotificadorResponsavel;
            this.criador = umCriador;
            this.geradorIdentificador = umGeradorIdentificador;
            this.obtentorResponsavel = umObtentorResponsavel;
            this.removedor = umRemovedor;
            this.editor = umEditor;
            this.obtendorProcessoPorId = umObtendorProcessoPorId;
            this.logger = umLogger;
            this.pipelineValidacaoCriacao = new PassoValidacao[] {
                ValidaSituacao, ValidaResponsaveis, ValidaDescricao, ValidaPastaFisica, ValidaDataDistribuicao, ValidaProcessoUnificado };
            this.pipelineValidacaoEdicao = new PassoValidacao[] {
                ValidaSituacao, ValidaResponsaveis, ValidaDescricao, ValidaPastaFisica, ValidaDataDistribuicao, ValidaProcessoUnificado };
        }

        public Either<IEnumerable<MensagensErros>, ProcessoDTO> CriaProcesso(ComandoCriaProcesso cmd) =>
            ValidaCriacao(cmd).Match(ProcedeCriacao, CancelaAcao<ProcessoDTO>);

        public Either<IEnumerable<MensagensErros>, ProcessoDTO> RemoveProcesso(ComandoRemoveProcesso cmd) =>
            ValidaRemocaoProcesso(cmd).Match(ProcedeRemocao, CancelaAcao<ProcessoDTO>);

        public Either<IEnumerable<MensagensErros>, ProcessoDTO> EditaProcesso(ComandoEditaProcesso cmd) =>
            ValidaEdicao(cmd)
                .Match<Either<IEnumerable<MensagensErros>, ComandoEditaProcesso>>(AdaptaComandoEdicao, CancelaAcao<ComandoEditaProcesso>)
                .Match(ProcedeEdicao, CancelaAcao<ProcessoDTO>);

        private bool Nega(bool v) => !v;
        private bool NuloOuVazio(string str) => string.IsNullOrWhiteSpace(str);
        private bool NaoNulaOuVazia(string str) => compose<string, bool, bool>(NuloOuVazio, Nega)(str);
        private bool PalavrasMinimas(string str, int tamanho) => str.Split(' ').Count() >= tamanho;

        private PassoValidacao ValidaProcessoUnificadoNaoInformado => f =>
            NuloOuVazio(f.cmd.ProcessoUnificado) ? (f.erros.Append(MensagensErros.NumeroProcessoUnificadoNaoInformado), f.cmd) : f;

        private PassoValidacao ValidaProcessoUnificadoLimite => f =>
            NaoNulaOuVazia(f.cmd.ProcessoUnificado) && f.cmd.ProcessoUnificado.Length != 20
            ? (f.erros.Append(MensagensErros.NumeroProcessoUnificadoMalFormatado), f.cmd) : f;

        private PassoValidacao TalvezValideNumeroPrcessoNoCadastro => f => f.cmd switch
        {
            ComandoCriaProcesso cmd =>
                f.erros.Contains(MensagensErros.NumeroProcessoUnificadoNaoInformado)
                || f.erros.Contains(MensagensErros.NumeroProcessoUnificadoMalFormatado)
                    ? f : ValidaNumeroProcessoJaCadastrado(f),
            _ => f
        };

        private PassoValidacao ValidaNumeroProcessoJaCadastrado => f =>
            this.validadorProcessoUnico.ProcessoJaCadastrado(f.cmd.ProcessoUnificado)
            ? (f.erros.Append(MensagensErros.NumeroProcessoUnificadoDuplicado), f.cmd) : f;

        private PassoValidacao ValidaPaiNoMesmoNivel => f =>
            this.validadorProcessoUnico.ProcessoJaCadastrado(f.cmd.ProcessoUnificado)
            ? (f.erros.Append(MensagensErros.ProcessoNaMesmoNívelHierarquico), f.cmd) : f;

        private PassoValidacao ValidaProcessoUnificado => f =>
            TalvezValideNumeroPrcessoNoCadastro(ValidaProcessoUnificadoLimite(ValidaProcessoUnificadoNaoInformado(f)));

        private PassoValidacao ValidaDataDistribuicao => f =>
            f.cmd.DataDistribuicao != null && f.cmd.DataDistribuicao.Value.Date > DateTime.Today
            ? (f.erros.Append(MensagensErros.DataDistribuicaoInvalida), f.cmd) : f;

        private PassoValidacao ValidaPastaFisica => f =>
            NaoNulaOuVazia(f.cmd.PastaFisicaCliente) && f.cmd.PastaFisicaCliente.Length > 50
            ? (f.erros.Append(MensagensErros.PastaFisicaExcedeuLimite), f.cmd) : f;

        private PassoValidacao ValidaDescricao => f =>
            NaoNulaOuVazia(f.cmd.Descricao) && f.cmd.Descricao.Length > 1000
            ? (f.erros.Append(MensagensErros.DescricaoExcedeuLimiteMaximo), f.cmd) : f;

        private PassoValidacao ValidaResponsavelNaoInformado => f =>
            f.cmd.ResponsaveisIds.Length() == 0 ? (f.erros.Append(MensagensErros.ResponsavelNaoInformado), f.cmd) : f;

        private PassoValidacao ValidaResponsavelExcedeuLimite => f =>
            f.cmd.ResponsaveisIds.Length() > 3 ? (f.erros.Append(MensagensErros.NumeroResponsaveisExcedeuLimite), f.cmd) : f;

        private PassoValidacao ValidaResponsavelDuplicado => f =>
            f.cmd.ResponsaveisIds.Count() != f.cmd.ResponsaveisIds.Distinct().Count()
                ? (f.erros.Append(MensagensErros.ResponsavelDuplicado), f.cmd) : f;

        private PassoValidacao ValidaSituacaoEdicao => f => f.cmd switch
        {
            ComandoEditaProcesso cmd => this.validadorSituacaoRemocao.JaFinalizado(new Guid(((ComandoEditaProcesso)cmd).Id))
                ? (f.erros.Append(MensagensErros.ProcessoJaFinalizado), f.cmd) : f,
            _ => f
        };

        private PassoValidacao ValidaSituacaoInexistente => f =>
            Situacao.GetAll<Situacao>().Find(x => x.SituacaoId == f.cmd.SituacaoId).Any()
                ? f : (f.erros.Append(MensagensErros.SituacaoInvalida), f.cmd);

        private PassoValidacao ValidaSituacao => f => ValidaSituacaoEdicao(ValidaSituacaoInexistente(f));

        private PassoValidacao ValidaResponsaveis => f =>
            ValidaResponsavelDuplicado(ValidaResponsavelExcedeuLimite(ValidaResponsavelNaoInformado(f)));

        private (IEnumerable<MensagensErros>, IComandoCriaEditaProcesso) ExecutaPipelineValidacao(
            IEnumerable<PassoValidacao> p, IComandoCriaEditaProcesso cmd)
        {
            IEnumerable<MensagensErros> acc = new List<MensagensErros>();
            return p.Aggregate((acc, cmd), (acc, passo) =>
            {
                (IEnumerable<MensagensErros> e, IComandoCriaEditaProcesso) r = passo(acc); return r;
            });
        }

        private Either<IEnumerable<MensagensErros>, IComandoCriaEditaProcesso> ValidaCriacao(
            in IComandoCriaEditaProcesso cmd)
        {
            (IEnumerable<MensagensErros> erros, _) = ExecutaPipelineValidacao(this.pipelineValidacaoCriacao, cmd);

            return erros.Any()
                ? Left<IEnumerable<MensagensErros>, IComandoCriaEditaProcesso>(erros)
                : Right<IEnumerable<MensagensErros>, IComandoCriaEditaProcesso>(cmd);
        }

        private Either<IEnumerable<MensagensErros>, IComandoCriaEditaProcesso> ValidaEdicao(
            in IComandoCriaEditaProcesso cmd)
        {
            (IEnumerable<MensagensErros> erros, _) = ExecutaPipelineValidacao(this.pipelineValidacaoEdicao, cmd);

            return erros.Any()
                ? Left<IEnumerable<MensagensErros>, IComandoCriaEditaProcesso>(erros)
                : Right<IEnumerable<MensagensErros>, IComandoCriaEditaProcesso>(cmd);
        }

        private Guid? toNullableGuid(string possivelGuid)
        {
            if (string.IsNullOrWhiteSpace(possivelGuid))
                return null;

            return new Guid(possivelGuid);
        }

        private Either<IEnumerable<MensagensErros>, ProcessoDTO> ProcedeCriacao(IComandoCriaEditaProcesso cmd)
        {
            try
            {
                var id = this.geradorIdentificador.ObtemProximoId();
                var paiId = toNullableGuid(cmd.PaiId);
                var responsaveisIds = cmd.ResponsaveisIds?.Select(r => new Guid(r));
                var processo = new Processo(
                    id,
                    cmd.ProcessoUnificado,
                    cmd.DataDistribuicao,
                    cmd.SegredoJustica,
                    cmd.PastaFisicaCliente,
                    responsaveisIds,
                    cmd.SituacaoId,
                    cmd.Descricao,
                    paiId);

                var responsaveis = this.obtentorResponsavel
                    .ObtemResponsaveis(responsaveisIds.ToArray())
                    .Select(x => (x.Nome, x.Email));
                var representacao = this.criador.Cria(processo);

                var notificacao = new NotificacaoResponsavel(
                    "Notificação de Processo",
                    $"Você foi cadastrado como envolvido no processo de número {cmd.ProcessoUnificado}."
                );

                this.notificadorResponsavel.NotificarAsync(notificacao, responsaveis);

                return Right<IEnumerable<MensagensErros>, ProcessoDTO>(CriarDesse(representacao));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException)
            {
                this.logger.LogError(ex, ex.Message, cmd);
                return Left<IEnumerable<MensagensErros>, ProcessoDTO>(new MensagensErros[] { MensagensErros.IdentificadorMalFormatado });
            }
            catch (System.Exception ex)
            {
                this.logger.LogError(ex, ex.Message, cmd);
                return Left<IEnumerable<MensagensErros>, ProcessoDTO>(new MensagensErros[] { MensagensErros.ErroNaoEsperado });
            }
        }

        private Either<IEnumerable<MensagensErros>, ProcessoDTO> ProcedeEdicao(ComandoEditaProcesso cmd)
        {
            try
            {
                var guid = new Guid(cmd.Id);
                var paiId = toNullableGuid(cmd.PaiId);
                var responsaveisIds = cmd.ResponsaveisIds?.Select(r => new Guid(r));
                var processoEditado = new Processo(
                    guid,
                    cmd.ProcessoUnificado,
                    cmd.DataDistribuicao,
                    cmd.SegredoJustica,
                    cmd.PastaFisicaCliente,
                    responsaveisIds,
                    cmd.SituacaoId,
                    cmd.Descricao,
                    paiId);

                var processo = this.obtendorProcessoPorId.ObtemProcessoPorId(guid);

                if (processo == null) 
                    return Left<IEnumerable<MensagensErros>, ProcessoDTO>(
                        new MensagensErros[] { MensagensErros.RecursoNaoEncontrado });

                var novosResponsaveis = responsaveisIds.Except(processo.ResponsaveisIds).ToArray();
                var haNovosResponsaveis = novosResponsaveis.Length > 0;

                var responsaveis = this.obtentorResponsavel
                    .ObtemResponsaveis(novosResponsaveis)
                    .Select(x => (x.Nome, x.Email));

                var representacao = this.editor.Edita(processoEditado);

                if (haNovosResponsaveis)
                {
                    var notificacao = new NotificacaoResponsavel(
                        "Notificação de Processo",
                        $"Você foi adicionado como envolvido no processo de número {cmd.ProcessoUnificado}."
                    );

                    this.notificadorResponsavel.NotificarAsync(notificacao, responsaveis);
                }

                return Right<IEnumerable<MensagensErros>, ProcessoDTO>(CriarDesse(representacao));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is OverflowException)
            {
                this.logger.LogError(ex, ex.Message, cmd);
                return Left<IEnumerable<MensagensErros>, ProcessoDTO>(new MensagensErros[] { MensagensErros.IdentificadorMalFormatado });
            }
            catch (System.Exception ex)
            {
                this.logger.LogError(ex, ex.Message, cmd);
                return Left<IEnumerable<MensagensErros>, ProcessoDTO>(new MensagensErros[] { MensagensErros.ErroNaoEsperado });
            }
        }

        private Either<IEnumerable<MensagensErros>, ComandoEditaProcesso> AdaptaComandoEdicao(IComandoCriaEditaProcesso cmd) => cmd switch
        {
            ComandoEditaProcesso ced => Right(ced),
            _ => Left((IEnumerable<MensagensErros>)new MensagensErros[] { MensagensErros.ErroNaoEsperado })
        };

        private Either<IEnumerable<MensagensErros>, TRight> CancelaAcao<TRight>(IEnumerable<MensagensErros> erros)
            => Left<IEnumerable<MensagensErros>, TRight>(erros);

        private ProcessoDTO CriarDesse(Processo processo)
        {
            return new ProcessoDTO()
            {
                Id = processo.Id,
                PaiId = processo.PaiId,
                DataDistribuicao = processo.DataDistribuicao,
                Descricao = processo.Descricao,
                PastaFisicaCliente = processo.PastaFisicaCliente,
                ProcessoUnificado = processo.ProcessoUnificado,
                SegredoJustica = processo.SegredoJustica,
                Responsaveis = processo.ResponsaveisIds,
                SituacaoId = processo.SituacaoId
            };
        }

        private Either<IEnumerable<MensagensErros>, ComandoRemoveProcesso> ValidaRemocaoProcesso(ComandoRemoveProcesso cmd)
        {
            var erros = new List<MensagensErros>();

            if (cmd == null)
                erros.Add(MensagensErros.ComandoInvalido);

            if (!Guid.TryParse(cmd.Id, out Guid id))
                erros.Add(MensagensErros.IdentificadorUnicoInvalido);

            if (id != Guid.Empty && this.validadorProcessoPai.ProcessoEhPai(id))
                erros.Add(MensagensErros.RemocaoDeProcessoPaiNaoPermitida);

            if (id != Guid.Empty && this.validadorSituacaoRemocao.JaFinalizado(id))
                erros.Add(MensagensErros.ProcessoJaFinalizado);

            return erros.Any()
                ? Left<IEnumerable<MensagensErros>, ComandoRemoveProcesso>(erros)
                : Right<IEnumerable<MensagensErros>, ComandoRemoveProcesso>(cmd);
        }

        private Either<IEnumerable<MensagensErros>, ProcessoDTO> ProcedeRemocao(ComandoRemoveProcesso cmd)
        {
            try
            {
                var guid = new Guid(cmd.Id);
                var processoRemovido = this.removedor.Remove(guid);

                if (processoRemovido != null)
                {
                    var responsaveis = this.obtentorResponsavel
                        .ObtemResponsaveis(processoRemovido.ResponsaveisIds.ToArray())
                        .Select(x => (x.Nome, x.Email));

                    var notificacao = new NotificacaoResponsavel(
                        "Notificação de Processo",
                        $"Você foi adicionado como envolvido no processo de número {processoRemovido.ProcessoUnificado}."
                    );

                    this.notificadorResponsavel.NotificarAsync(notificacao, responsaveis);

                    return Right<IEnumerable<MensagensErros>, ProcessoDTO>(CriarDesse(processoRemovido));
                } else 
                {
                    return Left<IEnumerable<MensagensErros>, ProcessoDTO>(
                            new MensagensErros[] { MensagensErros.RecursoNaoEncontrado });
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message, cmd);
                return Left<IEnumerable<MensagensErros>, ProcessoDTO>(new MensagensErros[] { MensagensErros.ErroNaoEsperado });
            }
        }
    }
}