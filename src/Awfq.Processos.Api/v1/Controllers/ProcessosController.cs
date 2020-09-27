using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awfq.Processos.App.Aplicacao.Processos.Dados;
using Awfq.Processos.App.Aplicacao.Processos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Awfq.Processos.App.Aplicacao.Processos.Comandos;

namespace Awfq.Processos.Api.v1.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessosController : ControllerBase
    {
        private readonly ILogger<ProcessosController> logger;

        private readonly IServicoConsultaProcessos servicoConsultaProcessos;

        private readonly IServicoAplicacaoProcessos servicoAplicacaoProcessos;

        public ProcessosController(
            ILogger<ProcessosController> logger, 
            IServicoConsultaProcessos servicoConsultaProcessos,
            IServicoAplicacaoProcessos servicoAplicacaoProcessos)
        {
            this.logger = logger;
            this.servicoConsultaProcessos = servicoConsultaProcessos;
            this.servicoAplicacaoProcessos = servicoAplicacaoProcessos;
        }

        [HttpPost]
        public ActionResult<ProcessoDTO> CriaProcesso([FromBody] NovoProcessoDTO dto)
        {
            var comando = new ComandoCriaProcesso(
                dto.ProcessoUnificado,
                dto.DataDistribuicao, 
                dto.SegredoJustica, 
                dto.PastaFisicaCliente,
                dto.Responsaveis,
                dto.SituacaoId,
                dto.Descricao,
                dto.PaiId);

            var result = this.servicoAplicacaoProcessos.CriaProcesso(comando);

            return result.Match(criadoNaRota, NecessarioCorrecaoEntrada);
        }

        [HttpGet("situacoes/")]
        public IEnumerable<SituacaoDTO> Get()
        {
            return this.servicoConsultaProcessos.ObtemSituacoes().ToArray();
        }

        protected ActionResult<ProcessoDTO> criadoNaRota(ProcessoDTO r) => Ok(r);

        protected ActionResult<ProcessoDTO> NecessarioCorrecaoEntrada(IEnumerable<MensagensErros> e) => BadRequest(e);
    }
}
