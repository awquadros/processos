using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awfq.Processos.App.Aplicacao.Processos.Dados;
using Awfq.Processos.App.Aplicacao.Processos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Awfq.Processos.Api.v1.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessosController : ControllerBase
    {
        private readonly ILogger<ProcessosController> _logger;

        private readonly IServicoConsultaProcessos _servicoConsultaProcessos;

        public ProcessosController(ILogger<ProcessosController> logger, IServicoConsultaProcessos servicoConsultaProcessos)
        {
            _logger = logger;
            _servicoConsultaProcessos = servicoConsultaProcessos;
        }

        [HttpGet("situacoes/")]
        public IEnumerable<SituacaoDTO> Get()
        {
            return _servicoConsultaProcessos.ObtemSituacoes().ToArray();
        }
    }
}
