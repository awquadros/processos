using System;
using System.Collections.Generic;

namespace Awfq.Processos.App.Aplicacao.Processos.Comandos
{
    /// <sumary>
    /// Define um comando para criação e edição de Processos
    /// </sumary>
    interface IComandoCriaEditaProcesso
    {
        string PaiId { get; }
        string ProcessoUnificado { get; }
        DateTime DataDistribuicao { get; }
        bool SegredoJustica { get; }
        string PastaFisicaCliente { get; }
        string Descricao { get; }
        IEnumerable<string> ResponsaveisIds { get; }
        int SituacaoId { get; }
    }
}