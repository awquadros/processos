using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using MongoDB.Driver;

namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB.Abstracoes
{
    public interface IContextoPersistencia
    {
        IMongoDatabase BaseDados { get; }

        IMongoCollection<Processo> Processos { get; }

        IMongoCollection<Responsavel> Responsaveis { get; }
    }
}