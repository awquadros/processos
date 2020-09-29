using Awfq.Processos.App.Utils;

namespace Awfq.Processos.App.Aplicacao.Responsaveis.Comandos
{
    /// <sumary>
    /// Define os possíveis erros de entrada de dados em Responsáveis.
    /// </sumary>
    public abstract class MensagensErros : Enumeration
    {
        public static readonly MensagensErros ComandoInvalido = new TipoErrosEntrada(0, "Dados do Comando não informados.");
        public static readonly MensagensErros NomeCompletoNaoInformado = new TipoErrosEntrada(1, "O nome completo é obrigatório");
        public static readonly MensagensErros NomeExcedeuLimiteMaximo = new TipoErrosEntrada(2, "O nome excedeu o limite máximo de 150 caracteres.");
        public static readonly MensagensErros CpfNaoInformado = new TipoErrosEntrada(3, "O CPF é obrigatório.");
        public static readonly MensagensErros CpfInvalido = new TipoErrosEntrada(4, "O CPF é inválido.");
        public static readonly MensagensErros CpfDuplicado = new TipoErrosEntrada(5, "Já existe um Responsável registrado com esse CPF.");
        public static readonly MensagensErros EmailExcedeuLimiteMaximo = new TipoErrosEntrada(6, "O E-mail excedeu o limite máximo de 400 caracteres.");
        public static readonly MensagensErros EmailInvalido = new TipoErrosEntrada(7, "O E-mail é inválido.");
        public static readonly MensagensErros EmailNaoInformado = new TipoErrosEntrada(8, "O E-mail é obrigatório.");
        public static readonly MensagensErros IdentificadorUnicoInvalido = new TipoErrosEntrada(9, "O identificador único é inválido.");
        public static readonly MensagensErros RecursoNaoEncontrado = new TipoErrosEntrada(10, "O Recurso não foi encontrado.");
        public static readonly MensagensErros ExisteVinculoProcessual = new TipoErrosEntrada(11, "O Responsável está vinculado a um ou mais processos.");
        public static readonly MensagensErros ErroNaoEsperado = new TipoErrosEntrada(99, "Ocorreu um erro não esperado.");

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