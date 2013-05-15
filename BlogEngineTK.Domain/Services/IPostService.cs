using BlogEngineTK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngineTK.Domain.Services
{
    public interface IPostService
    {
        /// <summary>
        /// Возвращает пост по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Post GetPost(int id);

        /// <summary>
        /// Возвращает посты для текущей страницы
        /// </summary>
        /// <param name="currentPage">Текущая страница</param>
        /// <param name="itemsPerPage">Кол-во постов на странице</param>
        IEnumerable<Post> GetPostsAtPage(int currentPage, int itemsPerPage);

        /// <summary>
        /// Возвращает информацию о кол-ве страниц
        /// </summary>
        /// <param name="currentPage">Текущая страница</param>
        /// <param name="itemsPerPage">Кол-во постов на странице</param>
        PagingInfo GetPagingInfo(int currentPage, int itemsPerPage);

        /// <summary>
        /// Возвращает информацию о предыдущей и следующей записи
        /// </summary>
        /// <param name="postId">Идентификатор текущей записи</param>
        PostPagingInfo GetPostPagingInfo(int postId);        
    }
}
