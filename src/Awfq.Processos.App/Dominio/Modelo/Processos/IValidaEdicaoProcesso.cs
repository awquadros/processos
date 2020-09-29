using System;

namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    /// <sumary>
    /// Define uma interface para verificar se um determinado processo está finalizado ou não.
    /// Apenas Processos não finalizados podem ser editados
    /// </sumary>
    public interface IValidaEdicaoProcesso
    {
        bool PodeSerEditado(string umProcessoUnificado);
    }
}