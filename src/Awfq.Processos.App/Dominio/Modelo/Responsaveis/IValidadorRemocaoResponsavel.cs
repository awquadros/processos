using System;

namespace Awfq.Processos.App.Dominio.Modelo.Responsaveis
{
    /// <sumary>
    /// Define uma interface para determinar se um Responsável não faz parte de algum processo
    /// </sumary>
    public interface IValidadorRemocaoResponsavel
    {
        bool FazParteDeProcesso(Guid umId);
    }
}