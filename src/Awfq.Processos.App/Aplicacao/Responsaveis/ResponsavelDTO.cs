using System;

namespace Awfq.Processos.App.Aplicacao.Dados.Responsaveis
{
    public class ResponsavelDTO
    {
        public Guid Id { get; }
        public string Nome { get; }
        public string Cpf { get; }
        public string Email { get; }
        public string Foto { get; }
    }
}