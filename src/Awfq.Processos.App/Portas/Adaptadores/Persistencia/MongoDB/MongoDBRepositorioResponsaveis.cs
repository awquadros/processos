using System;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using MongoDB.Driver;

namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    public class MongoDBRepositorioResponsaveis : IRepositorioResponsaveis
    {
        public ResponsavelId ObtemProximoId()
        {
            return new ResponsavelId(Guid.NewGuid().ToString().ToUpperInvariant());
        }

        public Responsavel Salva(Responsavel umResponsavel)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            client.ListDatabaseNames();

            return umResponsavel;
        }
    }
}