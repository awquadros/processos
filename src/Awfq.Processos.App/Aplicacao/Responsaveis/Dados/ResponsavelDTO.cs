using System;

namespace Awfq.Processos.App.Aplicacao.Responsaveis.Dados
{
    public class ResponsavelDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
    }
}