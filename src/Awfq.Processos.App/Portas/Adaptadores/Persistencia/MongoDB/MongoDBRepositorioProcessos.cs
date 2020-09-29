using System;
using System.Linq;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB.Abstracoes;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    public class MongoDBRepositorioProcessos : 
        ICriadorProcesso, IGeradorIdentificadorProcesso, IRemovedorProcesso, IValidadorProcessoUnico, 
        IValidadorHierarquico, IValidadorProcessoPai, IValidadorSituacaoRemocao, IValidaEdicaoProcesso,
        IObtendorProcessoPorId, IEditorProcesso
    {
        private readonly IContextoPersistencia contextoPersistencia;
        private readonly ILogger logger;


        public MongoDBRepositorioProcessos(IContextoPersistencia umContextoPersistencia, ILogger umLogger)
        {
            this.contextoPersistencia = umContextoPersistencia;
            this.logger = umLogger;
        }

        public bool PodeSerEditado(string umProcessoUnificado) =>
            this.contextoPersistencia.Processos.Find(
                p => p.ProcessoUnificado == umProcessoUnificado && p.SituacaoId != 4 && p.SituacaoId != 5 ).Any();

        public bool JaFinalizado(Guid umProcessoId) =>
            this.contextoPersistencia.Processos.Find(
                p => p.Id == umProcessoId && (p.SituacaoId == 4 || p.SituacaoId == 5 )).Any();
        

        /// <sumary>
        /// Dado um CPF, verifica se um determinado Responsavel existe na base de dados
        /// </sumary>
        public bool CpfJaCadastrado(String umCpf) =>
            this.contextoPersistencia.Responsaveis.Find(r => r.Cpf.Equals(umCpf)).Any();

        public Processo Cria(Processo umAgregado)
        {
            this.contextoPersistencia.Processos.InsertOne(umAgregado);
            return umAgregado;
        }


        public Processo Edita(Processo umProcesso)
        {
            var buscarPor = new BsonDocument()
            {
                { "_id", new BsonBinaryData(umProcesso.Id, GuidRepresentation.Standard) }
            };

            var pai = umProcesso.PaiId.HasValue
                ? new BsonElement("PaiId", new BsonBinaryData(umProcesso.PaiId.Value, GuidRepresentation.Standard))
                : new BsonElement("PaiId", BsonNull.Value);

            var dataDistribuicao = umProcesso.DataDistribuicao.HasValue
                ? new BsonElement("DataDistribuicao", new BsonDateTime(umProcesso.DataDistribuicao.Value))
                : new BsonElement("DataDistribuicao", BsonNull.Value);

            var descricao = umProcesso.Descricao != null
                ? new BsonElement("Descricao", new BsonString(umProcesso.Descricao))
                : new BsonElement("Descricao", BsonNull.Value);

            var atualizarCom = new BsonDocument()
            {
                pai,
                { "ProcessoUnificado", new BsonString(umProcesso.ProcessoUnificado) },
                dataDistribuicao,
                { "SegredoJustica", new BsonBoolean(umProcesso.SegredoJustica) },
                { "PastaFisicaCliente", new BsonString(umProcesso.PastaFisicaCliente) },
                descricao,
                { "ResponsaveisIds", new BsonArray(umProcesso.ResponsaveisIds) },
                { "SituacaoId", new BsonInt32(umProcesso.SituacaoId) }
            };

            var result = 
                this.contextoPersistencia
                    .Processos
                    .FindOneAndUpdate(buscarPor, atualizarCom);

            return result;
        }

        public bool EstaNaMesmaHierarquia(string umId)
        {
            throw new NotImplementedException();
        }

        public Guid ObtemProximoId() => Guid.NewGuid();

        public bool ProcessoEhPai(Guid umProcessoId) =>
            this.contextoPersistencia.Processos.Find(p => p.PaiId == umProcessoId).Any();
        

        public bool ProcessoJaCadastrado(string processoUnificado) =>
            this.contextoPersistencia.Processos.Find(r => r.ProcessoUnificado == processoUnificado).Any();

        public Processo Remove(Guid umId)
        {
            return
                this.contextoPersistencia
                    .Processos
                    .FindOneAndDelete(new BsonDocument()
                    {
                        { "_id", new BsonBinaryData(umId,  GuidRepresentation.Standard) }
                    });
        }

        public Processo ObtemProcessoPorId(Guid umId) =>
            this.contextoPersistencia.Processos.Find(
                new BsonDocument(new BsonElement("_id", 
                    new BsonBinaryData(umId, GuidRepresentation.Standard)))).FirstOrDefault();
    }
}