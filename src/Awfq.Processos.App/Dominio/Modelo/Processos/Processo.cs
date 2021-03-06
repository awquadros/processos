using System;
using System.Collections.Generic;

namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    public class Processo
    {
        public Guid Id { get; private set; }
        public Guid? PaiId { get; private set; }
        public string ProcessoUnificado { get; private set; }
        public DateTime? DataDistribuicao { get; private set; }
        public bool SegredoJustica { get; private set; }
        public string PastaFisicaCliente { get; private set; }
        public string Descricao { get; private set; }
        public IEnumerable<Guid> ResponsaveisIds { get; private set; }
        public int SituacaoId { get; private set; }

        /// <sumary>
        /// Inicia uma nova instância da classe <see cref="Processo"/>
        /// </sumary>
        /// <param name="processoUnificado">O número do processo unificado.</param>
        /// <param name="dataDistribuicao">A data de distruição do processo.</param>
        /// <param name="segredoJustica">Um valor indicando se o processo deve tramitar em segredo de jistiça.</param>
        /// <param name="pastaFisicaCliente">A pasta física.</param>
        /// <param name="responsaveisIds">Uma lista de Ids de Responsáveis.</param>
        /// <param name="situacaoId">Um identicador de Situação do Processo.</param>
        /// <param name="descricao">Uma desccrição.</param>
        /// <param name="paiId">Um identificadr único de um Processo Pai.</param>
        public Processo(
            Guid id,
            string processoUnificado,
            DateTime? dataDistribuicao,
            bool segredoJustica,
            string pastaFisicaCliente,
            IEnumerable<Guid> responsaveisIds,
            int situacaoId,
            string descricao,
            Guid? paiId)
        {
            this.Id = id;
            this.ProcessoUnificado = processoUnificado;
            this.DataDistribuicao = dataDistribuicao;
            this.SegredoJustica = segredoJustica;
            this.PastaFisicaCliente = pastaFisicaCliente;
            this.ResponsaveisIds = responsaveisIds;
            this.SituacaoId = situacaoId;
            this.Descricao = descricao;
            this.PaiId = paiId;
        }

        public override string ToString() =>
            $"Id: {this.Id}, PaiId: {this.PaiId}, ProcessoUnificado: {this.ProcessoUnificado}, PastaFisicaCliente: {this.PastaFisicaCliente}";
        
    }
}