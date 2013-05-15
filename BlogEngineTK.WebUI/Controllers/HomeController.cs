using BlogEngineTK.Domain.Entities;
using BlogEngineTK.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using BlogEngineTK.WebUI.Infrastructure.HtmlHelpers;
using BlogEngineTK.Domain.Services;
using NLog;
using BlogEngineTK.WebUI.Infrastructure.Filters;
using BlogEngineTK.Domain;

namespace BlogEngineTK.WebUI.Controllers
{
    [BlogOff]
    public class HomeController : Controller
    {        
        public int PageSize { get; set; }

        private IPostService postService;

        public HomeController(IPostService service)
        {
            this.PageSize = BlogSettings.Current.MaxPostsAtPage;
            this.postService = service;
        }
                
        /// <summary>
        /// Отображает посты текущей страницы и Paginator (листание страниц)
        /// </summary>
        /// <param name="page">Текущая страница</param>  
        [OutputCache(Duration = 60, VaryByCustom = BlogEngineTK.WebUI.MvcApplication.VARYBYAUTH)]
        public ViewResult Index(int page = 1)
        {
            IEnumerable<Post> posts = postService.GetPostsAtPage(page, PageSize);
            PagingInfo pagingInfo = postService.GetPagingInfo(page, PageSize);
            
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                Posts = posts,
                PagingInfo = pagingInfo
            };
            return View(model);
        }
        
        /// <summary>
        /// Отображает полную версию поста и PostPaginator (листание на пред. и след. запись)
        /// </summary>
        /// <param name="postId">Идентификатор поста, который нужно отобразить</param>
        [OutputCache(Duration = 30, VaryByParam = "postId", VaryByCustom = BlogEngineTK.WebUI.MvcApplication.VARYBYAUTH)]
        public ActionResult Post(int postId)
        {
            Post currentPost = postService.GetPost(postId);

            if (currentPost == null)
            {
                string url = "test";
                if (Url != null) // при юнит-тестирование будет равняться null
                {
                    url = Url.Home();
                }

                return Redirect(url);
            }

            PostPagingInfo pagingInfo = postService.GetPostPagingInfo(postId);

            HomePostViewModel model = new HomePostViewModel
            {
                PagingInfo = pagingInfo,
                Post = currentPost
            };
            TempData["fullpost"] = 1;
            return View(model);
        }
    }
}
