using System;

namespace Awfq.Processos.App.Dominio.Modelo.Responsaveis
{
    public interface IRepositorioResponsaveis
    {
        ResponsavelId ObtemProximoId();

        Responsavel Salva(Responsavel umResponsavel);
    }
}