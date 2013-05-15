using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngineTK.Tests.Environment
{
    class Helpers
    {
        /// <summary>
        /// Проверяет входную модель на ошибки с помощью указанных в модели атрибутах.
        /// Используем этот метод для проверки модели, т.к. ASP.NET MVC валидирует модель только при HTTP-запросе
        /// </summary>
        public static bool IsValidModel<T>(T model)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(model, validationContext, validationResults, true);
        }
    }
}
