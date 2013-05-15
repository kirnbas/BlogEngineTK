using BlogEngineTK.Domain;
using BlogEngineTK.Domain.Services.Authorization;
using BlogEngineTK.WebUI.Infrastructure;
using BlogEngineTK.WebUI.Infrastructure.Filters;
using BlogEngineTK.WebUI.Infrastructure.ModelBinders;
using BlogEngineTK.WebUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BlogEngineTK.WebUI.Controllers
{    
    public class AccountController : Controller
    {
        private IAuthProvider authProvider;

        public AccountController(IAuthProvider authProvider)
        {
            this.authProvider = authProvider; // для удобного юнит-тестирования
        }

        /// <summary>
        /// Отображает форму входа (ввод логина и пароля)
        /// </summary>
        [RedirectAuthAttribute]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Обрабатывает данные с формы входа
        /// </summary>
        /// <param name="model">Данные с формы</param>
        /// <param name="returnUrl">URL на который будет произведен переход, при успешной авторизации</param>
        /// <param name="storage">Хранилище данных в рамках сессии, для проверки капчи (как параметр для удобного юнит-тестирования)</param>
        /// <returns>Возвращает RedirectResult если входные данные корректны; в противном случае возвращает ViewResult</returns>
        [HttpPost]
        [RedirectAuthAttribute]
        public ActionResult Login(LoginViewModel model, string returnUrl, SessionStorage storage)
        {
            ValidateCaptcha(model.Captcha, storage.CaptchaCode);

            if (ModelState.IsValid)
            {
                if (authProvider.IsValidUser(model.Login, model.Password.ToCharArray()))
                {
                    storage.UnsucLoginAttempts = 0;
                    storage.CaptchaCode = null;
                    return Redirect(returnUrl + "?sign=true" ?? Url.Action("Index", "Home", new { sign = "true" }));
                }
                else
                {
                    storage.UnsucLoginAttempts++;
                    ModelState.AddModelError("", "Неправильный логин и/или пароль");
                    ModelState.AddModelError("login", "");
                    ModelState.AddModelError("password", "");
                    return View(model);
                }
            }
            else
            {
                storage.UnsucLoginAttempts++;
                return View(model);
            }
        }

        /// <summary>
        /// Выход пользователя из системы авторизации
        /// </summary>
        /// <param name="returnUrl">URL на который будет произведен переход после выхода</param>
        public ActionResult Logout(string returnUrl)
        {
            authProvider.Logout();
            return Redirect(returnUrl + "?signOut=true" ?? Url.Action("Index", "Home", new { signOut = "true" }));
        }

        /// <summary>
        /// Отображает форму ввода email-адреса для восстановления пароля
        /// </summary>
        [RedirectAuthAttribute]
        public ActionResult PassReminder(string returnUrl)
        {
            return View();
        }

        /// <summary>
        /// Проверяет введенный email, если он корректен отправляет письмо с паролем и потверждающим линком
        /// </summary>
        /// <param name="model">Данные с формы</param>
        /// <param name="storage">Хранилище данных в рамках сессии, для проверки капчи (как параметр для удобного юнит-тестирования)</param>
        /// <returns>Возвращает RedirectResult если входные данные корректны; в противном случае возвращает ViewResult</returns>
        [HttpPost]
        [RedirectAuthAttribute]
        public ActionResult PassReminder(PassReminderViewModel model, string returnUrl, SessionStorage storage)
        {
            ValidateCaptcha(model.Captcha, storage.CaptchaCode);

            if (ModelState.IsValid)
            {
                if (authProvider.IsEmailExists(model.Email))
                {
                    storage.UnsucLoginAttempts = 0;
                    storage.CaptchaCode = null;

                    Task.Factory.StartNew(() =>
                    {
                        // Блокируем общий для главного и фонового потока ресурс
                        // deadlock не ожидается, потому как пока это единственный монитор
                        // + выполниться этот процесс должен оч. быстро.
                        lock (BlogSettings.Current)
                        {
                            string url = "";
                            if (Request != null) // при юнит-тестировании пропускаем этот блок
                            {
                                url = Request.Url.Authority + Url.Action("ConfirmNewPass");
                            }
                            authProvider.RemindPass(model.Email, url);
                        }
                    });

                    // Перенаправляем пользователя с сообщением об отправлении письма (almost POST/Redirect/GET pattern)
                    // и продолжим обработку запроса (отправку письма) в отдельном потоке (чуть выше)

                    string redirectUrl = "mock";
                    if (Url != null) // т.к. объект Url == null при юнит-тестировании
                    {
                        redirectUrl = Url.Action("PassReminder", new { email = model.Email, returnUrl = returnUrl });
                    }

                    TempData["msgSended"] = true;
                    return Redirect(redirectUrl);
                }
                else
                {
                    storage.UnsucLoginAttempts++;
                    ModelState.AddModelError("email", "Указанный email-адрес не зарегистрирован");
                    return View(model);
                }
            }
            else
            {
                storage.UnsucLoginAttempts++;
                return View(model);
            }
        }

        /// <summary>
        /// Производит валидацию капча-кода, и если она не проходи добавляет ошибку в ModelState
        /// </summary>
        /// <param name="captchaCodeFromUser">Введенный пользователем код с капчи</param>
        /// <param name="captcha">Сгенерированная капча на сервере</param>
        private void ValidateCaptcha(string captchaCodeFromUser, string captcha)
        {
            if (captcha != null
                && (string.IsNullOrEmpty(captchaCodeFromUser) || captcha.ToLower() != captchaCodeFromUser.ToLower()))
            {
                ModelState.AddModelError("captcha", "Вы ввели неправильный код с изображения");
            }
        }

        /// <summary>
        /// Принимает код потверждения нового пароля, проверяет и обрабатывает запрос
        /// </summary>
        /// <param name="code">Код потверждения</param>
        /// <returns>Возвращает ViewResult если входные данные корректны; в противном случае возвращает HttpNotFound</returns>
        public ActionResult ConfirmNewPass(string code, SessionStorage storage)
        {
            if (storage.UnsucNumOfConfirmCode >= 3) // Простая мера защиты от брута кода потверждения (действует только в рамках сессии)
            {
                return HttpNotFound();
            }

            if (authProvider.IsCorrectCode(code))
            {
                storage.UnsucNumOfConfirmCode = 0;
                return View();
            }
            else
            {
                storage.UnsucNumOfConfirmCode++;
                return HttpNotFound();
            }
        }
    }
}
