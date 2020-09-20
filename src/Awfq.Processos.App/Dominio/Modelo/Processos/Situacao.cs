using Awfq.Processos.App.Utils;

namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    /// <sumary>
    /// Define as Situações possíveis em um Processo.
    /// </sumary>
    public abstract class Situacao : Enumeration
    {
        public static readonly Situacao EmAndamento = new TipoSituacaoNaoFinalizada(1, "Em andamento");
        public static readonly Situacao Desmembrado = new TipoSituacaoNaoFinalizada(2, "Desmembrado");
        public static readonly Situacao EmRecurso = new TipoSituacaoNaoFinalizada(3, "Em recurso");
        public static readonly Situacao Finalizado = new TipoSituacaoFinalizada(4, "Finalizado");
        public static readonly Situacao Arquivado = new TipoSituacaoFinalizada(5, "Arquivado");

        /// <sumary>
        /// Obtém um valor considerado o identificador único
        /// </sumary>
        public abstract int SituacaoId { get; }

        /// <sumary>
        /// Obtém um valor indicando se o Processo está finalizado em dada situação
        /// </sumary>
        public abstract string EstaFinalizado { get; }

        /// <sumary>
        /// Obtém um valor indicando o Nome da Situação
        /// </sumary>
        public abstract string Nome { get; }

        /// <sumary>
        /// Evita que a classe seja instânciada por clientes
        /// </sumary>
        private Situacao(int id, string name) : base(id, name)
        {
        }

        private class TipoSituacaoNaoFinalizada : Situacao
        {
            public TipoSituacaoNaoFinalizada(int id, string name) : base(id, name)
            {
            }

            public override string EstaFinalizado => "Não";

            public override string Nome => this.Name;

            public override int SituacaoId => this.Id;
        }

        private class TipoSituacaoFinalizada : Situacao
        {
            public TipoSituacaoFinalizada(int id, string name) : base(id, name)
            {
            }

            public override string EstaFinalizado => "Sim";

            public override string Nome => this.Name;

            public override int SituacaoId => this.Id;
        }
    }
}