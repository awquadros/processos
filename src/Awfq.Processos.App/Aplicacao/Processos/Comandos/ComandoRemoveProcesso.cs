namespace Awfq.Processos.App.Aplicacao.Processos.Comandos
{
    /// <sumary>
    /// Define um comando para remoção Processos
    /// </sumary>
    public class ComandoRemoveProcesso
    {
        public string Id { get; private set; }

        /// <sumary>
        /// Inicia uma nova instância da classe <see cref="ComandoRemoveProcesso" />
        /// </sumary>
        /// <param name="umId">O identificador único do processo que se deseja remover.</param>
        public ComandoRemoveProcesso(string umId)
        {
            this.Id = umId;
        }
    }
}