using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogEngineTK.Domain.Entities
{
    public class PostPagingInfo
    {
        /// <summary>
        /// Предыдущий пост
        /// </summary>
        public Post Previous { get; set; }

        /// <summary>
        /// Следующий пост
        /// </summary>
        public Post Next { get; set; }
    }
}