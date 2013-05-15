using BlogEngineTK.Domain;
using BlogEngineTK.Domain.Services;
using BlogEngineTK.Domain.Services.Authorization;
using BlogEngineTK.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogEngineTK.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IAuthProvider authProvider;

        public AdminController(IAuthProvider authProvider)
        {
            this.authProvider = authProvider;
        }

        /// <summary>
        /// Выводит панель управления администратора
        /// </summary>
        [HttpGet]        
        public ActionResult Settings(string returnUrl)
        {
            // Получаем текущие настройки для установки контролов в панели управления
            BlogSettings settings = BlogSettings.ShallowCopy;
            SettingsIndexViewModel model = new SettingsIndexViewModel
            {
                Settings = settings
            };            

            return View(model);
        }

        /// <summary>
        /// Сохраняет настройки указанные в панели управления
        /// </summary>
        /// <param name="model">Настройки с панели управления</param>
        [HttpPost]
        [ActionName("Settings")]
        public ActionResult SettingsPost(SettingsIndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.NewPassword) && string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Для смены пароля нужно указать текущий пароль");
                }
                else if (string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("NewPassword", "Вы не указали новый пароль");
                }               
            }

            string newPassHash = null; // если производится корректная смена пароля, то ниже получит значение

            if (ModelState.IsValid && !string.IsNullOrEmpty(model.NewPassword))
            {
                // Берем логин с сервера, потому что на клиенте он мог измениться
                if (authProvider.IsValidUser(BlogSettings.Current.AdminLogin, model.CurrentPassword.ToCharArray()))
	            {
                    // Получаем хэш пароля
                    newPassHash = Convert.ToBase64String(
                        HashPassword.HashPass(model.NewPassword.ToCharArray(), new byte[0]));
	            }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "Указанный пароль не соответствует текущему");
                }
            }            

            if (ModelState.IsValid)
            {
                // Сохранение новых настроек
                BlogSettings.Current.ChangeSettings(model.Settings, newPassHash, false);
                BlogEngineTK.WebUI.MvcApplication.ClearAppCache();
            }            

            return View(model);
        }
    }
}
