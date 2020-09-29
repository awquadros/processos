using System;
using System.Collections.Generic;

namespace Awfq.Processos.App.Aplicacao.Processos.Dados
{
    public class ProcessoConsultadoDTO
    {
        public Guid Id { get; set; }
        public string ProcessoUnificado { get; set; }
        public DateTime? DataDistribuicao { get; set; }
        public bool SegredoJustica { get; set; }
        public string PastaFisicaCliente { get; set; }
        public string Descricao { get; set; }
        public IEnumerable<string> Responsaveis { get; set; }
        public int SituacaoId { get; set; }
    }
}