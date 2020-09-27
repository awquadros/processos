namespace Awfq.Processos.App.Dominio.Modelo.Processos
{
    /// <sumary>
    /// Define uma interface para verificar o nivel hierarquivo de um processo candidato a Pai
    /// </sumary>
    public interface IValidadorHierarquico
    {
        bool EstaNaMesmaHierarquia(string umId);
    }
}