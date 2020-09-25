using System;
using System.Linq;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB.Abstracoes;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    public class MongoDBRepositorioResponsaveis : IRemovedorResponsavel, IRepositorioResponsaveis
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