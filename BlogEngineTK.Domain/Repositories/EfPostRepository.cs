using BlogEngineTK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngineTK.Domain.Repositories
{
    public class EfPostRepository : IPostRepository
    {
        private EfDbContext context = new EfDbContext();

        public IQueryable<Post> Posts
        {
            get { return context.Posts; }
        }        

        public void InsertPost(Post post)
        {
            context.Posts.Add(post);
            context.SaveChanges();
        }

        public void UpdatePost(Post post)
        {
            Post updatedPost = context.Posts.Find(post.PostId);

            if (updatedPost != null)
            {
                updatedPost.Header = post.Header;
                updatedPost.ShortText = post.ShortText;
                updatedPost.FullText = post.FullText;
                updatedPost.RouteSegment = post.RouteSegment;

                context.SaveChanges();
            }
        }

        public void DeletePost(int postId)
        {
            Post deleted = context.Posts.Find(postId);

            if (deleted != null)
            {
                context.Posts.Remove(deleted);
                context.SaveChanges();
            }
        }
    }
}
