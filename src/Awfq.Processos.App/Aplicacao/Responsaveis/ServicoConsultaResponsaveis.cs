using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;
using Awfq.Processos.App.Aplicacao.Responsaveis.Dados;
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
        public async Task<Either<IEnumerable<MensagensErros>, IEnumerable<ResponsavelDTO>>> ConsultaAsync(
            string nome, string cpf, string processo)
        {
            try
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

                this.logger.LogInformation(resultado.Count.ToString());

                return Right<IEnumerable<MensagensErros>, IEnumerable<ResponsavelDTO>>(resultado);
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