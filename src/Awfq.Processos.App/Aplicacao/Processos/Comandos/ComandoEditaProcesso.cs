using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Awfq.Processos.App.Aplicacao.Processos.Comandos
{
    /// <sumary>
    /// Define um comando para criação de novos Processos
    /// </sumary>
    public class ComandoEditaProcesso : IComandoCriaEditaProcesso
    {
        private readonly Regex regex = new Regex("[^0-9$]");

        public string Id { get; private set; }

        private string processoUnificado;

        public string PaiId { get; private set; }

        public string ProcessoUnificado 
        { 
            get => this.processoUnificado; 
            private set => this.processoUnificado = string.IsNullOrWhiteSpace(value) 
                ? value : regex.Replace(value, String.Empty); 
        }

        public DateTime? DataDistribuicao { get; private set; }

        public bool SegredoJustica { get; private set; }

        public string PastaFisicaCliente { get; private set; }

        public string Descricao { get; private set; }

        public IEnumerable<string> ResponsaveisIds { get; private set; }

        public int SituacaoId { get; private set; }

        /// <sumary>
        /// Inicia uma nova instância da classe <see cref="ComandoEditaProcesso"/>
        /// </sumary>
        /// <param name="processoUnificado">O número do processo unificado.</param>
        /// <param name="dataDistribuicao">A data de distruição do processo.</param>
        /// <param name="segredoJustica">Um valor indicando se o processo deve tramitar em segredo de jistiça.</param>
        /// <param name="pastaFisicaCliente">A pasta física.</param>
        /// <param name="responsaveisIds">Uma lista de Ids de Responsáveis.</param>
        /// <param name="situacaoId">Um identicador de Situação do Processo.</param>
        /// <param name="descricao">Uma desccrição.</param>
        /// <param name="paiId">Um identificadr único de um Processo Pai.</param>
        public ComandoEditaProcesso(
            string id,
            string processoUnificado,
            DateTime dataDistribuicao,
            bool segredoJustica,
            string pastaFisicaCliente,
            IEnumerable<string> responsaveisIds,
            int situacaoId,
            string descricao,
            string paiId)
        {
            this.Id = id;
            this.PaiId = paiId;
            this.ProcessoUnificado = processoUnificado;
            this.DataDistribuicao = dataDistribuicao;
            this.SegredoJustica = segredoJustica;
            this.PastaFisicaCliente = pastaFisicaCliente;
            this.ResponsaveisIds = responsaveisIds == null ? new string[] {} : responsaveisIds ;
            this.SituacaoId = situacaoId;
            this.Descricao = descricao;
        }
    }
}