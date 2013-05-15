using BlogEngineTK.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogEngineTK.WebUI.Areas.Admin.Models
{
    public class SettingsIndexViewModel
    {
        /// <summary>
        /// Основные настройки блога
        /// </summary>
        public BlogSettings Settings { get; set; }

        // Поля для изменения пароля администратора

        [StringLength(50, MinimumLength = 4, ErrorMessage = "Мин. длина: 4 символа, макс. длина: 50 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string CurrentPassword { get; set; }

        [StringLength(50, MinimumLength = 4, ErrorMessage = "Мин. длина: 4 символа, макс. длина: 50 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [Compare("ConfirmNewPassword", ErrorMessage = " ")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmNewPassword { get; set; }
    }
}