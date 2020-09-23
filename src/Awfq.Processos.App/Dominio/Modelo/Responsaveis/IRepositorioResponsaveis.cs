using System;

namespace Awfq.Processos.App.Dominio.Modelo.Responsaveis
{
    /// <sumary>
    /// Define uma interface para um repositório de Responsáveis
    /// </sumary>
    public interface IRepositorioResponsaveis
    {
        /// <sumary>
        /// Obtém um novo Id para ser usado na criação de um novo Responsável
        /// </sumary>
        /// <returns>
        /// Um novo identificador único
        /// </returns>
        Guid ObtemProximoId();

        /// <sumary>
        /// Persiste um novo Responsável
        /// </sumary>
        Responsavel Salva(Responsavel umResponsavel);

        /// <sumary>
        /// Remove um Responsável
        /// </sumary>
        Responsavel Remove(Guid umId);

        /// <sumary>
        /// Dado um CPF, verifica se um determinado Responsavel existe na base de dados
        /// </sumary>
        bool CpfJaCadastrado(String umCpf);
    }
}