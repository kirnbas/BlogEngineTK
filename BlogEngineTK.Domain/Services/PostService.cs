using BlogEngineTK.Domain.Entities;
using BlogEngineTK.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngineTK.Domain.Services
{
    public class PostService : IPostService
    {
        private IPostRepository repository;

        public PostService(IPostRepository repository)
        {
            this.repository = repository; // для удобного юнит-тестирования
        }

        public IEnumerable<Post> GetPostsAtPage(int page, int itemsPerPage)
        {
            return repository.Posts
                .OrderByDescending(x => x.CreatedDate)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage);
        }

        public PagingInfo GetPagingInfo(int currentPage, int itemsPerPage)
        {
            return new PagingInfo()
            {
                TotalItems = repository.Posts.Count(),
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage
            };
        }

        public Post GetPost(int id)
        {
            return repository.Posts.FirstOrDefault(x => x.PostId == id);
        }

        public PostPagingInfo GetPostPagingInfo(int postId)
        {
            PostPagingInfo pagingInfo = new PostPagingInfo();

            Post previous = repository.Posts.Where(x => x.PostId < postId).OrderByDescending(x => x.PostId).FirstOrDefault();
            Post next = repository.Posts.Where(x => x.PostId > postId).OrderBy(x => x.PostId).FirstOrDefault();

            pagingInfo.Previous = previous;
            pagingInfo.Next = next;

            return pagingInfo;
        }
    }
}
