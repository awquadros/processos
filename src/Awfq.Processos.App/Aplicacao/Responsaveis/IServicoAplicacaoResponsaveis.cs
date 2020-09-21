using System.Collections.Generic;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;
using Awfq.Processos.App.Aplicacao.Responsaveis.Dados;
using LanguageExt;

namespace Awfq.Processos.App.Aplicacao.Responsaveis
{
    public interface IServicoAplicacaoResponsaveis
    {
        Either<IEnumerable<ValidacoesEntrada>, ResponsavelDTO> CriaResponsavel(ComandoCriaResponsavel cmd);
    }
}