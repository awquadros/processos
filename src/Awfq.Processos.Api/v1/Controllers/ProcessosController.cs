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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcessoConsultadoDTO>>> ConsultaProcessos(
            [FromQuery] string processoUnificado, [FromQuery] DateTime? dataInicialDistribuicao, 
            [FromQuery] DateTime? dataFinalDistribuicao, [FromQuery] bool? segredoJustica,
            [FromQuery] string parcialPastaFisica, [FromQuery] short? situacaoId, 
            [FromQuery] string parcialNomeResponsavel)
        {
            var result = 
                await this
                    .servicoConsultaProcessos
                    .ConsultaAsync(
                        processoUnificado, 
                        dataInicialDistribuicao, 
                        dataFinalDistribuicao,
                        segredoJustica,
                        parcialPastaFisica,
                        situacaoId,
                        parcialNomeResponsavel);

            return result.Match(RetornaConsulta, LidaComFalhaConsulta);
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

            return result.Match(CriadoNaRota, NecessarioCorrecaoEntrada);
        }

        [HttpPut]
        public ActionResult<ProcessoDTO> EditaProcesso([FromBody] ProcessoExistenteDTO dto)
        {
            var comando = new ComandoEditaProcesso(
                dto.Id,
                dto.ProcessoUnificado,
                dto.DataDistribuicao,
                dto.SegredoJustica,
                dto.PastaFisicaCliente,
                dto.Responsaveis,
                dto.SituacaoId,
                dto.Descricao,
                dto.PaiId);

            var result = this.servicoAplicacaoProcessos.EditaProcesso(comando);

            return result.Match(CriadoNaRota, NecessarioCorrecaoEntrada);
        }

        [HttpDelete("{id}")]
        public ActionResult<ProcessoDTO> RemoveProcesso(string id)
        {
            var comando = new ComandoRemoveProcesso(id);
            var result = this.servicoAplicacaoProcessos.RemoveProcesso(comando);

            return result.Match(Sucesso, LidaComFalhaRemocao);
        }

        [HttpGet("situacoes/")]
        public IEnumerable<SituacaoDTO> Get()
        {
            return this.servicoConsultaProcessos.ObtemSituacoes().ToArray();
        }

        protected ActionResult<ProcessoDTO> Sucesso(ProcessoDTO r) => Ok(r);

        protected ActionResult<ProcessoDTO> CriadoNaRota(ProcessoDTO r) => Ok(r);

        protected ActionResult<ProcessoDTO> NecessarioCorrecaoEntrada(IEnumerable<MensagensErros> e) => BadRequest(e);

        protected ActionResult<ProcessoDTO> LidaComFalhaRemocao(IEnumerable<MensagensErros> e)
            => e.Contains(MensagensErros.RecursoNaoEncontrado)
                ? (ActionResult<ProcessoDTO>)NoContent()
                : (ActionResult<ProcessoDTO>)BadRequest(e);

        protected ActionResult<IEnumerable<ProcessoConsultadoDTO>> RetornaConsulta(IEnumerable<ProcessoConsultadoDTO> r) => Ok(r);

        protected ActionResult<IEnumerable<ProcessoConsultadoDTO>> LidaComFalhaConsulta(IEnumerable<MensagensErros> e) 
            => StatusCode(500, e);
    }
}
