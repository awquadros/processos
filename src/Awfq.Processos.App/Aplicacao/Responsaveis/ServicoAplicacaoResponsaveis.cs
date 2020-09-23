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

        public ServicoAplicacaoResponsaveis(IRepositorioResponsaveis umRepositorio)
        {
            this.repositorio = umRepositorio;
        }

        public Either<IEnumerable<ValidacoesEntrada>, ResponsavelDTO> CriaResponsavel(ComandoCriaResponsavel cmd)
        {
            var result =
                validaCriacaoResponsavel(cmd)
                    .Match(ProcedeCriacaoResponsavel, CancelaCriacaoResponsavel);

            return result;
        }

        private Either<IEnumerable<ValidacoesEntrada>, ComandoCriaResponsavel> validaCriacaoResponsavel(ComandoCriaResponsavel cmd)
        {
            var erros = new List<ValidacoesEntrada>();

            if (string.IsNullOrWhiteSpace(cmd.Nome))
                erros.Add(ValidacoesEntrada.NomeCompletoNaoInformado);

            if (!string.IsNullOrWhiteSpace(cmd.Nome) && cmd.Nome.Split(' ').Count() < 2)
                erros.Add(ValidacoesEntrada.NomeCompletoNaoInformado);

            if (!string.IsNullOrWhiteSpace(cmd.Nome) && cmd.Nome.Length > 150)
                erros.Add(ValidacoesEntrada.NomeExcedeuLimiteMaximo);

            if (string.IsNullOrWhiteSpace(cmd.Cpf))
                erros.Add(ValidacoesEntrada.CpfNaoInformado);

            if (string.IsNullOrWhiteSpace(cmd.Email))
                erros.Add(ValidacoesEntrada.EmailNaoInformado);

            if (!string.IsNullOrWhiteSpace(cmd.Email) && !ValidadorEmail.IsValidEmail(cmd.Email))
                erros.Add(ValidacoesEntrada.EmailInvalido);

            if (!string.IsNullOrWhiteSpace(cmd.Email) && cmd.Email.Length > 400)
                erros.Add(ValidacoesEntrada.EmailExcedeuLimiteMaximo);

            if (this.repositorio.CpfJaCadastrado(cmd.Cpf))
                erros.Add(ValidacoesEntrada.CpfDuplicado);

            return erros.Any()
                ? Left<IEnumerable<ValidacoesEntrada>, ComandoCriaResponsavel>(erros)
                : Right<IEnumerable<ValidacoesEntrada>, ComandoCriaResponsavel>(cmd);
        }
        private Either<IEnumerable<ValidacoesEntrada>, ResponsavelDTO> ProcedeCriacaoResponsavel(ComandoCriaResponsavel cmd)
        {
            var id = this.repositorio.ObtemProximoId();
            var responsavelSalvo = this.repositorio.Salva(new Responsavel(id, cmd.Nome, cmd.Cpf, cmd.Email, cmd.Foto));

            return Right<IEnumerable<ValidacoesEntrada>, ResponsavelDTO>(
                    new ResponsavelDTO()
                    {
                        Id = responsavelSalvo.Id,
                        Nome = responsavelSalvo.Nome,
                        Cpf = responsavelSalvo.Cpf,
                        Email = responsavelSalvo.Email,
                        Foto = responsavelSalvo.Foto
                    });
        }

        private Either<IEnumerable<ValidacoesEntrada>, ResponsavelDTO> CancelaCriacaoResponsavel(IEnumerable<ValidacoesEntrada> erros)
            => Left<IEnumerable<ValidacoesEntrada>, ResponsavelDTO>(erros);
    }
}