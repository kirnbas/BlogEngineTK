using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngineTK.Domain.Entities
{
    public class Tag
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// Название тэга
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Язык для которого был создан тэг
        /// </summary>
        public Language Language { get; set; }
    }
}
