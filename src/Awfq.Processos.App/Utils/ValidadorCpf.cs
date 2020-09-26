using System;
using System.Linq;
using System.Text.RegularExpressions;
using Awfq.Comuns;

namespace Awfq.Processos.App.Utils
{
    /// <sumary>
    /// Implementa a interface <see cref="IValidadorCpf" />
    /// </sumary>
    public class ValidadorCpf : IValidadorCpf
    {
        private const short tamanho = 11;

        /// <sumary>
        /// Obtém um valor indicando se se a entrada é um CPF matematicamente válido
        /// </sumary>
        /// <param name="umCpf">um valor candiidato à validação</param>
        /// <returns><c>true</c> caso seja considerado válido e <c>false</c> do contrário
        public bool CpfValido(string umCpf)
        {
            var pattern = "[^0-9$]";
            var candidato = umCpf == null
                ? string.Empty
                : Regex.Replace(umCpf, pattern, string.Empty);

            if (candidato.Trim().Length != tamanho)
                return false;

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma = 0;
            int resto;

            tempCpf = candidato.Substring(0, 9);

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito = resto.ToString();

            tempCpf = tempCpf + digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito = digito + resto.ToString();

            return candidato.EndsWith(digito);
        }
    }
}