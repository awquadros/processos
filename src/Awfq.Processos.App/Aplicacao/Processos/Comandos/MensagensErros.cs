using Awfq.Processos.App.Utils;

namespace Awfq.Processos.App.Aplicacao.Processos.Comandos
{
    /// <sumary>
    /// Define os possíveis erros de entrada de dados em Responsáveis.
    /// </sumary>
    public abstract class MensagensErros : Enumeration
    {
        public static readonly MensagensErros ComandoInvalido =
            new TipoErrosEntrada(0, "Dados do Comando não informados.");
        public static readonly MensagensErros NumeroProcessoUnificadoNaoInformado =
            new TipoErrosEntrada(1, "O número do Processo Unificado é obrigatório");
        public static readonly MensagensErros NumeroProcessoUnificadoMalFormatado =
            new TipoErrosEntrada(2, "O número do Processo Unificado deve ter 20 caracteres.");
        public static readonly MensagensErros NumeroProcessoUnificadoDuplicado =
            new TipoErrosEntrada(3, "O número do Processo Unificado já existe.");
        public static readonly MensagensErros DataDistribuicaoInvalida =
            new TipoErrosEntrada(4, "A data de distribuição é inválida (deve ser menor ou igual a data atual).");
        public static readonly MensagensErros PastaFisicaExcedeuLimite =
            new TipoErrosEntrada(5, "A pasta física excedeu o limite máximo de 50 caracteres.");
        public static readonly MensagensErros DescricaoExcedeuLimiteMaximo =
            new TipoErrosEntrada(6, "A descrição excedeu o limite máximo de 1000 caracteres.");
        public static readonly MensagensErros SituacaoNaoInformada =
            new TipoErrosEntrada(7, "É obrigatório informar a Situação.");
        public static readonly MensagensErros ResponsavelNaoInformado =
            new TipoErrosEntrada(8, "É obrigatório informar um Responsável.");
        public static readonly MensagensErros NumeroResponsaveisExcedeuLimite =
            new TipoErrosEntrada(9, "O total de Responsáveis excedeu o limite de 3.");
        public static readonly MensagensErros ResponsavelDuplicado =
            new TipoErrosEntrada(10, "Não é permitido Responsável duplicado.");
        public static readonly MensagensErros ProcessoNaMesmoNívelHierarquico =
            new TipoErrosEntrada(11, "Processo vinculado não deve pertenncer ao mesmo nível Hieraquico.");
        public static readonly MensagensErros IdentificadorMalFormatado =
            new TipoErrosEntrada(12, "É possível que um ou mais dos ids estejam mal formatados. Tenta refazer a requisição.");
        public static readonly MensagensErros RecursoNaoEncontrado = new TipoErrosEntrada(13, "O Recurso não foi encontrado.");
        public static readonly MensagensErros IdentificadorUnicoInvalido = new TipoErrosEntrada(14, "O identificador único é inválido.");
        public static readonly MensagensErros ProcessoJaFinalizado = new TipoErrosEntrada(15, "A Situação do Processo não permite remoção.");
        public static readonly MensagensErros RemocaoDeProcessoPaiNaoPermitida = new TipoErrosEntrada(16, "Não é permitida remoção de uma processo Pai.");
        public static readonly MensagensErros ErroNaoEsperado =
            new TipoErrosEntrada(99, "Ocorreu um erro não esperado.");

        /// <sumary>
        /// Obtém um valor considerado o identificador único
        /// </sumary>
        public abstract int MensagemId { get; }


        /// <sumary>
        /// Obtém um valor indicando o Nome da Situação
        /// </sumary>
        public abstract string Mensagem { get; }

        /// <sumary>
        /// Evita que a classe seja instânciada por clientes
        /// </sumary>
        private MensagensErros(int id, string name) : base(id, name)
        {
        }

        private class TipoErrosEntrada : MensagensErros
        {
            public TipoErrosEntrada(int id, string name) : base(id, name)
            {
            }

            public override string Mensagem => this.Name;

            public override int MensagemId => this.Id;
        }
    }
}