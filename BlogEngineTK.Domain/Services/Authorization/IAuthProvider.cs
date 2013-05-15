using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogEngineTK.Domain.Services.Authorization
{
    /// <summary>
    /// Предоставляет методы для авторизации пользователя
    /// </summary>
    public interface IAuthProvider
    {
        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="login">Введенный логин пользователя</param>
        /// <param name="password">Введенный пароль пользователя</param>
        /// <returns>Значение true, если авторизация прошла успешна</returns>
        bool IsValidUser(string login, char[] password);

        /// <summary>
        /// Деавторизация текущего пользователя
        /// </summary>
        void Logout();

        /// <summary>
        /// Существует ли пользователь с таким email-адресом в хранилище данных
        /// </summary>
        bool IsEmailExists(string email);

        /// <summary>
        /// Отправляет письмо для восстановление доступа
        /// </summary>
        /// <param name="url">URL на action, который будет вызван для получения кода подтверждения</param>
        void RemindPass(string email, string url);

        /// <summary>
        /// Проверка кода подтверждения, если он корректен, то новый пароль будет принят
        /// </summary>
        bool IsCorrectCode(string code);
    }
}