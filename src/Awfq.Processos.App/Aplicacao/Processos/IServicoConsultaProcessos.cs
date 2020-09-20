using System.Collections.Generic;
using Awfq.Processos.App.Dominio.Modelo.Processos;

namespace Awfq.Processos.App.Aplicacao.Dados.Processos
{
    public interface IServicoConsultaProcessos
    {
        /// <sumary>
        /// Obtém Situações
        /// </sumary>
        /// <returns>
        /// Uma lista de todas situações possíveis para um Processo
        /// </returns>
        IEnumerable<SituacaoDTO> ObtemSituacoes();
    }
}