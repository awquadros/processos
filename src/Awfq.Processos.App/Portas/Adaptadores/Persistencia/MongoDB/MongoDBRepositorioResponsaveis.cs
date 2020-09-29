using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB.Abstracoes;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    public class MongoDBRepositorioResponsaveis : 
        IEditorResponsavel, IRemovedorResponsavel, 
        IRepositorioResponsaveis, IValidadorRemocaoResponsavel, IObtentorResponsavel
    {
        private readonly IContextoPersistencia contextoPersistencia;
        private readonly ILogger logger;


        public MongoDBRepositorioResponsaveis(IContextoPersistencia umContextoPersistencia, ILogger umLogger)
        {
            this.contextoPersistencia = umContextoPersistencia;
            this.logger = umLogger;
        }

        /// <sumary>
        /// Dado um CPF, verifica se um determinado Responsavel existe na base de dados
        /// </sumary>
        public bool CpfJaCadastrado(String umCpf) =>
            this.contextoPersistencia.Responsaveis.Find(r => r.Cpf.Equals(umCpf)).Any();

        public Responsavel Edita(Responsavel umResponsavel)
        {
            var buscarPor = new BsonDocument()
            {
                { "_id", new BsonBinaryData(umResponsavel.Id, GuidRepresentation.Standard) }
            };

            var atualizarCom = new BsonDocument()
            {
                { "Nome", new BsonString(umResponsavel.Nome) },
                { "Cpf", new BsonString(umResponsavel.Cpf) },
                { "Email", new BsonString(umResponsavel.Email) }
            };

            if (umResponsavel.Foto == null)
            {
                atualizarCom.Add(new BsonElement("Foto", BsonNull.Value));
            }
            else
            {
                atualizarCom.Add(new BsonElement("Foto", new BsonString(umResponsavel.Foto)));
            }

            var result = 
                this.contextoPersistencia
                    .Responsaveis
                    .FindOneAndUpdate(buscarPor, atualizarCom);

            this.logger.LogInformation((result == null).ToString());

                    return result;
        }

        public bool FazParteDeProcesso(Guid umId)
        {
            // db.getCollection("processos").aggregate([ { $project: { "_id": 0,  ResponsaveisIds: { $filter: { input: "$ResponsaveisIds", 
            // as: "item", cond: { $eq: ["$$item", BinData(3,"AlcB6StI3kiMD+QhiIArFw==") ]}}}}}, { $project: {ResponsaveisIds: 1, Size: {$size: "$ResponsaveisIds"} } }, 
            // { $match: { Size: {$gt: 0} }}, { $count: "total" } ])

            BsonDocument filter = new BsonDocument() {
                { "$filter", new BsonDocument()
                    { 
                        { "input", "$ResponsaveisIds"},
                        { "as", "item" }, 
                        { "cond", new BsonDocument() 
                            { 
                                { "$eq", new BsonArray() { "$$item", new BsonBinaryData(umId, GuidRepresentation.Standard) } } 
                            } 
                        } 
                    } 
                }
            };

            PipelineDefinition<Processo, dynamic> pipeline = new BsonDocument[] 
            {
                new BsonDocument() {
                    { "$project", new BsonDocument()
                        { 
                            { "ResponsaveisIds", filter }
                        }
                    }
                },
                new BsonDocument() 
                {
                    { "$project", new BsonDocument()
                        { 
                            { "ResponsaveisIds", 1 },
                            { "Size", new BsonDocument(){ { "$size", "$ResponsaveisIds" } } }
                        }
                    }
                },
                new BsonDocument() 
                {
                    { "$match", new BsonDocument()
                        { 
                            { "Size", new BsonDocument() 
                                { 
                                    { "$gt", 0 } 
                                } 
                            } 
                        }
                    }
                },
                new BsonDocument()
                {
                    { "$count", "total" }
                }
            };

            var resultado = 
                this.contextoPersistencia
                    .Processos
                    .Aggregate(pipeline)
                    .ToList()
                    .Any();

            this.logger.LogInformation("**************************************" + resultado.ToString());

            return resultado;
        }

        public Guid ObtemProximoId() => Guid.NewGuid();

        public IEnumerable<Responsavel> ObtemResponsaveis(Guid[] responsaveisId)
        {   
            var ids = responsaveisId.Select(x => new BsonBinaryData(x, GuidRepresentation.Standard));
            var result = this.contextoPersistencia.Responsaveis.Find(new BsonDocument() {
                { "_id", new BsonDocument(new BsonElement("$in", new BsonArray(ids)))}
            });

            return result.ToList();
        }

        public Responsavel Remove(Guid umId)
        {
            return
                this.contextoPersistencia
                    .Responsaveis
                    .FindOneAndDelete(new BsonDocument()
                    {
                        { "_id", new BsonBinaryData(umId,  GuidRepresentation.Standard) }
                    });
        }

        public Responsavel Salva(Responsavel umResponsavel)
        {
            this.contextoPersistencia.Responsaveis.InsertOne(umResponsavel);
            return umResponsavel;
        }
    }
}