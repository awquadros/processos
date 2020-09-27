namespace Awfq.Comuns.Portas.Adaptadores.Persistencia
{
    public interface ICriador<TOut, TIn>
    {
        TOut Cria(TIn umAgregado);
    }
}
