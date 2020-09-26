using System;

namespace Awfq.Processos.App.Dominio.Modelo.Responsaveis
{
    /// <sumary>
    /// Define um colaborador com o papel de Responsável por um ou mais <see cref="Processo" />
    /// </sumary>
    public class Responsavel
    {
        public Guid Id { get; set; }
        public string Nome { get; private set; }
        public string Cpf { get; private set; }
        public string Email { get; private set; }

        /// <sumary>
        /// Obtém um valor representando uma foto do Responsável.
        /// </sumary>
        /// <remarks>
        /// A representação deve estar em Base64
        /// </remarks>
        public string Foto { get; private set; }

        /// <sumary>
        /// Inicia uma nova instância da classe <see cref="Responsavel" />
        /// </sumary>
        public Responsavel(Guid id, string nome, string cpf, string email, string foto)
        {
            this.Id = id;
            this.Nome = nome;
            this.Cpf = cpf;
            this.Email = email;
            this.Foto = foto;
        }

        public override string ToString() =>
            $"Id: {this.Id}, Nome: {this.Nome}, Cpf: {this.Cpf}, Email: {this.Email}";
        
    }
}