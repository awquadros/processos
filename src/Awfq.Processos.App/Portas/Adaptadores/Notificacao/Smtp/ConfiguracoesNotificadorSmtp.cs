
namespace Awfq.Processos.App.Portas.Adaptadores.Notificacao.Smtp
{
    public class ConfiguracoesNotificadorSmtp : IConfiguracoesNotificadorSmtp
    {
        public string ChaveApi { get; set; }
        public string Segredo { get; set; }
    }
}