using BlogEngineTK.Domain.Services;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BlogEngineTK.Domain.Services.Authorization
{
    /// <summary>
    /// Провайдер авторизации для администратора блога
    /// </summary>
    public class FormsAuthProvider: IAuthProvider
    {
        // Логин администратора
        private string login;

        // Хэш пароля администратора
        private string passwordHash;

        // Email-адрес администратора
        private string email;

        private Logger logger = LogManager.GetLogger("Auth");

        public FormsAuthProvider()
        {
            login = BlogSettings.Current.AdminLogin;
            passwordHash = BlogSettings.Current.PasswordHash;
            email = BlogSettings.Current.Email;
        }

        /// <summary>
        /// Инициализирует новый экземпляр (для юнит-тестов)
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <param name="password">Хэш пароля администратора</param>
        /// <param name="email">Email-адрес администратора</param>
        public FormsAuthProvider(string login, string password, string email)
        {
            this.login = login;
            this.passwordHash = password;
            this.email = email;            
        }

        public bool IsValidUser(string login, char[] password)
        {
            if (login.ToLower() == this.login.ToLower()
                && HashPassword.Verify(password, "", this.passwordHash))
            {
                if (HttpContext.Current != null) // Для юнит-тестов просто возвращаем true
                {                    
                    HttpContext.Current.Response.Cookies.Add(FormsAuthentication.GetAuthCookie(login, false));                    
                    logger.Info(string.Format("User: '{0}' authorized.", login));
                }
                Array.Clear(password, 0, password.Length); // Очищаем массив с паролем             
                return true;
            }

            return false;
        }

        public void Logout()
        {
            if (HttpContext.Current != null) // Для юнит-тестов просто возвращаем true
            {
                logger.Info(string.Format("User: '{0}' deauthorized.", HttpContext.Current.User.Identity.Name));
                FormsAuthentication.SignOut();
            }
        }

        public bool IsEmailExists(string email)
        {            
            return !string.IsNullOrEmpty(this.email) && email.ToLower() == this.email.ToLower();            
        }

        public void RemindPass(string email, string url)
        {
            // Создаем данные для отправки

            // Генерируем код потверждения
            string code = JsonConvert.SerializeObject(new { guid = Guid.NewGuid(), login = login });
            code = code.Replace('"', '\'');
            url = url + "?code=" + code;            

            char[] newPass = HashPassword.GeneratePassword(6); // Генерируем новый пароль длиной в 6 символов                      
            
            // Отправляем письмо с паролем и кодом подтверждения
            MailSender.SendEmailToNewPass(email, login, newPass, url, MailSender.FromEmailServer.gmail);
            logger.Warn(string.Format("Mail to recover access sended to email: '{0}'.", email));
                                    
            // Сохраняем новый пароль, код подтверждения и дату устаревания в хранилище данных
            string newPassHash = Convert.ToBase64String(HashPassword.HashPass(newPass, new byte[0]));   
            BlogSettings.Current.TurnOnPassRemind(newPassHash, code, DateTime.Now.AddDays(1));            
        }

        public bool IsCorrectCode(string code)
        {
            logger.Warn(string.Format("Attempt to recover access with input confirm code: '{0}'.", code));

            // Дополн. предвар. проверка кода
            try
            {
                var json = JsonConvert.DeserializeObject(code);
                if (json == null)
                {
                    throw new JsonReaderException(string.Format(
                        "Attempt failed. Deserialized object from input == null. Input confirm code: '{0}'.", code));
                }
            }
            catch (JsonReaderException e)
            {
                logger.Warn(string.Format("Attempt failed. Can't parse json from input code: '{0}'.", code));
                return false;
            }

            // Основная проверка кода
            if (code == BlogSettings.Current.ConfirmCode)
            {
                BlogSettings.Current.PassRemindSuccess();
                logger.Warn(string.Format("Attempt success. Valid input confirm code: '{0}'.", code));                
                return true;
            }
            else
            {
                logger.Warn(string.Format("Attempt failed. Input code dont't equal to confirm code at server. Input code: '{0}'.", code));
                return false;
            }
        }
    }
}