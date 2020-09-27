using System;
using System.Collections.Generic;
using System.Linq;
using Awfq.Processos.App.Aplicacao.Processos.Comandos;
using Awfq.Processos.App.Aplicacao.Processos.Dados;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
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
        private readonly IEditorResponsavel editor;
        private readonly IValidadorProcessoUnico validadorProcessoUnico;
        private readonly IEnumerable<PassoValidacao> pipelineValidacaoCriacao;
        //private readonly IEnumerable<PassoValidacao> pipelineValidacaoEdicao;

        delegate (IEnumerable<MensagensErros>, IComandoCriaEditaProcesso) PassoValidacao(
            (IEnumerable<MensagensErros> erros, IComandoCriaEditaProcesso cmd) fluxo);

        /// <sumary>
        /// Inicia uma nova isntância da classe <see cref="ServicoAplicacaoProcessos" />
        /// </sumary>
        public ServicoAplicacaoProcessos(
            IValidadorProcessoUnico umValidadorProcessoUnico,
            ICriadorProcesso umCriador, IGeradorIdentificadorProcesso umGeradorIdentificador,
            IRemovedorProcesso umRemovedor, IEditorResponsavel umEditor, ILogger umLogger)
        {
            this.validadorProcessoUnico = umValidadorProcessoUnico;
            this.criador = umCriador;
            this.geradorIdentificador = umGeradorIdentificador;
            this.removedor = umRemovedor;
            this.editor = umEditor;
            this.logger = umLogger;
            this.pipelineValidacaoCriacao = new PassoValidacao[] {
                ValidaResponsaveis, ValidaDescricao, ValidaPastaFisica, ValidaDataDistribuicao, ValidaProcessoUnificado };
            //this.pipelineValidacaoEdicao = new PassoValidacao[] { ValidaNome, ValidaApenasValorCpf, ValidaEmail };
        }

        public Either<IEnumerable<MensagensErros>, ProcessoDTO> CriaProcesso(ComandoCriaProcesso cmd)
        {
            var result =
                ValidaCriacao(cmd)
                    .Match(ProcedeCriacao, CancelaAcao<ProcessoDTO>);

            return result;
        }


        public Either<IEnumerable<MensagensErros>, ProcessoDTO> RemoveProcesso(ComandoRemoveProcesso cmd)
        {
            var result =
                ValidaRemocaoProcesso(cmd)
                    .Match(ProcedeRemocao, CancelaAcao<ProcessoDTO>);

            return result;
        }

        private bool Nega(bool v) => !v;
        private bool NuloOuVazio(string str) => string.IsNullOrWhiteSpace(str);
        private bool NaoNulaOuVazia(string str) => compose<string, bool, bool>(NuloOuVazio, Nega)(str);
        private bool PalavrasMinimas(string str, int tamanho) => str.Split(' ').Count() >= tamanho;

        private PassoValidacao ValidaProcessoUnificadoNaoInformado => f =>
            NuloOuVazio(f.cmd.ProcessoUnificado) ? (f.erros.Append(MensagensErros.NumeroProcessoUnificadoNaoInformado), f.cmd) : f;

        private PassoValidacao ValidaProcessoUnificadoLimite => f =>
            NaoNulaOuVazia(f.cmd.ProcessoUnificado) && f.cmd.ProcessoUnificado.Length != 20
            ? (f.erros.Append(MensagensErros.NumeroProcessoUnificadoMalFormatado), f.cmd) : f;

        private PassoValidacao TalvezValideNumeroPrcessoNoCadastro => f =>
            f.erros.Contains(MensagensErros.NumeroProcessoUnificadoNaoInformado)
            || f.erros.Contains(MensagensErros.NumeroProcessoUnificadoMalFormatado)
                ? f : ValidaNumeroProcessoJaCadastrado(f);

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
                var responsaveis = cmd.ResponsaveisIds?.Select(r => new Guid(r));
                var processo = new Processo(
                    id,
                    cmd.ProcessoUnificado,
                    cmd.DataDistribuicao,
                    cmd.SegredoJustica,
                    cmd.PastaFisicaCliente,
                    responsaveis,
                    cmd.SituacaoId,
                    cmd.Descricao,
                    paiId);

                var representacao = this.criador.Cria(processo);

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

            return erros.Any()
                ? Left<IEnumerable<MensagensErros>, ComandoRemoveProcesso>(erros)
                : Right<IEnumerable<MensagensErros>, ComandoRemoveProcesso>(cmd);
        }

        private Either<IEnumerable<MensagensErros>, ProcessoDTO> ProcedeRemocao(ComandoRemoveProcesso cmd)
        {
            try
            {
                var guid = new Guid(cmd.Id);
                var processo = this.removedor.Remove(guid);

                return processo == null
                        ? Left<IEnumerable<MensagensErros>, ProcessoDTO>(
                            new MensagensErros[] { MensagensErros.RecursoNaoEncontrado })
                        : Right<IEnumerable<MensagensErros>, ProcessoDTO>(CriarDesse(processo));
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is FormatException || ex is OverflowException)
            {
                this.logger.LogWarning(ex, ex.Message, cmd);
                return Left<IEnumerable<MensagensErros>, ProcessoDTO>(new MensagensErros[] { MensagensErros.IdentificadorMalFormatado });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message, cmd);
                return Left<IEnumerable<MensagensErros>, ProcessoDTO>(new MensagensErros[] { MensagensErros.RecursoNaoEncontrado });
            }
        }
    }
}