using System;
using System.Net.Mail;
using Awfq.Comuns;

namespace Awfq.Processos.App.Utils
{
    public class ValidadorEmail : IValidadorEmail
    {
        public bool EmailValido(string umEmail)
        {
            try
            {
                new MailAddress(umEmail);

                return true;
            }
            catch (Exception ex) when (
                ex is ArgumentNullException || 
                ex is ArgumentException || 
                ex is FormatException)
            {
                return false;
            }
        }
    }
}