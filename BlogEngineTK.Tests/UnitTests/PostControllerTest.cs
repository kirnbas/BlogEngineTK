using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogEngineTK.Domain.Repositories;
using BlogEngineTK.Domain.Services;
using BlogEngineTK.Domain.Entities;
using BlogEngineTK.WebUI.Controllers;
using System.Web.Mvc;
using Moq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BlogEngineTK.Tests.Environment;
using System.Linq;

namespace BlogEngineTK.Tests.UnitTests
{
    [TestClass]
    public class PostControllerTest
    {
        #region Insert New Post

        [TestMethod]
        public void Can_Insert_Correct_Post()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);
            Post post = new Post()
            {
                Header = "h",
                ShortText = "s",
                FullText = "f"
            };

            // Act
            if (!Helpers.IsValidModel<Post>(post))
            {
                target.ModelState.AddModelError("", "");
            }
            ActionResult result = target.Create(post, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            MockPostRepository.Mock.Verify(x => x.InsertPost(It.Is<Post>(p => p == post)), Times.Once());
        }

        [TestMethod]
        public void Cant_Insert_Uncorrect_Post()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);
            Post post = new Post();

            // Act
            if (!Helpers.IsValidModel<Post>(post))
            {
                target.ModelState.AddModelError("", "");
            }
            ActionResult result = target.Create(post, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            MockPostRepository.Mock.Verify(x => x.InsertPost(It.Is<Post>(p => p == post)), Times.Never());
        }

        #endregion

        #region Update Post

        [TestMethod]
        public void Update_Found_Exists_Post()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);

            // Act
            ActionResult result = target.Update(MockPostRepository.STDN, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Update_Doesnt_Found_Post()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);

            // Act
            ActionResult result = target.Update(MockPostRepository.STDN + 1, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        [TestMethod]
        public void Can_Update_Correct_Post()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);
            Post post = repository.Posts.First(x => x.PostId == MockPostRepository.STDN);
            post.Header = "edited";
            post.FullText = "edited";
            post.ShortText = "edited";
            post.RouteSegment = "edited";

            // Act
            if (!Helpers.IsValidModel<Post>(post))
            {
                target.ModelState.AddModelError("", "");
            }
            ActionResult result = target.Update(post, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            MockPostRepository.Mock.Verify(x => x.UpdatePost(It.Is<Post>(p => p == post)), Times.Once());
        }

        [TestMethod]
        public void Cant_Update_Uncorrect_Post_1()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);
            Post post = repository.Posts.First(x => x.PostId == MockPostRepository.STDN);
            post.Header = "edited";
            post.FullText = "edited";
            post.ShortText = "edited";
            switch (new Random().Next(3)) // Проверяем что модель не валидна, при отсутствии любого из полей
            {
                case 0: post.Header = null; break;
                case 1: post.ShortText = null; break;
                case 2: post.FullText = null; break;
            }

            // Act
            if (!Helpers.IsValidModel<Post>(post))
            {
                target.ModelState.AddModelError("", "");
            }
            ActionResult result = target.Update(post, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            MockPostRepository.Mock.Verify(x => x.UpdatePost(It.Is<Post>(p => p == post)), Times.Never());
        }

        [TestMethod]
        public void Cant_Update_Uncorrect_Post_2()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);
            Post post = new Post();

            // Act
            if (!Helpers.IsValidModel<Post>(post))
            {
                target.ModelState.AddModelError("", "");
            }
            ActionResult result = target.Update(post, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            MockPostRepository.Mock.Verify(x => x.UpdatePost(It.Is<Post>(p => p == post)), Times.Never());
        }        

        #endregion

        #region Delete post

        [TestMethod]
        public void Delete_Found_Exists_Post()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);

            // Act
            ActionResult result = target.Delete(MockPostRepository.STDN, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Delete_Doesnt_Found_Post()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);

            // Act
            ActionResult result = target.Delete(MockPostRepository.STDN + 1, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        [TestMethod]
        public void Delete_Post_With_Correct_Id()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);

            // Act
            ActionResult result = target.DeletePost(MockPostRepository.STDN, "#r");

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        [TestMethod]
        public void Cant_Delete_Post_With_Uncorrect_Id()
        {
            // Arrange
            IPostRepository repository = MockPostRepository.GetRepository(MockPostRepository.STDN);
            PostController target = new PostController(repository);

            // Act
            ActionResult result = target.DeletePost(MockPostRepository.STDN + 1, "#r");

            // Assert            
            Assert.IsNull(result);
        }

        #endregion
    }
}
