using System.Collections.Generic;
using System.Threading.Tasks;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;
using Awfq.Processos.App.Aplicacao.Responsaveis.Dados;
using LanguageExt;

namespace Awfq.Processos.App.Aplicacao.Responsaveis
{
    public interface IServicoConsultaResponsaveis
    {
        Task<Either<IEnumerable<MensagensErros>, IEnumerable<ResponsavelDTO>>> ConsultaAsync(
            string nome, string cpf, string processo);
    }
}