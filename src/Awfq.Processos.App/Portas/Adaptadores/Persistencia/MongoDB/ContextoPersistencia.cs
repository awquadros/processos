using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB.Abstracoes;
using MongoDB.Driver;

namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    /// <sumary>
    /// Implementa a interface <see cref="IContextoPersistencia" /> que por sua vez
    /// expõe as coleções disponíveis para o Domínio
    /// </sumary>
    public class ContextoPersistencia : IContextoPersistencia
    {
        private readonly ConfiguracoesMongoDb configuracoes;
        private readonly IMongoClient mongoClient;

        /// <sumary>
        /// Inicia uma nova instância da classe <see cref="ContextoPersistencia" />
        /// </sumary>
        /// <param name="mongoClient">uma instância do cliente mongo</param>
        /// <param name="configuracoes">uma instância sw um configurador para a base</param>
        public ContextoPersistencia(IMongoClient mongoClient, ConfiguracoesMongoDb configuracoes)
        {
            this.mongoClient = mongoClient;
            this.configuracoes = configuracoes;
        }

        public IMongoDatabase BaseDados => 
            this.mongoClient.GetDatabase(configuracoes.NomeBaseDados);

        public IMongoCollection<Processo> Processos => 
            this.BaseDados.GetCollection<Processo>(configuracoes.NomeColecaoProcessos);

        public IMongoCollection<Responsavel> Responsaveis => 
            this.BaseDados.GetCollection<Responsavel>(this.configuracoes.NomeColecacoResponsaveis);
    }
}