using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BlogEngineTK.WebUI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, укажите имя")]
        [Display(Name= "Логин")]
        [MaxLength(50, ErrorMessage = "Максимальная длина 50 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [MaxLength(50, ErrorMessage = "Максимальная длина 50 символов")]
        public string Password { get; set; }

        /// <summary>
        /// Введенный пользователем код с капчи
        /// </summary>
        [MaxLength(50, ErrorMessage = "Максимальная длина 50 символов")]
        public string Captcha { get; set; }
    }
}