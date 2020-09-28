using System;
using System.Collections.Generic;
using System.Linq;
using Awfq.Comuns;
using Awfq.Processos.App.Aplicacao.Responsaveis.Comandos;
using Awfq.Processos.App.Aplicacao.Responsaveis.Dados;
using Awfq.Processos.App.Dominio.Modelo.Responsaveis;
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
        private readonly IEditorResponsavel editor;
        private readonly IValidadorEmail validadorEmail;
        private readonly IValidadorCpf validadorCpf;
        private readonly IEnumerable<PassoValidacao> pipelineValidacaoCriacao;
        private readonly IEnumerable<PassoValidacao> pipelineValidacaoEdicao;
        private readonly IValidadorRemocaoResponsavel validadorRemocao;

        delegate (IEnumerable<MensagensErros>, IComandoCriaEditaResponsavel) PassoValidacao(
            (IEnumerable<MensagensErros> erros, IComandoCriaEditaResponsavel cmd) fluxo);

        /// <sumary>
        /// Inicia uma nova isntância da classe <see cref="ServicoAplicacaoResponsaveis" />
        /// </sumary>
        public ServicoAplicacaoResponsaveis(
            IRepositorioResponsaveis umRepositorio, IRemovedorResponsavel umRemovedor, IEditorResponsavel umEditor,
            IValidadorEmail umValidadorEmail, IValidadorCpf umValidadorCpf, IValidadorRemocaoResponsavel validadorRemocao)
        {
            this.repositorio = umRepositorio;
            this.removedor = umRemovedor;
            this.editor = umEditor;
            this.validadorEmail = umValidadorEmail;
            this.validadorCpf = umValidadorCpf;
            this.validadorRemocao = validadorRemocao;
            this.pipelineValidacaoCriacao = new PassoValidacao[] { ValidaNome, ValidaCpf, ValidaEmail };
            this.pipelineValidacaoEdicao = new PassoValidacao[] { ValidaNome, ValidaApenasValorCpf, ValidaEmail };
        }

        public Either<IEnumerable<MensagensErros>, ResponsavelDTO> RemoveResponsavel(ComandoRemoveResponsavel cmd)
        {
            var result =
                validaRemocaoResponsavel(cmd)
                    .Match(ProcedeRemocaoResponsavel, CancelaAcao<ResponsavelDTO>);

            return result;
        }

        public Either<IEnumerable<MensagensErros>, ResponsavelDTO> CriaResponsavel(ComandoCriaResponsavel cmd)
        {
            var result =
                ValidaCriacao(cmd)
                    .Match(ProcedeCriacao, CancelaAcao<ResponsavelDTO>);

            return result;
        }

        public Either<IEnumerable<MensagensErros>, ResponsavelDTO> EditaResponsavel(ComandoEditaResponsavel cmd)
        {
            var result =
                ValidaEdicao(cmd)
                    .Match<Either<IEnumerable<MensagensErros>, ComandoEditaResponsavel>>(AdaptaComando, CancelaAcao<ComandoEditaResponsavel>)
                    .Match(ProcedeEdicao, CancelaAcao<ResponsavelDTO>);

            return result;
        }

        private Either<IEnumerable<MensagensErros>, ComandoEditaResponsavel> AdaptaComando(IComandoCriaEditaResponsavel cmd) => cmd switch
        {
            ComandoEditaResponsavel ced => Right((ComandoEditaResponsavel)cmd),
            _ => Left((IEnumerable<MensagensErros>)new MensagensErros[] { MensagensErros.ErroNaoEsperado })
        };

        private bool Nega(bool v) => !v;
        private bool NuloOuVazio(string str) => string.IsNullOrWhiteSpace(str);
        private bool NaoNulaOuVazia(string str) => compose<string, bool, bool>(NuloOuVazio, Nega)(str);
        private bool PalavrasMinimas(string str, int tamanho) => str.Split(' ').Count() >= tamanho;

        private PassoValidacao ValidaNomeNaoInformado => f =>
            NuloOuVazio(f.cmd.Nome) ? (f.erros.Append(MensagensErros.NomeCompletoNaoInformado), f.cmd) : f;

        private PassoValidacao ValidaNomeCompleto => f =>
            NaoNulaOuVazia(f.cmd.Nome) && !PalavrasMinimas(f.cmd.Nome, 2)
                ? (f.erros.Append(MensagensErros.NomeCompletoNaoInformado), f.cmd) : f;

        private PassoValidacao ValidaNomeLimite => f => NaoNulaOuVazia(f.cmd.Nome) && f.cmd.Nome.Length > 150
            ? (f.erros.Append(MensagensErros.NomeExcedeuLimiteMaximo), f.cmd) : f;
        private PassoValidacao ValidaNome => f => ValidaNomeLimite(ValidaNomeCompleto(ValidaNomeNaoInformado(f)));

        private PassoValidacao ValidaCpfNaoInformado => f => NuloOuVazio(f.cmd.Cpf)
            ? (f.erros.Append(MensagensErros.CpfNaoInformado), f.cmd) : f;

        private PassoValidacao ValidaCpfDigito => f => NaoNulaOuVazia(f.cmd.Cpf) && !validadorCpf.CpfValido(f.cmd.Cpf)
            ? (f.erros.Append(MensagensErros.CpfInvalido), f.cmd) : f;

        private PassoValidacao TalvezValideCpfNoCadastro => f =>
            f.erros.Contains(MensagensErros.CpfInvalido) || f.erros.Contains(MensagensErros.CpfNaoInformado)
                ? f : ValidaCpfJaCadastrado(f);

        private PassoValidacao ValidaCpfJaCadastrado => f => this.repositorio.CpfJaCadastrado(f.cmd.Cpf)
            ? (f.erros.Append(MensagensErros.CpfDuplicado), f.cmd) : f;

        private PassoValidacao ValidaCpf => f => TalvezValideCpfNoCadastro(ValidaCpfDigito(ValidaCpfNaoInformado(f)));

        private PassoValidacao ValidaApenasValorCpf => f => ValidaCpfDigito(ValidaCpfNaoInformado(f));

        private PassoValidacao ValidaEmailNaoInformado => f => NuloOuVazio(f.cmd.Email) 
            ? (f.erros.Append(MensagensErros.EmailNaoInformado), f.cmd) : f;

        private PassoValidacao ValidaEmailInvalido => f => NaoNulaOuVazia(f.cmd.Email) && !validadorEmail.EmailValido(f.cmd.Email)
            ? (f.erros.Append(MensagensErros.EmailInvalido), f.cmd) : f;

        private PassoValidacao ValidaEmailExcedeuLimiteMaximo => f => NaoNulaOuVazia(f.cmd.Email) && f.cmd.Email.Length > 400
            ? (f.erros.Append(MensagensErros.EmailExcedeuLimiteMaximo), f.cmd) : f;

        private PassoValidacao ValidaEmail => f => ValidaEmailExcedeuLimiteMaximo(ValidaEmailInvalido(ValidaEmailNaoInformado(f)));

        private (IEnumerable<MensagensErros>, IComandoCriaEditaResponsavel) ExecutaPipelineValidacao(
            IEnumerable<PassoValidacao> p, IComandoCriaEditaResponsavel cmd) 
        {
            IEnumerable<MensagensErros> acc = new List<MensagensErros>();
            return p.Aggregate((acc, cmd), (acc, passo) =>
            {
                (IEnumerable<MensagensErros> e, IComandoCriaEditaResponsavel) r = passo(acc); return r;
            });
        }

        private Either<IEnumerable<MensagensErros>, IComandoCriaEditaResponsavel> ValidaCriacao(
            in IComandoCriaEditaResponsavel cmd)
        {
            (IEnumerable<MensagensErros> erros, _) = ExecutaPipelineValidacao(this.pipelineValidacaoCriacao, cmd);

            return erros.Any()
                ? Left<IEnumerable<MensagensErros>, IComandoCriaEditaResponsavel>(erros)
                : Right<IEnumerable<MensagensErros>, IComandoCriaEditaResponsavel>(cmd);
        }


        private Either<IEnumerable<MensagensErros>, IComandoCriaEditaResponsavel> ValidaEdicao(
            in IComandoCriaEditaResponsavel cmd)
        {
            (IEnumerable<MensagensErros> erros, _) = ExecutaPipelineValidacao(this.pipelineValidacaoEdicao, cmd);

            return erros.Any()
                ? Left<IEnumerable<MensagensErros>, IComandoCriaEditaResponsavel>(erros)
                : Right<IEnumerable<MensagensErros>, IComandoCriaEditaResponsavel>(cmd);
        }

        private Either<IEnumerable<MensagensErros>, ResponsavelDTO> ProcedeCriacao(IComandoCriaEditaResponsavel cmd)
        {
            var id = this.repositorio.ObtemProximoId();
            var responsavel = this.repositorio.Salva(new Responsavel(id, cmd.Nome, cmd.Cpf, cmd.Email, cmd.Foto));

            return Right<IEnumerable<MensagensErros>, ResponsavelDTO>(CriarDesse(responsavel));
        }

        private Either<IEnumerable<MensagensErros>, ResponsavelDTO> ProcedeEdicao(ComandoEditaResponsavel cmd)
        {
            var guid = new Guid(cmd.Id);
            var responsavel = this.editor.Edita(new Responsavel(guid, cmd.Nome, cmd.Cpf, cmd.Email, cmd.Foto));

            return Right<IEnumerable<MensagensErros>, ResponsavelDTO>(CriarDesse(responsavel));
        }

        private Either<IEnumerable<MensagensErros>, TRight> CancelaAcao<TRight>(IEnumerable<MensagensErros> erros)
            => Left<IEnumerable<MensagensErros>, TRight>(erros);

        private Either<IEnumerable<MensagensErros>, ComandoRemoveResponsavel> validaRemocaoResponsavel(ComandoRemoveResponsavel cmd)
        {
            var erros = new List<MensagensErros>();

            if (cmd == null)
                erros.Add(MensagensErros.ComandoInvalido);

            if (!Guid.TryParse(cmd.Id, out Guid id))
                erros.Add(MensagensErros.IdentificadorUnicoInvalido);

            if (id != Guid.Empty && this.validadorRemocao.FazParteDeProcesso(id))
                erros.Add(MensagensErros.ExisteVinculoProcessual);
                
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

        private ResponsavelDTO CriarDesse(Responsavel responsavel)
        {
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