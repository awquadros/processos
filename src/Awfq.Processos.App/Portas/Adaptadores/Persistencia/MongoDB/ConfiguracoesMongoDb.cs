namespace Awfq.Processos.App.Portas.Adaptadores.Persistencia.MongoDB
{
    public class ConfiguracoesMongoDb : IConfiguracoesMongoDb
    {
        public string NomeColecaoProcessos { get; set; }
        public string NomeColecacoResponsaveis { get; set; }
        public string StringConexao { get; set; }
        public string NomeBaseDados { get; set; }
    }
}