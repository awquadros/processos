using System;

namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    /// <sumary>
    /// Define uma interface para verificar se um determinado processo é Pai de outro
    /// </sumary>
    public interface IValidadorProcessoPai
    {
        bool ProcessoEhPai(Guid umProcessoId);
    }
}