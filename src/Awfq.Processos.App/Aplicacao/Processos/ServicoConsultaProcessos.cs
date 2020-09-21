using System;
using System.Linq;
using System.Collections.Generic;
using Awfq.Processos.App.Dominio.Modelo.Processos;
using Awfq.Processos.App.Aplicacao.Processos.Dados;

namespace Awfq.Processos.App.Aplicacao.Processos
{
    public class ServicoConsultaProcessos : IServicoConsultaProcessos
    {
        /// <sumary>
        /// Obtém Situações
        /// </sumary>
        /// <returns>
        /// Uma lista de todas situações possíveis para um Processo
        /// </returns>
        public IEnumerable<SituacaoDTO> ObtemSituacoes()
        {
            return
                Situacao
                    .GetAll<Situacao>()
                    .Select(s => new SituacaoDTO()
                    {
                        Finalizado = s.EstaFinalizado,
                        Nome = s.Nome,
                        Id = s.SituacaoId
                    });
        }
    }
}