using System;

namespace Awfq.Processos.App.Aplicacao.Responsaveis.Comandos
{
    /// <sumary>
    /// Define um comando para edição de Responsáveis
    /// </sumary>
    public class ComandoEditaResponsavel : IComandoCriaEditaResponsavel
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public string Cpf { get; private set; }
        public string Email { get; private set; }
        public string Foto { get; private set; }

        /// <sumary>
        /// Inicia uma nova instância da classe ComandoEditaResponsavel
        /// </sumary>
        /// <remarks>
        /// O responsável criado por esse comando não terá foto.
        /// </remarks>
        /// <param name="id">O nome identificador úniico de responsável.</param>
        /// <param name="nome">O nome completo do futuro responsável.</param>
        /// <param name="cpf">O CPF do futuro responsável.</param>
        /// <param name="email">O email do futuro responsável.</param>
        public ComandoEditaResponsavel(string id, string nome, string cpf, string email) : this(id, nome, cpf, email, String.Empty)
        {
        }

        /// <sumary>
        /// Inicia uma nova instância da classe ComandoEditaResponsavel
        /// </sumary>
        /// <param name="id">O nome identificador úniico de responsável.</param>
        /// <param name="nome">O nome completo do futuro responsável.</param>
        /// <param name="cpf">O CPF do futuro responsável.</param>
        /// <param name="email">O email do futuro responsável.</param>
        /// <param name="foto">A foto do futuro responsável.</param>
        public ComandoEditaResponsavel(string id, string nome, string cpf, string email, string foto)
        {
            this.Id = id;
            this.Nome = nome;
            this.Cpf = cpf;
            this.Email = email;
            this.Foto = foto;
        }
    }
}