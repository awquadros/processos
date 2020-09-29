using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awfq.Comuns.Portas.Notificacao;
using Awfq.Processos.App.Dominio.Modelo.Processos;

using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Awfq.Processos.App.Portas.Adaptadores.Notificacao.Smtp
{
    /// <sumary>
    /// Implementa a interface <see cref=""/> para o envio de mensagens de Email
    /// </sumary>
    public class NotificadorSmtp : INotificadorResponsavel
    {
        private readonly ILogger logger;
        
        public NotificadorSmtp(ILogger umLogger) {
            this.logger = umLogger;
        }

        public async Task NotificarAsync(INotificacaoSimples notificacao, IEnumerable<(string Nome, string Email)> responsaveis)
        {
            var r = responsaveis
                .Select(x => new JObject { { "Email", x.Email }, { "Name", x.Nome } });

            MailjetClient client = new MailjetClient("b670283a8af9ae848de603d743530a51","92baab854581c924f3c89261b030990a")
            {
                Version = ApiVersion.V3_1,
            };

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.Messages, 
            new JArray {
                new JObject {
                    { "From", new JObject { {"Email", "sistema@awfq.com"}, {"Name", "Sistema"} } }, 
                    { "To", new JArray(r) }, 
                    { "Subject", notificacao.Titulo }, 
                    { "TextPart", notificacao.Mensagem }, 
                    { "HTMLPart", $"<p>{notificacao.Mensagem}</p>"}
                }
             });

            // Envia Mensagem
            MailjetResponse response = await client.PostAsync(request);

            if (response.IsSuccessStatusCode) {
                this.logger.LogInformation(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
            } else {
                this.logger.LogError(string.Format("StatusCode: {0}\n", response.StatusCode));
                this.logger.LogError(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                this.logger.LogError(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
            }
        }
    }
}