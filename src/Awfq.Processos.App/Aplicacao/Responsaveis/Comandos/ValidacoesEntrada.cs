using Awfq.Processos.App.Utils;

namespace Awfq.Processos.App.Aplicacao.Responsaveis.Comandos
{
    /// <sumary>
    /// Define os possíveis erros de entrada de dados em Responsáveis.
    /// </sumary>
    public abstract class ValidacoesEntrada : Enumeration
    {
        public static readonly ValidacoesEntrada ErroNaoEsperado = new TipoValidacoesEntrada(0, "Ocorreu um erro não esperado.");
        public static readonly ValidacoesEntrada NomeCompletoNaoInformado = new TipoValidacoesEntrada(1, "O nome completo é obrigatório");
        public static readonly ValidacoesEntrada NomeExcedeuLimiteMaximo = new TipoValidacoesEntrada(2, "O nome excedeu o limite máximo de 150 caracteres.");
        public static readonly ValidacoesEntrada CpfNaoInformado = new TipoValidacoesEntrada(3, "O CPF é obrigatório.");
        public static readonly ValidacoesEntrada CpfInvalido = new TipoValidacoesEntrada(4, "O CPF é inválido.");
        public static readonly ValidacoesEntrada CpfDuplicado = new TipoValidacoesEntrada(5, "Já existe um Responsável registrado com esse CPF.");
        public static readonly ValidacoesEntrada EmailExcedeuLimiteMaximo = new TipoValidacoesEntrada(6, "O E-mail excedeu o limite máximo de 400 caracteres.");
        public static readonly ValidacoesEntrada EmailInvalido = new TipoValidacoesEntrada(7, "O E-mail é inválido.");
        public static readonly ValidacoesEntrada EmailNaoInformado = new TipoValidacoesEntrada(8, "O E-mail é obrigatório.");

        /// <sumary>
        /// Obtém um valor considerado o identificador único
        /// </sumary>
        public abstract int ValidacaoEntradaId { get; }


        /// <sumary>
        /// Obtém um valor indicando o Nome da Situação
        /// </sumary>
        public abstract string Nome { get; }

        /// <sumary>
        /// Evita que a classe seja instânciada por clientes
        /// </sumary>
        private ValidacoesEntrada(int id, string name) : base(id, name)
        {
        }

        private class TipoValidacoesEntrada : ValidacoesEntrada
        {
            public TipoValidacoesEntrada(int id, string name) : base(id, name)
            {
            }

            public override string Nome => this.Name;

            public override int ValidacaoEntradaId => this.Id;
        }
    }
}