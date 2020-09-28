using System;
using System.Linq;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB.Abstracoes;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    public class MongoDBRepositorioResponsaveis : 
        IEditorResponsavel, IRemovedorResponsavel, IRepositorioResponsaveis, IValidadorRemocaoResponsavel
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

            this.logger.LogInformation("criou a busca com " + umResponsavel.Id.ToString());

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
            // db.getCollection("processos").aggregate([ { $project: { ResponsaveisIds: { $filter: { input: "$ResponsaveisIds", as: "item", cond: { $eq: ["$$item", BinData(3,"mUTCwFT0B063HN7a92uemw==") ]  } } } } }, { $count: "total" } ])
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
                    { "$count", "total" }
                }
            };

            var resultado = 
                this.contextoPersistencia
                    .Processos
                    .Aggregate(pipeline)
                    .First();

            int total = resultado.total;

            return total > 0;
        }

        public Guid ObtemProximoId() => Guid.NewGuid();

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