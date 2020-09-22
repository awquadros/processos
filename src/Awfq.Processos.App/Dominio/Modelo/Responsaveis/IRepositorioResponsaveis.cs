using System;

namespace Awfq.Processos.App.Dominio.Modelo.Responsaveis
{
    public interface IRepositorioResponsaveis
    {
        Guid ObtemProximoId();

        Responsavel Salva(Responsavel umResponsavel);
    }
}