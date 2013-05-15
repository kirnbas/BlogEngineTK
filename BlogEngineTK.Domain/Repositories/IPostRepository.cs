using BlogEngineTK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngineTK.Domain.Repositories
{
    public interface IPostRepository
    {
        IQueryable<Post> Posts { get; }        

        /// <summary>
        /// Добавляет пост в хранилище данных
        /// </summary>
        void InsertPost(Post post);

        /// <summary>
        /// Обновляет пост по ID
        /// </summary>
        /// <returns>Возвращает true если обновление прошло успешно; в противном случае false</returns>
        void UpdatePost(Post post);

        /// <summary>
        /// Удаляет пост по ID
        /// </summary>
        /// <returns>Возвращает true если удаление прошло успешно; в противном случае false</returns>
        void DeletePost(int postId);
    }
}
