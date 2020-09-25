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
        private readonly ILogger<ProcessosController> _logger;
        private readonly IServicoAplicacaoResponsaveis _servicoAplicacaoResponsaveis;

        public ResponsaveisController(
            ILogger<ProcessosController> logger, 
            IServicoAplicacaoResponsaveis servicoAplicacaoResponsaveis)
        {
            _logger = logger;
            _servicoAplicacaoResponsaveis = servicoAplicacaoResponsaveis;
        }

        [HttpPost]
        public ActionResult<ResponsavelDTO> CriaResponsavel([FromBody] NovoResponsavelDTO dto) {

            var comando = new ComandoCriaResponsavel(dto.Nome, dto.Cpf, dto.Email, dto.Foto);
            var result = this._servicoAplicacaoResponsaveis.CriaResponsavel(comando);

            return result.Match(criadoNaRota, usuarioPrecisaCorrigirEntrada);
        }

        [HttpDelete("{id}")]
        public ActionResult<ResponsavelDTO> RemoveResponsavel(string id) {
            var comando = new ComandoRemoveResponsavel(id);
            var result = this._servicoAplicacaoResponsaveis.RemoveResponsavel(comando);

            return result.Match(removido, lidaComFalhaRemocao);
        }

        protected ActionResult<ResponsavelDTO> removido(ResponsavelDTO r) => Ok(r);

        protected ActionResult<ResponsavelDTO> criadoNaRota(ResponsavelDTO r) => Ok(r);

        protected ActionResult<ResponsavelDTO> usuarioPrecisaCorrigirEntrada(IEnumerable<MensagensErros> e) 
            => BadRequest(e);

        protected ActionResult<ResponsavelDTO> lidaComFalhaRemocao(IEnumerable<MensagensErros> e) 
            => e.Contains(MensagensErros.RecursoNaoEncontrado) 
                ? (ActionResult<ResponsavelDTO>) NoContent()
                : (ActionResult<ResponsavelDTO>) BadRequest(e);
    }
}
