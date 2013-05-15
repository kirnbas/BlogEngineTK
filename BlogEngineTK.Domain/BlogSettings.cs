using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using BlogEngineTK.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.IO;

namespace BlogEngineTK.Domain
{
    /// <summary>
    /// Параметры блога (serialized in JSON file).
    /// Multhreading: к данному классу м.б. одновременное обращение с главного 
    /// и фонового потока (только одного в момент времени, блокируется lock'erom), 
    /// но т.к. поля являются static, которые инициализируется при запуске программы,
    /// вероятность рассинхронизации между потоками ничтожно мала.
    /// </summary>
    public class BlogSettings
    {
        private const string FILENAME = "BlogSettings.json";

        #region Serializable properties (value types & main settings)        

        #region Опции во вкладке "Администратор"

        /// <summary>
        /// Логин администратора
        /// </summary>        
        [Required(ErrorMessage = "Пожалуйста, укажите логин")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Мин. длина: 4 символа, макс. длина: 50 символов")]
        [Display(Name = "Логин")]
        public string AdminLogin { get; set; }

        /// <summary>
        /// Email-адрес администратора (для восстановления доступа)
        /// </summary>
        [DataType(DataType.EmailAddress)]        
        [EmailAddress(ErrorMessage = "Пожалуйста, введите корректный почтовый адрес")]
        [Display(Name = "Email-адрес (для восстановления доступа)")]
        [MaxLength(50, ErrorMessage = "Максимальная длина 50 символов")]
        public string Email { get; set; } 

        /// <summary>
        /// Хэш-код пароля администратора
        /// </summary>
        public string PasswordHash { get; set; }

        ///// <summary>
        ///// Имя администратора (возможно включая Ф.И.О.)
        ///// </summary>        
        //public string AdminFullName { get; set; }

        #endregion

        #region Password Reminder

        /// <summary>
        /// Хэш нового пароля
        /// </summary>
        public string NewPassHash { get; set; }

        /// <summary>
        /// Код подтверждения для установки нового пароля в качестве основного
        /// </summary>
        public string ConfirmCode { get; set; }

        /// <summary>
        /// Дата и время устаревания подтверждения нового пароля
        /// </summary>
        public DateTime NewPassExpired { get; set; }

        #endregion

        #region Опции во вкладке "Общее"

        /// <summary>
        /// Цветовая схема блога
        /// </summary>
        [Display(Name = "Цветовая схема")]
        public ColorScheme Color { get; set; }

        /// <summary>
        /// Включать ли виджет "Об авторе (администраторе)" (в боковой панели)
        /// </summary>
        [Display(Name = "Информация об авторе")]
        public bool IsAboutAuthorOn { get; set; }

        /// <summary>
        /// Включать ли виджет "Популярные посты" 
        /// </summary>
        [Display(Name = "Популярные посты")]
        public bool IsPopularPostsOn { get; set; }

        /// <summary>
        /// Включать ли виджет "Тэги" 
        /// </summary>
        [Display(Name = "Тэги")]
        public bool IsTagsWidgetOn { get; set; }

        /// <summary>
        /// Включать ли виджет "Архив постов" 
        /// </summary>
        [Display(Name = "Архив постов")]
        public bool IsPostsArchiveOn { get; set; }

        /// <summary>
        /// Включать ли виджет "Голосование" 
        /// </summary>
        [Display(Name = "Голосование")]
        public bool IsPollWidgetOn { get; set; }

        /// <summary>
        /// Включен ли блог (если он откл., то доступ есть только к панеле администратора)
        /// </summary>
        [Display(Name = "Блог включен")]
        public bool IsBlogOn { get; set; }

        /// <summary>
        /// Имя блога
        /// </summary>
        [StringLength(30, ErrorMessage = "Макс. длина: 30 символов")]
        [Display(Name = "Название блога")]
        public string BlogName { get; set; }

        /// <summary>
        /// Подзаголовок блога
        /// </summary>
        [StringLength(30, ErrorMessage = "Макс. длина: 30 символов")]
        [Display(Name = "Подзаголовок блога")]
        public string BlogSubheader { get; set; }

        /// <summary>
        /// Копирайт администратора блога
        /// </summary>
        [StringLength(30, ErrorMessage = "Макс. длина: 30 символов")]
        [Display(Name = "Текст в нижней части блога")]
        public string FooterText { get; set; }

        #endregion

        #region Опции во вкладке "Посты"

        /// <summary>
        /// Макс. кол-во постов на странице
        /// </summary>
        [Display(Name = "Максимальное кол-во постов на странице")]
        [Required(ErrorMessage = "Пожалуйста введите значение (1-20)")]        
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Значение должно быть числом: 1-20")]
        [Range(1, 20, ErrorMessage = "Значение должно быть числом: 1-20")]
        public int MaxPostsAtPage { get; set; }

        #endregion

        #endregion

        /// <summary>
        /// .NET guarantees thread-safe initialization for static readonly fields
        /// </summary>
        private static readonly BlogSettings current = new BlogSettings();

        /// <summary>
        /// Return main instance which used to read and change blog settings
        /// </summary>
        public static BlogSettings Current 
        {
            get { return current; }
        }

        /// <summary>
        /// Return instance which is shallow copy of Current instance
        /// and contains only serializable instance fields (Value-type)
        /// </summary>
        public static BlogSettings ShallowCopy
        {
            get { return (BlogSettings)Current.MemberwiseClone(); }
        }

        private static readonly Logger logger = LogManager.GetLogger("Auth");
        private static Timer singleTimer;

        /// <summary>
        /// Чтение настроек с JSON файла (должно вызываться при первом запуске приложения из Global.asax)
        /// </summary>
        public void ReadSettings()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + FILENAME;
            bool defaultSets = true;

            if (File.Exists(path))
            {
                BlogSettings settings = null;

                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        using (JsonReader jr = new JsonTextReader(sr))
                        {
                            settings = (BlogSettings)new JsonSerializer().Deserialize(jr, this.GetType());                            
                        }
                    }
                }
                // Запускаем после чтения файла, чтобы не держать хэндлер файла, который далее пригодятся
                ChangeSettings(settings, null, true);                

