using System;
using System.Linq;
using System.Collections.Generic;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Aplicacao.Processos.Dados;
using System.Threading.Tasks;
using LanguageExt;
using Awfq.Processos.App.Aplicacao.Processos.Comandos;

using static LanguageExt.Prelude;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB.Abstracoes;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Awfq.Processos.App.Aplicacao.Processos
{
    /// <sumary>
    /// Implementa um Serviço de Consulta para o Aggregado Processo
    /// </sumary>
    public class ServicoConsultaProcessos : IServicoConsultaProcessos
    {
        private readonly IContextoPersistencia contextoPersistencia;
        private readonly ILogger logger;

        /// <sumary>
        /// Inicia uma nova instância da classe <see cref"ServicoConsultaProcessos" />
        /// </sumary>
        /// <param name="umContextoPersistencia">um implementação da interface <see cref="IContextoPersistencia"/></param>
        /// <param name="umLogger">um implementação da interface <see cref="ILogger"/></param>
        public ServicoConsultaProcessos(IContextoPersistencia umContextoPersistencia, ILogger umLogger)
        {
            this.contextoPersistencia = umContextoPersistencia;
            this.logger = umLogger;
        }

        /// <sumary>
        /// Obtém Situações
        /// </sumary>
        /// <returns>
        /// Uma lista de todas situações possíveis para um Processo
        /// </returns>
        public IEnumerable<SituacaoDTO> ObtemSituacoes()
        {
            return
                Situacao
                    .GetAll<Situacao>()
                    .Select(s => new SituacaoDTO()
                    {
                        Finalizado = s.EstaFinalizado,
                        Nome = s.Nome,
                        Id = s.SituacaoId
                    });
        }

        /// <sumary>
        /// Projeta a busca por uma ou mais processos
        /// </sumary>
        /// <remarks>
        /// sendo o número do Processo Unificado único, quanod informado, todos os outros filtros serão descartados.
        /// </remarks>
        public async Task<Either<IEnumerable<MensagensErros>, IEnumerable<ProcessoConsultadoDTO>>> ConsultaAsync(
            string processoUnificado, DateTime? dataInicialDistribuicao, DateTime? dataFinalDistribuicao, bool? segredoJustica,
            string parcialPastaFisica, short? situacaoId, string parcialNomeResponsavel)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(processoUnificado))
                {
                    return await ConsultaPorProcessoUnificadoAsync(processoUnificado);
                }
                else
                {
                    var regex = new Regex("[^0-9$]");
                    var filtros = new BsonDocument();
                    var periodo = CriaFiltroPeriodo(dataInicialDistribuicao, dataFinalDistribuicao);

                    // Match no Inicio da Coleção
                    if (segredoJustica.HasValue)
                        filtros.Add(new BsonElement("SegredoJustica", new BsonBoolean(segredoJustica.Value)));

                    if (!string.IsNullOrWhiteSpace(parcialPastaFisica))
                        filtros.Add(new BsonElement("PastaFisicaCliente", new BsonRegularExpression($".*{parcialPastaFisica}.*", "i")));

                    if (situacaoId.HasValue)
                        filtros.Add(new BsonElement("SituacaoId", new BsonInt32(situacaoId.Value)));

                    filtros.Add(new BsonElement("DataDistribuicao", periodo));


                    // Match no fim da Pipeline
                    BsonRegularExpression filtroResponsaveis =
                        !string.IsNullOrWhiteSpace(parcialNomeResponsavel)
                            ? new BsonRegularExpression($".*{parcialNomeResponsavel}.*", "i")
                            : new BsonRegularExpression($".*", "i");

                    BsonDocument filtroFinal =
                        new BsonDocument() {
                            { "$match", new BsonDocument()
                                {
                                    { "Responsaveis", new BsonDocument()
                                        { { "$regex", filtroResponsaveis } }
                                    }
                                }
                            }};

                    return await this.ConsultaAsync(filtros, filtroFinal);
                }
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    this.logger.LogError(ex.Message);
                    return true;
                });

                return Left<IEnumerable<MensagensErros>, IEnumerable<ProcessoConsultadoDTO>>(
                    new MensagensErros[] { MensagensErros.ErroNaoEsperado });
            }
        }

        /// <sumary>
        /// Projeta um Processo a partir do seu número de processo unificado
        /// </sumary>
        private async Task<Either<IEnumerable<MensagensErros>, IEnumerable<ProcessoConsultadoDTO>>> ConsultaPorProcessoUnificadoAsync(
            string umProcessoUnificado)
        {
            var regex = new Regex("[^0-9$]");
            var processo = regex.Replace(umProcessoUnificado, string.Empty);

            // Match no fim da Pipeline
            BsonRegularExpression filtroResponsaveis = new BsonRegularExpression($".*", "i");

            BsonDocument filtroFinal =
                new BsonDocument() {
                            { "$match", new BsonDocument()
                                {
                                    { "Responsaveis", new BsonDocument()
                                        { { "$regex", filtroResponsaveis } }
                                    }
                                }
                            }};

            return await this.ConsultaAsync(new BsonDocument(
                    new BsonElement("ProcessoUnificado", new BsonString(processo))), filtroFinal);
        }

        /// <sumary>
        /// Projeta um Processo a partir do seu número de processo unificado
        /// </sumary>
        private async Task<Either<IEnumerable<MensagensErros>, IEnumerable<ProcessoConsultadoDTO>>> ConsultaAsync(
            BsonDocument filtroInicial, BsonDocument filtroFinal)
        {
            IEnumerable<BsonDocument> stages = new BsonDocument[]
            {
                new BsonDocument() { { "$match", filtroInicial } },
                new BsonDocument() { { "$unwind", "$ResponsaveisIds" }},
                new BsonDocument() { { "$lookup", new BsonDocument()
                    {
                        { "from", "responsaveis" },
                        { "localField", "ResponsaveisIds" },
                        { "foreignField", "_id"},
                        { "as", "resp" }
                    }
                }},
                new BsonDocument()
                {
                    { "$group", new BsonDocument()
                        {
                            { "_id", "$_id"},
                            { "ProcessoUnificado", new BsonDocument() { { "$first", "$ProcessoUnificado" } }},
                            { "DataDistribuicao", new BsonDocument() { { "$first", "$DataDistribuicao" } }},
                            { "SegredoJustica", new BsonDocument() { { "$first", "$SegredoJustica" } }},
                            { "PastaFisicaCliente", new BsonDocument() { { "$first", "$PastaFisicaCliente" } }},
                            { "Descricao", new BsonDocument() { { "$first", "$Descricao" } }},
                            { "SituacaoId", new BsonDocument() { { "$first", "$SituacaoId" } }},
                            { "Responsaveis", new BsonDocument()
                                {
                                    { "$addToSet",  new BsonDocument()
                                        {
                                            { "$arrayElemAt", new BsonArray() { "$resp.Nome", 0 } }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }, filtroFinal
            };

            PipelineDefinition<Processo, ProcessoConsultadoDTO> pipeline =
                PipelineDefinition<Processo, ProcessoConsultadoDTO>.Create(stages);

            var cursor = await this.contextoPersistencia.Processos.AggregateAsync<ProcessoConsultadoDTO>(pipeline);
            var projecao = cursor.ToList();

            return Right<IEnumerable<MensagensErros>, IEnumerable<ProcessoConsultadoDTO>>(projecao);
        }

        private BsonDocument CriaFiltroPeriodo(DateTime? dataInicialDistribuicao, DateTime? dataFinalDistribuicao)
        {
            var inicio = dataInicialDistribuicao.HasValue && dataInicialDistribuicao?.Date < DateTime.Now.Date
                            ? dataInicialDistribuicao.Value.Date
                            : DateTime.Now.AddDays(-30).Date;

            var fim = dataFinalDistribuicao.HasValue && dataFinalDistribuicao?.Date > inicio.Date
                        ? dataFinalDistribuicao.Value.Date
                        : DateTime.Now.Date;

            var filtro = new BsonDocument() {
                { "$gte", new BsonDateTime(inicio) },
                { "$lt", new BsonDateTime(fim) }
            };

            return filtro;
        }
    }
}