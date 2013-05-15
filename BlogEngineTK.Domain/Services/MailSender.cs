using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace BlogEngineTK.Domain.Services
{
    class MailSender
    {
        public enum FromEmailServer
        {
            gmail,
        }

        /// <summary>
        /// Отправляет письмо пользователю с новым паролем
        /// </summary>
        /// <param name="toEmail">newEmail-адрес пользователя</param>
        /// <param name="login">Логин пользователя</param>
        /// <param name="clearText">Пароль пользователя</param>
        /// <param name="codeToNewPass">Ключ для генерации нового пароля</param>
        /// <param name="salt">Дополнение к ключу для генерации нового пароля</param>
        /// <param name="emailServer">Сервер с которого нужно отправить письмо</param>
        public static void SendEmailToNewPass(string toEmail, string login, char[] clearText,
            string urlToAcceptNewpassword, FromEmailServer emailServer)
        {
            // Отправляет активационный код, логин и пароль пользователю
            
            string subject = "Генерация нового пароля BlogEngineTK";

            StringBuilder msgBody = new StringBuilder();

            msgBody.Append("Уважаемый " + login + @", вы запросили генерацию нового пароля.
							Пожалуйста перейдите по ");
            msgBody.AppendFormat(@"<a href=""http://{0}"">этой ссылке</a> для подтверждения нового пароля.",
                                    urlToAcceptNewpassword);
            msgBody.Append("<br />");
            msgBody.Append("<br />");
            msgBody.AppendFormat("Ваш логин: {0}", login);
            msgBody.Append("<br />");
            msgBody.AppendFormat("Ваш пароль: {0}", new string(clearText));
            msgBody.Append("<br />");
            msgBody.Append("Если Вы не запрашивали генерацию нового пароля, то можете проигнорировать это письмо," +
                " ваш текущий пароль не будет затронут.");
            msgBody.Append("<br /><br /><br />");
            msgBody.Append("Пожалуйста, не отвечайте на это письмо.");
            msgBody.Append("<br /><hr><br />");
            msgBody.Append("С уважением, команда портала BlogEngineTK");
            
            switch (emailServer)
            {
                case FromEmailServer.gmail:
                    SendEmailViaGmail(toEmail, subject, msgBody.ToString());
                    break;
            }
            
            msgBody.Clear();
        }

        /// <summary>
        /// Отправляет письмо пользователю для потверждения нового newEmail-адреса
        /// </summary>
        /// <param name="toEmail">newEmail-адрес пользователя</param>
        /// <param name="login">Логин пользователя</param>
        /// <param name="clearText">Пароль пользователя</param>
        /// <param name="codeToNewPass">Активационный ключ</param>
        /// <param name="emailServer">Сервер с которого нужно отправить письмо</param>
        public static void SendConfirmEmail(string toEmail, string login, string urlToConfirm, FromEmailServer emailServer)
        {
            // (безопасность хромает, я думаю не стоит менять email считывая его со строки запроса (передающий метод), 
            // но нужно успеть в срок сделать)

            string subject = "Подтверждение email-адреса TestOnline";

            StringBuilder msgBody = new StringBuilder();

            msgBody.AppendFormat("Уважаемый {0}, вы изменили свой email-адрес на нашем сайте. Пожалуйста перейдите по ", login);
            msgBody.AppendFormat(@"<a href=""{0}"">этой ссылке</a> для подтверждения своего нового email-адреса.", urlToConfirm);
            msgBody.Append("<br />");
            msgBody.Append("<br />");
            msgBody.Append("Если Вы не регистрировались на нашем сайте, то можете проигнорировать это письмо.");
            msgBody.Append("<br /><br /><br />");
            msgBody.Append("Пожалуйста, не отвечайте на это письмо.");
            msgBody.Append("<br /><hr><br />");
            msgBody.Append("С уважением, команда портала TestOnline");

            switch (emailServer)
            {
                case FromEmailServer.gmail:
                    SendEmailViaGmail(toEmail, subject, msgBody.ToString());
                    break;
            }

            subject = ""; // я понимаю, что тот текст еще остается до чистки GC, но хотя бы ссылка на него не указывает
            msgBody.Clear();
        }

        /// <summary>
        /// Отправка письма с сервера почты gmail
        /// </summary>
        /// <param name="emailAdress">newEmail-адрес на который нужно отправить сообщение</param>
        /// <param name="subject">Заголовок письма</param>
        /// <param name="content">Тело письма</param>
        public static void SendEmailViaGmail(string emailAdress, string subject, string content)
        {
            string gmailAcc = WebConfigurationManager.AppSettings.Get("GmailAcc");
            string gmailAccPass = WebConfigurationManager.AppSettings.Get("GmailPass");

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(gmailAcc);
            msg.To.Add(new MailAddress(emailAdress));
            msg.Subject = subject;
            msg.Body = content;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;            
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(gmailAcc, gmailAccPass);
            client.Send(msg);

            content = string.Empty;
        }
    }
}
