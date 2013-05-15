using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace BlogEngineTK.WebUI.Infrastructure.ModelBinders
{
    public class SessionStorage
    {
        /// <summary>
        /// Возвращает единственный экземпляр класса (в рамках сессии)
        /// </summary>
        public static SessionStorage Current 
        {
            get
            {
                string key = "_sessionstorage";

                // Для юнит-тестирования возвратим просто данный экземпляр
                if (HttpContext.Current == null || HttpContext.Current.Session == null)
                {
                    return new SessionStorage();
                }

                SessionStorage ss = HttpContext.Current.Session[key] as SessionStorage;

                if (ss == null)
                {
                    ss = new SessionStorage();
                    HttpContext.Current.Session[key] = ss;
                }

                return ss;
            }
        }

        /// <summary>
        /// Код генерируемый в классе Captcha для сравнения с вводимым
        /// </summary>
        public string CaptchaCode { get; set; }

        /// <summary>
        /// Попыток неудачного входа (используется для блокировки после определенного значения)
        /// </summary>
        public int UnsucLoginAttempts { get; set; }

        /// <summary>
        /// Попыток неудачного ввода кода потверждения (используется для блокировки после определенного значения)
        /// </summary>
        public int UnsucNumOfConfirmCode { get; set; }

        /// <summary>
        /// Т.к. нужен только единственный экземпляр в рамках одной сессии
        /// </summary>
        private SessionStorage()
        {
        }
    }
}