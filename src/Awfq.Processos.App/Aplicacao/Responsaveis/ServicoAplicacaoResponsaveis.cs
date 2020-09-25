using System;
using System.Collections.Generic;
using System.Linq;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;
using Awfq.Processos.App.Aplicacao.Responsaveis.Dados;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
using Awfq.Processos.App.Utils;
using LanguageExt;

using static LanguageExt.Prelude;

namespace Awfq.Processos.App.Aplicacao.Responsaveis
{
    /// <sumary>
    /// Implementa um Serviço de Aplicação dedicada à manipular a entidade <see cref="Responsavel"/>
    /// </sumary>
    public class ServicoAplicacaoResponsaveis : IServicoAplicacaoResponsaveis
    {
        private readonly IRepositorioResponsaveis repositorio;
        private readonly IRemovedorResponsavel removedor;

        public ServicoAplicacaoResponsaveis(IRepositorioResponsaveis umRepositorio, IRemovedorResponsavel umRemovedor)
        {
            this.repositorio = umRepositorio;
            this.removedor = umRemovedor;
        }

        public Either<IEnumerable<MensagensErros>, ResponsavelDTO> RemoveResponsavel(ComandoRemoveResponsavel cmd)
        {
            var result = 
                validaRemocaoResponsavel(cmd)
                    .Match(ProcedeRemocaoResponsavel, CancelaRemocaoResponsavel);

            return result;
        }

        public Either<IEnumerable<MensagensErros>, ResponsavelDTO> CriaResponsavel(ComandoCriaResponsavel cmd)
        {
            var result =
                validaCriacaoResponsavel(cmd)
                    .Match(ProcedeCriacaoResponsavel, CancelaCriacaoResponsavel);

            return result;
        }

        private Either<IEnumerable<MensagensErros>, ComandoCriaResponsavel> validaCriacaoResponsavel(ComandoCriaResponsavel cmd)
        {
            var erros = new List<MensagensErros>();

            if (string.IsNullOrWhiteSpace(cmd.Nome))
                erros.Add(MensagensErros.NomeCompletoNaoInformado);

            if (!string.IsNullOrWhiteSpace(cmd.Nome) && cmd.Nome.Split(' ').Count() < 2)
                erros.Add(MensagensErros.NomeCompletoNaoInformado);

            if (!string.IsNullOrWhiteSpace(cmd.Nome) && cmd.Nome.Length > 150)
                erros.Add(MensagensErros.NomeExcedeuLimiteMaximo);

            if (string.IsNullOrWhiteSpace(cmd.Cpf))
                erros.Add(MensagensErros.CpfNaoInformado);

            if (string.IsNullOrWhiteSpace(cmd.Email))
                erros.Add(MensagensErros.EmailNaoInformado);

            if (!string.IsNullOrWhiteSpace(cmd.Email) && !ValidadorEmail.IsValidEmail(cmd.Email))
                erros.Add(MensagensErros.EmailInvalido);

            if (!string.IsNullOrWhiteSpace(cmd.Email) && cmd.Email.Length > 400)
                erros.Add(MensagensErros.EmailExcedeuLimiteMaximo);

            if (this.repositorio.CpfJaCadastrado(cmd.Cpf))
                erros.Add(MensagensErros.CpfDuplicado);

            return erros.Any()
                ? Left<IEnumerable<MensagensErros>, ComandoCriaResponsavel>(erros)
                : Right<IEnumerable<MensagensErros>, ComandoCriaResponsavel>(cmd);
        }
        private Either<IEnumerable<MensagensErros>, ResponsavelDTO> ProcedeCriacaoResponsavel(ComandoCriaResponsavel cmd)
        {
            var id = this.repositorio.ObtemProximoId();
            var responsavel = this.repositorio.Salva(new Responsavel(id, cmd.Nome, cmd.Cpf, cmd.Email, cmd.Foto));

            return Right<IEnumerable<MensagensErros>, ResponsavelDTO>(CriarDesse(responsavel));
        }

        private Either<IEnumerable<MensagensErros>, ResponsavelDTO> CancelaCriacaoResponsavel(IEnumerable<MensagensErros> erros)
            => Left<IEnumerable<MensagensErros>, ResponsavelDTO>(erros);

        private Either<IEnumerable<MensagensErros>, ResponsavelDTO> CancelaRemocaoResponsavel(IEnumerable<MensagensErros> erros)
            => Left<IEnumerable<MensagensErros>, ResponsavelDTO>(erros);

        private Either<IEnumerable<MensagensErros>, ComandoRemoveResponsavel> validaRemocaoResponsavel(ComandoRemoveResponsavel cmd) {
            var erros = new List<MensagensErros>();

            if (cmd == null)
                erros.Add(MensagensErros.ComandoInvalido);

            if (!Guid.TryParse(cmd.Id, out Guid id))
                erros.Add(MensagensErros.IdentificadorUnicoInvalido);

            return erros.Any()
                ? Left<IEnumerable<MensagensErros>, ComandoRemoveResponsavel>(erros)
                : Right<IEnumerable<MensagensErros>, ComandoRemoveResponsavel>(cmd);
        }

        private Either<IEnumerable<MensagensErros>, ResponsavelDTO> ProcedeRemocaoResponsavel(ComandoRemoveResponsavel cmd)
        {
            var guid = new Guid(cmd.Id);
            var responsavel = this.removedor.Remove(guid);

            return responsavel == null 
                    ? Left<IEnumerable<MensagensErros>, ResponsavelDTO>(
                        new MensagensErros[] { MensagensErros.RecursoNaoEncontrado })
                    : Right<IEnumerable<MensagensErros>, ResponsavelDTO>(CriarDesse(responsavel));
        }

        private ResponsavelDTO CriarDesse(Responsavel responsavel) {
            return new ResponsavelDTO()
                    {
                        Id = responsavel.Id,
                        Nome = responsavel.Nome,
                        Cpf = responsavel.Cpf,
                        Email = responsavel.Email,
                        Foto = responsavel.Foto
                    };
        }
    }
}