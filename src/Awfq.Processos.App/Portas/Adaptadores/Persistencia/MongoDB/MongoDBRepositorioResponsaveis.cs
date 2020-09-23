using System;
using System.Linq;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using MongoDB.Driver;

namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    public class MongoDBRepositorioResponsaveis : IRepositorioResponsaveis
    {
        private readonly ConfiguracoesMongoDb configuracoes;
        private readonly IMongoClient mongoClient;

        public MongoDBRepositorioResponsaveis(ConfiguracoesMongoDb configuracoes, IMongoClient mongoClient) {
            this.configuracoes = configuracoes;
            this.mongoClient = mongoClient;
        }

        /// <sumary>
        /// Dado um CPF, verifica se um determinado Responsavel existe na base de dados
        /// </sumary>
        public bool CpfJaCadastrado(String umCpf)
        {
            return this.mongoClient
                    .GetDatabase(configuracoes.NomeBaseDados)
                    .GetCollection<Responsavel>(configuracoes.NomeColecacoResponsaveis)
                    .Find(r => r.Cpf.Equals(umCpf)).Any();
        }

        public Guid ObtemProximoId()
        {
            return Guid.NewGuid();
        }

        public Responsavel Remove(Guid umId)
        {
            throw new NotImplementedException();
        }

        public Responsavel Salva(Responsavel umResponsavel)
        {

            var db = this.mongoClient.GetDatabase(configuracoes.NomeBaseDados);

            db.GetCollection<Responsavel>(configuracoes.NomeColecacoResponsaveis).InsertOne(umResponsavel);

            return umResponsavel;
        }
    }
}