using BlogEngineTK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogEngineTK.WebUI.Models
{
    public class HomePostViewModel
    {
        /// <summary>
        /// Текущий (открытый пользователем) пост
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        /// Информация о пред. и след. записи (для генерирования перелистывания)
        /// </summary>
        public PostPagingInfo PagingInfo { get; set; }
    }
}