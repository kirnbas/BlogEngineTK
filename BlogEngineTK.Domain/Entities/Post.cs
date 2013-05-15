using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BlogEngineTK.Domain.Entities
{
    public class Post
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Сегмент URL: "~/PostId/RouteSegment" (для красивых URL)
        /// </summary>
        [Display(Name = "Последний сегмент в URL-адресе")]
        [StringLength(50, ErrorMessage = "Максимальная длина: 50 символов")]
        [RegularExpression("^([a-zA-Z0-9]+)(-[a-zA-Z0-9]+)*$", ErrorMessage = "Разрешается использовать только символы английского алфавита, цифры и '-'")]
        public string RouteSegment { get; set; }

        /// <summary>
        /// Дата создания поста
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Название, заголовок поста
        /// </summary>
        [Display(Name = "Заголовок")]
        [Required(ErrorMessage = "Пожалуйста, укажите заголовок поста")]
        [MaxLength(256, ErrorMessage = "Максимальная длина: 256 символов")]
        public string Header { get; set; }

        /// <summary>
        /// Короткая версия поста
        /// </summary>
        [AllowHtml]
        [Required(ErrorMessage = "Пожалуйста, введите короткую версию поста")]
        [StringLength(10000, ErrorMessage = "Вы превысили лимит в 10 000 символов (посмотрите HTML код текста).")]
        [Display(Name = "Короткая версия")]
        public string ShortText { get; set; }

        /// <summary>
        /// Полная версия поста
        /// </summary>
        [AllowHtml]
        [Required(ErrorMessage = "Пожалуйста, введите полную версию поста")]
        [StringLength(200000, ErrorMessage = "Вы превысили лимит в 200 000 символов (посмотрите HTML код текста).")]
        [Display(Name = "Полная версия")]
        public string FullText { get; set; }

        /// <summary>
        /// Кол-во комментариев
        /// TODO mb otkazatsya ot takogo svoistva
        /// </summary>
        public int CommentsNumber { get; set; }

        /// <summary>
        /// Тэги поста
        /// </summary>
        public IEnumerable<Tag> Tags { get; set; }

        /// <summary>
        /// Язык на котором написан пост
        /// </summary>
        public Language Language { get; set; }
    }
}
