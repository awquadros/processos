using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awfq.Processos.App.Aplicacao.Processos;
using Awfq.Processos.App.Aplicacao.Processos.Dados;
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

        public ResponsaveisController(ILogger<ProcessosController> logger, IServicoAplicacaoResponsaveis servicoAplicacaoResponsaveis)
        {
            _logger = logger;
            _servicoAplicacaoResponsaveis = servicoAplicacaoResponsaveis;
        }

        [HttpPost]
        public ActionResult<ResponsavelDTO> CriaResponsavel(NovoResponsavelDTO dto) {

            var comando = new ComandoCriaResponsavel(dto.Nome, dto.Cpf, dto.Email, dto.Foto);
            var result = this._servicoAplicacaoResponsaveis.CriaResponsavel(comando);
            
            return CreatedAtRoute(1, "");
        }
    }
}
