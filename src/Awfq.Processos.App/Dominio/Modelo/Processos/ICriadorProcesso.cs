using Awfq.Comuns.Portas.Adaptadores.Persistencia;

namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    /// <sumary>
    /// Define uma interface para gerração de Ids de Processos
    /// </sumary>
    public interface ICriadorProcesso : ICriador<Processo, Processo>
    {
    }
}