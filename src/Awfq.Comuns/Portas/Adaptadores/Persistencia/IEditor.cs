namespace Awfq.Comuns.Portas.Adaptadores.Persistencia
{
    public interface IEditor<TOut, TIn>
    {
        TOut Edita(TIn umAgregado);
    }
}
