using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogEngineTK.WebUI.Models
{
    public class PassReminderViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Пожалуйста, введите ваш почтовый адрес")]
        [Display(Name = "Email-адрес")]
        [EmailAddress(ErrorMessage = "Пожалуйста, введите корректный почтовый адрес")]
        [MaxLength(50, ErrorMessage = "Максимальная длина 50 символов")]
        public string Email { get; set; }

        /// <summary>
        /// Введенный пользователем код с капчи
        /// </summary>
        [MaxLength(50, ErrorMessage = "Максимальная длина 50 символов")]
        public string Captcha { get; set; }
    }
}