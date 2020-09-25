using System;

namespace Awfq.Comuns.Portas.Adaptadores.Persistencia
{
    public interface IRemovedor<TOut, TIn>
    {
        TOut Remove(TIn umId);
    }
}
