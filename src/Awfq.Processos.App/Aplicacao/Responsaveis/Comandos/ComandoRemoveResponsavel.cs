using System;

namespace Awfq.Processos.App.Aplicacao.Responsaveis.Comandos
{
    /// <sumary>
    /// Define um comando para remoção Responsáveis
    /// </sumary>
    public class ComandoRemoveResponsavel
    {
        public string Id { get; private set; }

        /// <sumary>
        /// Inicia uma nova instância da classe <see cref="ComandoRemoveResponsavel" />
        /// </sumary>
        /// <param name="umId">O identificador único do responsável que se deseja remover.</param>
        public ComandoRemoveResponsavel(string umId)
        {
            this.Id = umId;
        }
    }
}