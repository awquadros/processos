using System;
using System.Text;

namespace Awfq.Processos.App.Dominio.Modelo.Responsaveis
{
    public class Responsavel
    {
        public Guid Id { get; set; }
        public string Nome { get; private set; }
        public string Cpf { get; private set; }
        public string Email { get; private set; }
        public string Foto { get; private set; }

        public Responsavel(Guid id, string nome, string cpf, string email, string foto) {
            this.Id = id;
            this.Nome = nome;
            this.Cpf = cpf;
            this.Email = email;
            this.Foto = foto;
        }

        public override string ToString()
        {
            return new StringBuilder().Append("Id: ").Append(this.Id).ToString();
        }
    }
}