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
    public class MongoDBRepositorioProcessos : 
        ICriadorProcesso, IGeradorIdentificadorProcesso, IRemovedorProcesso, IValidadorProcessoUnico, 
        IValidadorHierarquico, IValidadorProcessoPai, IValidadorSituacaoRemocao
    {
        private readonly IContextoPersistencia contextoPersistencia;
        private readonly ILogger logger;


        public MongoDBRepositorioProcessos(IContextoPersistencia umContextoPersistencia, ILogger umLogger)
        {
            this.contextoPersistencia = umContextoPersistencia;
            this.logger = umLogger;
        }

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
    }
}