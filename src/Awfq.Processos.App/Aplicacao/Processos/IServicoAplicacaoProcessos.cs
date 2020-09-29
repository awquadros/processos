using System.Collections.Generic;
using Awfq.Processos.App.Aplicacao.Processos.Comandos;
using Awfq.Processos.App.Aplicacao.Processos.Dados;
using LanguageExt;

namespace Awfq.Processos.App.Aplicacao.Processos
{
    public interface IServicoAplicacaoProcessos
    {
        Either<IEnumerable<MensagensErros>, ProcessoDTO> CriaProcesso(ComandoCriaProcesso cmd);

        Either<IEnumerable<MensagensErros>, ProcessoDTO> EditaProcesso(ComandoEditaProcesso cmd);

        Either<IEnumerable<MensagensErros>, ProcessoDTO> RemoveProcesso(ComandoRemoveProcesso cmd);
    }
}