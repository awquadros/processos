namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    public interface IConfiguracoesMongoDb
    {
        string NomeColecaoProcessos { get; set; }
        string NomeColecacoResponsaveis { get; set; }
        string StringConexao { get; set; }
        string NomeBaseDados { get; set; }
    }
}