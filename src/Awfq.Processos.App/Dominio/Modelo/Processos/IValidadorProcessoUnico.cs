namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    /// <sumary>
    /// Define uma interface para verificar se um determinado processo unificado já existe
    /// </sumary>
    public interface IValidadorProcessoUnico
    {
        bool ProcessoJaCadastrado(string processoUnificado);
    }
}