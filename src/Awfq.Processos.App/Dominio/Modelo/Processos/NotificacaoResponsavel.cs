using Awfq.Comuns.Portas.Notificacao;

namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    public class NotificacaoResponsavel : INotificacaoSimples
    {
        public string Titulo { get; private set; }

        public string Mensagem { get; private set; }

        public NotificacaoResponsavel(string umTitulo, string umaMensagem)
        {
            this.Titulo = umTitulo;
            this.Mensagem = umaMensagem;
        }
    }
}