using System;
using Awfq.Comuns.Portas.Adaptadores.Persistencia;

namespace Awfq.Processos.App.Dominio.Modelo.Responsaveis
{
    /// <sumary>
    /// Define uma interface para um repositório de Responsáveis
    /// </sumary>
    public interface IRemovedorResponsavel : IRemovedor<Responsavel, Guid>
    {
    }
}