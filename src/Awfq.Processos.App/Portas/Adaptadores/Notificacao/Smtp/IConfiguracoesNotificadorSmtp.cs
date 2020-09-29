namespace Awfq.Processos.App.Portas.Adaptadores.Notificacao.Smtp
{
    public interface IConfiguracoesNotificadorSmtp
    {
        string ChaveApi { get; set; }
        string Segredo { get; set; }
    }
}