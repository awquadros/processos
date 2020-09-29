using System;

namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    /// <sumary>
    /// Define uma interface para obter um agregado de processo pelo ID
    /// </sumary>
    public interface IObtendorProcessoPorId
    {
        Processo ObtemProcessoPorId(Guid umId);
    }
}