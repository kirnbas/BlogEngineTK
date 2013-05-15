using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogEngineTK.WebUI.Controllers;
using BlogEngineTK.Domain.Entities;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Moq;
using BlogEngineTK.Domain.Repositories;
using BlogEngineTK.Tests.Environment;

namespace BlogEngineTK.Tests.UnitTests
{
    [TestClass]
    public class PostModelTest
    {
        #region У Post модели должны быть основные значения

        [TestMethod]
        public void Valid_Post_Model_Because_Have_Need_Values()
        {
            // Arrange
            Post model = new Post
            {
                Header = "hello",
                ShortText = "short",
                FullText = "full"
            };
            Mock<IPostRepository> mock = new Mock<IPostRepository>();
            PostController target = new PostController(mock.Object);

            // Assert            
            Assert.IsTrue(Helpers.IsValidModel<Post>(model));
        }

        [TestMethod]
        public void Invalid_Post_Model_Because_Havent_Need_Values_1()
        {
            // Arrange
            Mock<IPostRepository> mock = new Mock<IPostRepository>();
            Post model = new Post
            {
            };
            PostController target = new PostController(mock.Object);
            
            // Assert            
            Assert.IsFalse(Helpers.IsValidModel<Post>(model));
        }

        [TestMethod]
        public void Invalid_Post_Model_Because_Havent_Need_Values_2()
        {
            // Arrange
            Post model = new Post
            {
                Header = "hello",
                ShortText = "short",
                FullText = "full"
            };
            Mock<IPostRepository> mock = new Mock<IPostRepository>();
            PostController target = new PostController(mock.Object);
            switch (new Random().Next(3)) // Проверяем что модель не валидна, при отсутствии любого из полей
            {
                case 0: model.Header = null; break;
                case 1: model.ShortText = null; break;
                case 2: model.FullText = null; break;
            }

            // Assert            
            Assert.IsFalse(Helpers.IsValidModel<Post>(model));
        }  

        #endregion

        #region Проверка RouteSegment

        [TestMethod]
        public void Valid_Post_Model_Because_RouteSegment_1()
        {
            // Arrange
            Post model = new Post
            {
                Header = "hello",
                ShortText = "short",
                FullText = "full",
                RouteSegment = "invalid"
            };
            Mock<IPostRepository> mock = new Mock<IPostRepository>();
            PostController target = new PostController(mock.Object);
            
            // Assert            
            Assert.IsTrue(Helpers.IsValidModel<Post>(model));
        }

        [TestMethod]
        public void Valid_Post_Model_Because_RouteSegment_2()
        {
            // Arrange
            Post model = new Post
            {
                Header = "hello",
                ShortText = "short",
                FullText = "full",
                RouteSegment = "inva-lid"
            };
            Mock<IPostRepository> mock = new Mock<IPostRepository>();

            // Assert            
            Assert.IsTrue(Helpers.IsValidModel<Post>(model));
        }

        [TestMethod]
        public void Valid_Post_Model_Because_RouteSegment_3()
        {
            // Arrange
            Post model = new Post
            {
                Header = "hello",
                ShortText = "short",
                FullText = "full",
                RouteSegment = "in-va-lid"
            };
            Mock<IPostRepository> mock = new Mock<IPostRepository>();

            // Assert            
            Assert.IsTrue(Helpers.IsValidModel<Post>(model));
        }

        [TestMethod]
        public void Invalid_Post_Model_Because_RouteSegment_1()
        {
            // Arrange
            Post model = new Post
            {
                Header = "hello",
                ShortText = "short",
                FullText = "full",
                RouteSegment = "inva lid"
            };
            Mock<IPostRepository> mock = new Mock<IPostRepository>();

            // Assert            
            Assert.IsFalse(Helpers.IsValidModel<Post>(model));
        }

        [TestMethod]
        public void Invalid_Post_Model_Because_RouteSegment_2()
        {
            // Arrange
            Post model = new Post
            {
                Header = "hello",
                ShortText = "short",
                FullText = "full",
                RouteSegment = "inva - lid"
            };
            Mock<IPostRepository> mock = new Mock<IPostRepository>();


            // Assert            
            Assert.IsFalse(Helpers.IsValidModel<Post>(model));
        }

        [TestMethod]
        public void Invalid_Post_Model_Because_RouteSegment_3()
        {
            // Arrange
            Post model = new Post
            {
                Header = "hello",
                ShortText = "short",
                FullText = "full",
                RouteSegment = "inva- lid"
            };
            Mock<IPostRepository> mock = new Mock<IPostRepository>();


            // Assert            
            Assert.IsFalse(Helpers.IsValidModel<Post>(model));
        }

        [TestMethod]
        public void Invalid_Post_Model_Because_RouteSegment_4()
        {
            // Arrange
            Post model = new Post
            {
                Header = "hello",
                ShortText = "short",
                FullText = "full",
                RouteSegment = "inva -lid"
            };
            Mock<IPostRepository> mock = new Mock<IPostRepository>();

            // Assert            
            Assert.IsFalse(Helpers.IsValidModel<Post>(model));
        }

        #endregion

        #region Макс. длина входных значений

        [TestMethod]
        public void Valid_Post_Model_Because_Length()
        {
            // Arrange
            Post model = new Post
            {
                Header = new string('h', 256),                
                ShortText = new string('s', 10000),
                FullText = new string('f', 200000),
                RouteSegment = new string('r', 50)
            };            
            Mock<IPostRepository> mock = new Mock<IPostRepository>();

            // Assert           
            Assert.IsTrue(Helpers.IsValidModel<Post>(model));
        }

        [TestMethod]
        public void Invalid_Post_Model_Because_Length()
        {
            // Arrange
            Post model = new Post
            {
                Header = new string('h', 256),
                ShortText = new string('s', 10000),
                FullText = new string('f', 200000),
                RouteSegment = new string('r', 50)
            };
            // Проверяем что модель не валидна, при превышение значения в одном из полей
            switch (new Random().Next(4))
            {
                case 0: model.Header += "z"; break;
                case 1: model.ShortText += "z"; break;
                case 2: model.FullText += "z"; break;
                case 3: model.RouteSegment += "z"; break;
            }
            Mock<IPostRepository> mock = new Mock<IPostRepository>();
            
            // Assert           
            Assert.IsFalse(Helpers.IsValidModel<Post>(model));
        }

        #endregion
    }
}
