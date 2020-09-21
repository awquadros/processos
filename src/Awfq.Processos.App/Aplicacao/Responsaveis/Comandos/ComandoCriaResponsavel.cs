using System;

namespace Awfq.Processos.App.Aplicacao.Responsaveis.Comandos
{
    /// <sumary>
    /// Define um comando para criação de novos Responsáveis
    /// </sumary>
    public class ComandoCriaResponsavel
    {
        public string Nome { get; private set; }
        public string Cpf { get; private set; }
        public string Email { get; private set; }
        public string Foto { get; private set; }

        /// <sumary>
        /// Inicia uma nova instância da classe ComandoCriaResponsavel
        /// </sumary>
        /// <remarks>
        /// O responsável criado por esse comando não terá foto.
        /// </remarks>
        /// <param name="nome">O nome completo do futuro responsável.</param>
        /// <param name="cpf">O CPF do futuro responsável.</param>
        /// <param name="email">O email do futuro responsável.</param>
        public ComandoCriaResponsavel(string nome, string cpf, string email) : this(nome, cpf, email, String.Empty)
        {
        }

        /// <sumary>
        /// Inicia uma nova instância da classe ComandoCriaResponsavel
        /// </sumary>
        /// <param name="nome">O nome completo do futuro responsável.</param>
        /// <param name="cpf">O CPF do futuro responsável.</param>
        /// <param name="email">O email do futuro responsável.</param>
        /// <param name="foto">A foto do futuro responsável.</param>
        public ComandoCriaResponsavel(string nome, string cpf, string email, string foto)
        {
            this.Nome = nome;
            this.Cpf = cpf;
            this.Email = email;
            this.Foto = foto;
        }
    }
}