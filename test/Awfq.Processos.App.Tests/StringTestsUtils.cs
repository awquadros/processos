using System;
using System.Linq;

namespace Awfq.Processos.App.Tests
{
    public class StringTestsUtils
    {
        public const string CHARS_PARA_NOMES = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string RandomString(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(CHARS_PARA_NOMES, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}