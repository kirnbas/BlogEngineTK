using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogEngineTK.WebUI.Controllers;
using System.Collections.Generic;
using Moq;
using System.Linq;
using BlogEngineTK.Domain.Entities;
using System.Web.Mvc;
using BlogEngineTK.WebUI.Models;
using BlogEngineTK.Domain.Repositories;
using BlogEngineTK.Domain.Services;

namespace BlogEngineTK.Tests.UnitTests
{
    /// <summary>
    /// Тестирование HomeController (WebUI) 
    /// и исп-мого им PostService (Domain)
    /// </summary>
    [TestClass]
    public class HomeCntPostSrvcTest
    {
        [TestMethod]
        public void Can_Return_Posts_At_Page()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            IPostService service = new PostService(repository);
            HomeController target = new HomeController(service);
            target.PageSize = 4;

            // Act
            IEnumerable<Post> result0 = ((HomeIndexViewModel)target.Index(-1).Model).Posts.ToArray();
            IEnumerable<Post> result1 = ((HomeIndexViewModel)target.Index().Model).Posts.ToArray();
            IEnumerable<Post> result2 = ((HomeIndexViewModel)target.Index(2).Model).Posts.ToArray();
            IEnumerable<Post> result3 = ((HomeIndexViewModel)target.Index(3).Model).Posts.ToArray();
            IEnumerable<Post> result4 = ((HomeIndexViewModel)target.Index(4).Model).Posts.ToArray();

            // Assert            
            Assert.IsTrue(result0.Count() == 4);
            Assert.IsTrue(result1.Count() == 4);
            Assert.IsTrue(result2.Count() == 4);
            Assert.IsTrue(result3.Count() == 2);
            Assert.IsTrue(result4.Count() == 0);
        }

        [TestMethod]
        public void Can_Return_Post_At_Id()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            IPostService service = new PostService(repository);
            HomeController target = new HomeController(service);
            Post expectedPost = repository.Posts.First(x => x.PostId == 1);

            // Act
            Post result1 = ((HomePostViewModel)((ViewResult)target.Post(1)).Model).Post;
            Post result2 = ((HomePostViewModel)((ViewResult)target.Post(2)).Model).Post;
            Post result3 = ((HomePostViewModel)((ViewResult)target.Post(MockPostRepository.STDN)).Model).Post;

            // Assert            
            Assert.IsTrue(((ViewResult)target.Post(1)).TempData.ContainsKey("fullpost"));
            Assert.IsTrue(result1.PostId == 1);
            Assert.AreSame(expectedPost, result1);
            Assert.IsTrue(result2.PostId == 2);
            Assert.IsTrue(result3.PostId == MockPostRepository.STDN);
        }

        [TestMethod]
        public void Cannot_Return_Post_At_Id0()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            IPostService service = new PostService(repository);
            HomeController target = new HomeController(service);
            Post expectedPost = repository.Posts.First(x => x.PostId == 1);

            // Act
            ActionResult result = target.Post(0);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        [TestMethod]
        public void Cannot_Return_Post_At_Not_Exists_Id()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            IPostService service = new PostService(repository);
            HomeController target = new HomeController(service);
            Post expectedPost = repository.Posts.First(x => x.PostId == 1);

            // Act
            ActionResult result = target.Post(MockPostRepository.STDN + 1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            IPostService service = new PostService(repository);
            HomeController target = new HomeController(service);
            target.PageSize = 4;

            // Act
            PagingInfo result = ((HomeIndexViewModel)target.Index(3).Model).PagingInfo;       

            // Assert            

            Assert.IsTrue(result.CurrentPage == 3);
            Assert.IsTrue(result.ItemsPerPage == target.PageSize);
            Assert.IsTrue(result.TotalItems == repository.Posts.Count());
            Assert.IsTrue(result.TotalPages == (int)Math.Ceiling((double)MockPostRepository.STDN / target.PageSize));           
        }

        [TestMethod]
        public void Can_Send_PostPagination_View_Model_1()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            IPostService service = new PostService(repository);
            HomeController target = new HomeController(service);           

            // Act            
            PostPagingInfo pagingInfo = ((HomePostViewModel)((ViewResult)target.Post(2)).Model).PagingInfo;

            // Assert            

            Assert.IsTrue(pagingInfo.Previous.PostId == 1);
            Assert.IsTrue(pagingInfo.Next.PostId == 3);
        }

        [TestMethod]
        public void Can_Send_PostPagination_View_Model_2()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            IPostService service = new PostService(repository);
            HomeController target = new HomeController(service);

            // Act            
            PostPagingInfo pagingInfo = ((HomePostViewModel)((ViewResult)target.Post(1)).Model).PagingInfo;

            // Assert            

            Assert.IsNull(pagingInfo.Previous);
            Assert.IsTrue(pagingInfo.Next.PostId == 2);
        }

        [TestMethod]
        public void Can_Send_PostPagination_View_Model_3()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            IPostService service = new PostService(repository);
            HomeController target = new HomeController(service);

            // Act            
            PostPagingInfo pagingInfo = ((HomePostViewModel)((ViewResult)target.Post(MockPostRepository.STDN)).Model).PagingInfo;

            // Assert            

            Assert.IsTrue(pagingInfo.Previous.PostId == MockPostRepository.STDN - 1);
            Assert.IsNull(pagingInfo.Next);
        }
    }
}
