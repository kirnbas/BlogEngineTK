using BlogEngineTK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogEngineTK.WebUI.Models
{
    public class HomeIndexViewModel
    {
        /// <summary>
        /// Посты на текущей странице
        /// </summary>
        public IEnumerable<Post> Posts { get; set; }

        /// <summary>
        /// Информация о страницах в блоге (для генерирования перелистывания)
        /// </summary>
        public PagingInfo PagingInfo { get; set; }
    }
}