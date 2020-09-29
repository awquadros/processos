using System.Collections.Generic;
using System.Threading.Tasks;
using Awfq.Comuns.Portas.Notificacao;

namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    /// <sumary>
    /// Define uma interface para notificar responsaveis quando são incluidos em Processos
    /// </sumary>
    public interface INotificadorResponsavel
    {
        Task NotificarAsync(INotificacaoSimples notificacao, IEnumerable<(string Nome, string Email)> responsaveis);
    }
}