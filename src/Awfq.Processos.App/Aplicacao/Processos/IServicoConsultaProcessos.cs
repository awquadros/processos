using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Awfq.Processos.App.Aplicacao.Processos.Comandos;
using Awfq.Processos.App.Aplicacao.Processos.Dados;
using LanguageExt;

namespace Awfq.Processos.App.Aplicacao.Processos
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

        Task<Either<IEnumerable<MensagensErros>, IEnumerable<ProcessoConsultadoDTO>>> ConsultaAsync(
            string processoUnificado, DateTime? dataInicialDistribuicao, DateTime? dataFinalDistribuicao, bool? segredoJustica,
                string parcialPastaFisica, short? situacaoId, string parcialNomeResponsavel);
    }
}