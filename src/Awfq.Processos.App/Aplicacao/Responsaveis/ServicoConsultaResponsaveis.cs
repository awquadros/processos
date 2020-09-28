using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;
using Awfq.Processos.App.Aplicacao.Responsaveis.Dados;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB.Abstracoes;
using LanguageExt;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

using static LanguageExt.Prelude;

namespace Awfq.Processos.App.Aplicacao.Responsaveis
{
    /// <sumary>
    /// Implementa um Serviço de Connsulta do Agregado <see cref="Responsavel"/>
    /// </sumary>
    public class ServicoConsultaResponsaveis : IServicoConsultaResponsaveis
    {
        private readonly IContextoPersistencia contextoPersistencia;
        private readonly ILogger logger;

        public ServicoConsultaResponsaveis(IContextoPersistencia umContextoPersistencia, ILogger umLogger)
        {
            this.contextoPersistencia = umContextoPersistencia;
            this.logger = umLogger;
        }

        private async Task<Either<IEnumerable<MensagensErros>, IEnumerable<ResponsavelDTO>>> ConsultaComProcessoAsync(
            string nome, string cpf, string processo)
        {
            // db.getCollection("processos").aggregate([ { $match: { ProcessoUnificado : "35130380020168230027" } }, { $unwind: "$ResponsaveisIds" }, { $lookup: { from: "responsaveis", localField: "ResponsaveisIds", foreignField: "_id", as: "resp" }}, { $group: { _id: "$_id", responsaveis: {$addToSet: {$arrayElemAt:["$resp", 0]}}  } }  ])
            var regex = new Regex("[^0-9$]");
            var filtro = new BsonDocument();

            if (!String.IsNullOrWhiteSpace(processo))
                filtro.Add(new BsonElement("ProcessoUnificado", new BsonString(regex.Replace(processo, String.Empty))));


            PipelineDefinition<Processo, ResponsaveisPorProcessoDTO> pipeline = new BsonDocument[]
            {
                new BsonDocument() { { "$match", filtro } },
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
                            { "Responsaveis", new BsonDocument()
                                {
                                    { "$addToSet",  new BsonDocument()
                                        {
                                            { "$arrayElemAt", new BsonArray() { "$resp", 0 } }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var cursor = await this.contextoPersistencia
                        .Processos
                        .AggregateAsync<ResponsaveisPorProcessoDTO>(pipeline);

            var projecao = cursor.ToList();
            var resultado = projecao != null 
                ? projecao.SelectMany(x => x.Responsaveis) 
                : new ResponsavelDTO[] { };

            if (!string.IsNullOrWhiteSpace(cpf))
                resultado = resultado.Where(x => x.Cpf.Equals(cpf));

            if (!string.IsNullOrWhiteSpace(nome))
                resultado = resultado.Where(x => x.Nome.IndexOf(nome, StringComparison.InvariantCultureIgnoreCase) >= 0);

            return Right<IEnumerable<MensagensErros>, IEnumerable<ResponsavelDTO>>(resultado);
        }

        public async Task<Either<IEnumerable<MensagensErros>, IEnumerable<ResponsavelDTO>>> ConsultaAsync(
            string nome, string cpf, string processo)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(processo))
                {
                    return await ConsultaComProcessoAsync(nome, cpf, processo);
                }
                else
                {

                    var regex = new Regex("[^0-9$]");
                    var parametros = new BsonDocument();

                    if (!String.IsNullOrWhiteSpace(nome))
                        parametros.Add(new BsonElement("Nome", new BsonRegularExpression($".*{nome}.*", "i")));

                    if (!String.IsNullOrWhiteSpace(cpf))
                        parametros.Add(new BsonElement("Cpf", new BsonString(regex.Replace(cpf, String.Empty))));

                    var cursor =
                        await this.contextoPersistencia
                            .Responsaveis
                            .FindAsync<ResponsavelDTO>(parametros);

                    var resultado = cursor.ToList();

                    return Right<IEnumerable<MensagensErros>, IEnumerable<ResponsavelDTO>>(resultado);

                }
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    this.logger.LogError(ex.Message);
                    return true;
                });

                return Left<IEnumerable<MensagensErros>, IEnumerable<ResponsavelDTO>>(
                    new MensagensErros[] { MensagensErros.ErroNaoEsperado });
            }
        }
    }
}