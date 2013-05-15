using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BlogEngineTK.Domain.Services
{
    /// <summary>
    /// Метод для хэширования пароля, проверки и генерирования пароля
    /// </summary>
    public class HashPassword
    {
        /// <summary>
        /// Проверяет, что указанный пароль в открытом виде соответствует
        /// синхропосылке и хэшу, сохраненным в объекте.
        /// </summary>
        /// <param name="clearText">Пароль в открытом виде.</param>
        /// <param name="salt">Синхропосылка</param>
        /// <param name="hashFromDataStore">Хэш пароля пользователя с хранилища данных</param>
        /// <returns>Метод возвращает true, если пароль соответствует
        /// синхропосылке и хэшу, и false - в противном случае.</returns>
        public static bool Verify(char[] clearText1, string salt, string hashFromDataStore)
        {
            byte[] hash = HashPass(clearText1, Convert.FromBase64String(salt));
            byte[] hash2 = Convert.FromBase64String(hashFromDataStore);

            if (hash.Length == hash2.Length)
            {
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != hash2[i])
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Хэширует указанный пароль в открытом в виде в комбинации с
        /// синхропосылкой, находящейся в поле _salt.
        /// </summary>
        /// <param name="clearText">Пароль в открытом виде.</param>
        /// <param name="salt">Синхропосылка</param>
        /// <returns>Хэш пароля</returns>
        public static byte[] HashPass(char[] clearText, byte[] salt)
        {
            Encoding utf8 = Encoding.UTF8;
            byte[] hash;

            // создаем рабочий массив достаточного размера
            byte[] data = new byte[salt.Length + utf8.GetMaxByteCount(clearText.Length)];

            try
            {
                // копируем синхропосылку в рабочий массив
                Array.Copy(salt, 0, data, 0, salt.Length);

                // копируем пароль в рабочий массив, преобразуя его в UTF-8
                int byteCount = utf8.GetBytes(clearText, 0, clearText.Length,
                  data, salt.Length);

                // хэшируем данные массива
                using (HashAlgorithm alg = new SHA256Managed())
                {
                    hash = alg.ComputeHash(data, 0, salt.Length + byteCount);
                }
            }
            finally
            {
                // Очищаем массивы в конце работы, чтобы избежать утечки открытого пароля
                Array.Clear(data, 0, data.Length);
                Array.Clear(clearText, 0, clearText.Length);
            }

            return hash;
        }

        /// <summary>
        /// Генерирует случайный пароль
        /// </summary>
        /// <param name="size">Длина пароля</param>
        /// <returns>Сгенерированный пароль в виде массива символов</returns>
        public static char[] GeneratePassword(int size)
        {
            char[] randomBytes = new char[size];
            Random random = new Random();

            for (int i = 0; i < size; i++)
            {
                if (random.Next(3) != 1)
                {
                    randomBytes[i] = (random.Next(2) == 1) ?
                                    (char)random.Next(65, 90) :
                                    (char)random.Next(97, 122);
                }
                else
                {
                    randomBytes[i] = (char)random.Next(48, 57);
                }
            }

            return randomBytes;
        }
    }
}