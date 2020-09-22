using System;

namespace Awfq.Processos.App.Dominio.Modelo.Responsaveis
{
    public class ResponsavelId
    {
        public string Id {get; private set;}

        public ResponsavelId(string id) {
            this.Id = id;
        }
    }
}