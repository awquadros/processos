using System;
using System.Net.Mail;

namespace Awfq.Processos.App.Utils
{
    public class ValidadorEmail
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}