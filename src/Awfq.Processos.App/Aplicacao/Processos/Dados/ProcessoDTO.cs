using System;

namespace Awfq.Processos.App.Aplicacao.Processos.Dados
{
    public class ProcessoDTO
    {
        public Guid Id { get; }
        public Guid PaiId { get; }
        public string ProcessoUnificado { get; }
        public DateTime DataDistribuicao { get; }
        public bool SegredoJustica { get; }
        public string PastaFisicaCliente { get; }
        public string Descricao { get; }
    }
}