using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awfq.Processos.App.Aplicacao.Responsaveis;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;
using Awfq.Processos.App.Aplicacao.Responsaveis.Dados;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Awfq.Processos.Api.v1.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsaveisController : ControllerBase
    {
        private readonly ILogger<ProcessosController> logger;
        private readonly IServicoAplicacaoResponsaveis servicoAplicacaoResponsaveis;
        private readonly IServicoConsultaResponsaveis servicoConsultaResponsaveis;

        public ResponsaveisController(
            ILogger<ProcessosController> logger, 
            IServicoAplicacaoResponsaveis servicoAplicacaoResponsaveis,
            IServicoConsultaResponsaveis servicoConsultaResponsaveis)
        {
            this.logger = logger;
            this.servicoAplicacaoResponsaveis = servicoAplicacaoResponsaveis;
            this.servicoConsultaResponsaveis = servicoConsultaResponsaveis;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponsavelDTO>>> ConsultaResponsaveis(
            [FromQuery] string nome, string cpf, string processo) 
        {
            var result = 
                await this
                    .servicoConsultaResponsaveis
                    .ConsultaAsync(nome, cpf, processo);

            return result.Match(retornaConsulta, lidaComFalhaConsulta);
        }

        [HttpPost]
        public ActionResult<ResponsavelDTO> CriaResponsavel([FromBody] NovoResponsavelDTO dto) 
        {
            var comando = new ComandoCriaResponsavel(dto.Nome, dto.Cpf, dto.Email, dto.Foto);
            var result = this.servicoAplicacaoResponsaveis.CriaResponsavel(comando);

            return result.Match(criadoNaRota, usuarioPrecisaCorrigirEntrada);
        }

        [HttpPut]
        public ActionResult<ResponsavelDTO> EditaResponsavel([FromBody] ResponsavelExistenteDTO dto) 
        {
            var comando = new ComandoEditaResponsavel(dto.Id, dto.Nome, dto.Cpf, dto.Email, dto.Foto);
            var result = this.servicoAplicacaoResponsaveis.EditaResponsavel(comando);

            return result.Match(ok, usuarioPrecisaCorrigirEntrada);
        }

        [HttpDelete("{id}")]
        public ActionResult<ResponsavelDTO> RemoveResponsavel(string id) {
            var comando = new ComandoRemoveResponsavel(id);
            var result = this.servicoAplicacaoResponsaveis.RemoveResponsavel(comando);

            return result.Match(ok, lidaComFalhaRemocao);
        }

        protected ActionResult<ResponsavelDTO> ok(ResponsavelDTO r) => Ok(r);

        protected ActionResult<ResponsavelDTO> criadoNaRota(ResponsavelDTO r) => Ok(r);

        protected ActionResult<IEnumerable<ResponsavelDTO>> retornaConsulta(IEnumerable<ResponsavelDTO> r) => Ok(r);

        protected ActionResult<ResponsavelDTO> usuarioPrecisaCorrigirEntrada(IEnumerable<MensagensErros> e) 
            => BadRequest(e);

        protected ActionResult<IEnumerable<ResponsavelDTO>> lidaComFalhaConsulta(IEnumerable<MensagensErros> e) 
            => StatusCode(500, e);

        protected ActionResult<ResponsavelDTO> lidaComFalhaRemocao(IEnumerable<MensagensErros> e) 
            => e.Contains(MensagensErros.RecursoNaoEncontrado) 
                ? (ActionResult<ResponsavelDTO>) NoContent()
                : (ActionResult<ResponsavelDTO>) BadRequest(e);
    }
}
