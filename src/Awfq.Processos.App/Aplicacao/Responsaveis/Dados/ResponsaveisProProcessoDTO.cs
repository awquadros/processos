using System;
using System.Collections.Generic;

namespace Awfq.Processos.App.Aplicacao.Responsaveis.Dados
{
    public class ResponsaveisPorProcessoDTO
    {
        public Guid Id { get; set; }
        public IEnumerable<ResponsavelDTO> Responsaveis { get; set; }
    }
}