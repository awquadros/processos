using System;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    public class MongoDBRepositorioResponsaveis : IRepositorioResponsaveis
    {
        public Guid ObtemProximoId()
        {
            return Guid.NewGuid();
        }

        public Responsavel Salva(Responsavel umResponsavel)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            var client = new MongoClient("mongodb://mongo:27017");
            var db = client.GetDatabase("processosapp");

            db.GetCollection<Responsavel>("responsaveis").InsertOne(umResponsavel);

            return umResponsavel;
        }
    }
}