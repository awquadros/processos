using System;
using System.Collections.Generic;
using System.Linq;
using Awfq.Comuns;
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
        private readonly IRemovedorResponsavel removedor;
        private readonly IEditorResponsavel editor;
        //private readonly IEnumerable<PassoValidacao> pipelineValidacaoCriacao;
        //private readonly IEnumerable<PassoValidacao> pipelineValidacaoEdicao;

        delegate (IEnumerable<MensagensErros>, IComandoCriaEditaProcesso) PassoValidacao(
            (IEnumerable<MensagensErros> erros, IComandoCriaEditaProcesso cmd) fluxo);

        /// <sumary>
        /// Inicia uma nova isntância da classe <see cref="ServicoAplicacaoProcessos" />
        /// </sumary>
        public ServicoAplicacaoProcessos(
            ICriadorProcesso umCriador, IGeradorIdentificadorProcesso umGeradorIdentificador,
            IRemovedorResponsavel umRemovedor, IEditorResponsavel umEditor, ILogger umLogger)
        {
            this.criador = umCriador;
            this.geradorIdentificador = umGeradorIdentificador;
            this.removedor = umRemovedor;
            this.editor = umEditor;
            this.logger = umLogger;
            //this.pipelineValidacaoCriacao = new PassoValidacao[] { ValidaNome, ValidaCpf, ValidaEmail };
            //this.pipelineValidacaoEdicao = new PassoValidacao[] { ValidaNome, ValidaApenasValorCpf, ValidaEmail };
        }

        public Either<IEnumerable<MensagensErros>, ProcessoDTO> CriaProcesso(ComandoCriaProcesso cmd)
        {
            var result =
                ValidaCriacao(cmd)
                    .Match(ProcedeCriacao, CancelaAcao<ProcessoDTO>);

            return result;
        }

        private bool Nega(bool v) => !v;
        private bool NuloOuVazio(string str) => string.IsNullOrWhiteSpace(str);
        private bool NaoNulaOuVazia(string str) => compose<string, bool, bool>(NuloOuVazio, Nega)(str);
        private bool PalavrasMinimas(string str, int tamanho) => str.Split(' ').Count() >= tamanho;

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
            /*
            (IEnumerable<MensagensErros> erros, _) = ExecutaPipelineValidacao(this.pipelineValidacaoCriacao, cmd);

            return erros.Any()
                ? Left<IEnumerable<MensagensErros>, IComandoCriaEditaProcesso>(erros)
                : Right<IEnumerable<MensagensErros>, IComandoCriaEditaProcesso>(cmd);
*/
            return Right<IEnumerable<MensagensErros>, IComandoCriaEditaProcesso>(cmd); ;
        }


        private Either<IEnumerable<MensagensErros>, ProcessoDTO> ProcedeCriacao(IComandoCriaEditaProcesso cmd)
        {
            try
            {
                var id = this.geradorIdentificador.ObtemProximoId();
                var paiId = string.IsNullOrWhiteSpace(cmd.PaiId) ? Guid.Empty : new Guid(cmd.PaiId);
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
            };
        }
    }
}