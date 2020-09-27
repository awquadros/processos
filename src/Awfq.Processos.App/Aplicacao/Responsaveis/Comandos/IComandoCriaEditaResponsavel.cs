namespace Awfq.Processos.App.Aplicacao.Responsaveis.Comandos
{
    /// <sumary>
    /// Define um comando para criação e edição de Responsáveis
    /// </sumary>
    interface IComandoCriaEditaResponsavel
    {
        string Nome { get; }
        string Cpf { get; }
        string Email { get; }
        string Foto { get; }
    }
}