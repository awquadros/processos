using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;

namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    /// <sumary>
    /// Define uma interface para notificar responsaveis quando são incluidos em Processos
    /// </sumary>
    public interface IObtentorResponsavel
    {
        IEnumerable<Responsavel> ObtemResponsaveis(Guid[] responsaveisId);
    }
}