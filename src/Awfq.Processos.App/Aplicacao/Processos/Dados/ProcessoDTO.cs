using System;
using System.Collections.Generic;

namespace Awfq.Processos.App.Aplicacao.Processos.Dados
{
    public class ProcessoDTO
    {
        public Guid Id { get; set; }
        public Guid? PaiId { get; set; }
        public string ProcessoUnificado { get; set; }
        public DateTime? DataDistribuicao { get; set; }
        public bool SegredoJustica { get; set; }
        public string PastaFisicaCliente { get; set; }
        public string Descricao { get; set; }
        public IEnumerable<Guid> Responsaveis { get; set; }
        public int SituacaoId { get; set; }
    }
}