                // Если данные PassRemind с прочитанного Json не null,
                // то запуститься таймер по очистки устаревшей попытки восстановления пароля
                ClearExpiredPass();

                defaultSets = settings == null;
            }

            if (defaultSets) // Начальные настройки (при первом запуске или неудачном чтении с файла)
            {                
                AdminLogin = "test";
                PasswordHash = "n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=";
                Email = "";
                MaxPostsAtPage = 2;

                IsAboutAuthorOn = IsPopularPostsOn = IsPostsArchiveOn
                    = IsTagsWidgetOn = IsBlogOn = true;
                Color = ColorScheme.Blue;
                BlogName = "Название блога";
                BlogSubheader = "Подзаголовок блога";
                FooterText = "Текст в нижней части блога";
                SaveSettings();
            }
        }

        /// <summary>
        /// Сохранение настроек в JSON файл
        /// </summary>
        public void SaveSettings()
        {
            using (FileStream fs = File.Open(AppDomain.CurrentDomain.BaseDirectory + FILENAME, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    using (JsonWriter jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;

                        new JsonSerializer().Serialize(jw, this);
                    }
                }
            }
        }

        /// <summary>
        /// Включает механизм восстановления доступа по коду подтверждения 
        /// (данный метод может выполняться только одним потоком в момент времени)
        /// </summary>
        /// <param name="newPassHash">Хэш нового пароля</param>
        /// <param name="confirmCode">Код подтверждения для установки нового пароля в качестве основного</param>
        /// <param name="expired">Дата и время устаревания подтверждения нового пароля</param>
        public void TurnOnPassRemind(string newPassHash, string confirmCode, DateTime expired)
        {
            if (!string.IsNullOrEmpty(newPassHash))
            {
                NewPassHash = newPassHash;
                ConfirmCode = confirmCode;
                NewPassExpired = expired;
                SaveSettings();

                ClearExpiredPass();
            }
        }

        /// <summary>
        /// Метод вызываемый для очистки устаревшего нового пароля.
        /// Выполнен через System.Threading.Timer как не лучший, но простой вариант.
        /// </summary>        
        private void ClearExpiredPass()
        {
            if (!string.IsNullOrEmpty(NewPassHash))
            {
                // Присваивание должно быть потокобезопасным, т.к. метод вызывается либо при запуске приложения
                // и static поле singleTimer получает значение.
                // Либо вызывается в процессе работы с FormsAuthProvider, но только одним потоком в момент времени, 
                // т.к. в месте вызова данного метода блок кода обернут в lock'er.
                singleTimer = new System.Threading.Timer(
                    delegate
                    {                        
                        logger.Info("TimerHandler in ClearExpiredPass() was called.");

                        if (string.IsNullOrEmpty(NewPassHash))
                        {
                            logger.Warn("TimerHandler in ClearExpiredPass(). Timer disposed, because hash of new password == null.");
                            singleTimer.Dispose();
                        }
                        else if (DateTime.Now > NewPassExpired)
                        {
                            NewPassHash = null;
                            ConfirmCode = null;
                            NewPassExpired = default(DateTime);
                            SaveSettings();
                            logger.Warn("TimerHandler in ClearExpiredPass(). Time limit has expired. Operation 'PassRemind' cancelled.");
                            singleTimer.Dispose();
                        }
                        else
                        {
                            logger.Info("TimerHandler in ClearExpiredPass(). Time limit doesn't expired.");
                        }
                    },
                    null,
                    0,
                    1000 * 60 * 60); // Данный анонимный метод будет вызываться каждый час
            }
        }

        /// <summary>
        /// Вызывается при успешной проверке кода подтверждения для принятия нового пароля как основного
        /// </summary>
        public void PassRemindSuccess()
        {
            if (!string.IsNullOrEmpty(NewPassHash) && !string.IsNullOrEmpty(ConfirmCode) && DateTime.Now < NewPassExpired )
            {
                PasswordHash = NewPassHash; // Устанавливаем новый основной пароль

                // Стираем данные для восстановления доступа
                NewPassHash = null;
                ConfirmCode = null;
                NewPassExpired = default(DateTime);
                SaveSettings();
                logger.Warn("Attempt success. New password installed as main.");
            }
        }

        /// <summary>
        /// Сохраняет измененные настройки в свойстве Current и файле (вызывается из SettingsController)
        /// </summary>
        /// <param name="newSettings">Новые настройки блога</param>
        /// <param name="newPasswordHash">Хэш нового пароля (null, если смены пароля не происходит)</param>
        /// <param name="isNewSettingsFull">Указаны ли все параметры в новых настройках блога, с SettingsController 
        /// приходят не все нужные данные (например, отсутствует Hash текущего пароля и данные PassRemind)</param>
        public void ChangeSettings(BlogSettings newSettings, string newPasswordHash, bool isNewSettingsFull)
        {
            if (newSettings != null)
            {
                this.AdminLogin = newSettings.AdminLogin;
                this.Email = newSettings.Email;
                
                if (!string.IsNullOrEmpty(newPasswordHash))
                {
                    this.PasswordHash = newPasswordHash;
                }                

                this.BlogName = newSettings.BlogName;
                this.BlogSubheader = newSettings.BlogSubheader;
                this.FooterText = newSettings.FooterText;
                this.Color = newSettings.Color;
                this.MaxPostsAtPage = newSettings.MaxPostsAtPage;

                this.IsAboutAuthorOn = newSettings.IsAboutAuthorOn;
                this.IsPopularPostsOn = newSettings.IsPopularPostsOn;
                this.IsPostsArchiveOn = newSettings.IsPostsArchiveOn;
                this.IsTagsWidgetOn = newSettings.IsTagsWidgetOn;
                this.IsPollWidgetOn = newSettings.IsPollWidgetOn;
                this.IsBlogOn = newSettings.IsBlogOn;                

                if (isNewSettingsFull)
                {
                    this.PasswordHash = newSettings.PasswordHash;
                    this.NewPassHash = newSettings.NewPassHash;
                    this.NewPassExpired = newSettings.NewPassExpired;
                    this.ConfirmCode = newSettings.ConfirmCode;
                }

                this.SaveSettings();
            }            
        }
    }
}