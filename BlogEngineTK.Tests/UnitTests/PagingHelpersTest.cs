using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogEngineTK.WebUI.Infrastructure.HtmlHelpers;
using System.Web.Mvc;
using BlogEngineTK.WebUI.Models;
using System.Linq;
using BlogEngineTK.Domain.Entities;

namespace BlogEngineTK.Tests.UnitTests
{
    /// <summary>
    /// Тестирование HtmlHelper's extension method
    /// предназначенного для реализации перелистывания страниц
    /// </summary>
    [TestClass]
    public class PagingHelpersTest
    {
        [TestMethod]
        public void Can_Return_Correct_Paginator_For_3_Page_2()
        {
            // Assert
            PagingInfo info = new PagingInfo {
                CurrentPage = 2,
                ItemsPerPage = 10,
                TotalItems = 30
            };
            HtmlHelper helper = null;            

            // Arrange
            string result = helper.GetPageLinks(info, i => "page" + i).ToString();

            // Act
            Assert.IsTrue(result.Contains(@"<a href=""page1"">Пред. </a>"));
            Assert.IsTrue(!result.Contains("page0"));
            Assert.IsTrue(result.Contains("page1"));
            Assert.IsTrue(!result.Contains("page2"));
            Assert.IsTrue(result.Contains("2"));
            Assert.IsTrue(result.Contains("page3"));
            Assert.IsTrue(!result.Contains("page4"));
            Assert.IsTrue(result.Contains(@"<a href=""page3""> След.</a>"));
        }

        [TestMethod]
        public void Can_Return_Correct_Paginator_For_3_Page_1()
        {
            // Assert
            PagingInfo info = new PagingInfo
            {
                CurrentPage = 1,
                ItemsPerPage = 10,
                TotalItems = 30
            };
            HtmlHelper helper = null;

            // Arrange
            string result = helper.GetPageLinks(info, i => "page" + i).ToString();

            // Act
            Assert.IsTrue(!result.Contains(@"Пред."));
            Assert.IsTrue(!result.Contains("page0"));
            Assert.IsTrue(!result.Contains("page1"));
            Assert.IsTrue(result.Contains("page2"));
            Assert.IsTrue(result.Contains("1"));
            Assert.IsTrue(result.Contains("page3"));
            Assert.IsTrue(!result.Contains("page4"));
            Assert.IsTrue(result.Contains(@"<a href=""page2""> След.</a>"));
        }

        [TestMethod]
        public void Can_Return_Correct_Paginator_For_3_Page_3()
        {
            // Assert
            PagingInfo info = new PagingInfo
            {
                CurrentPage = 3,
                ItemsPerPage = 10,
                TotalItems = 30
            };
            HtmlHelper helper = null;

            // Arrange
            string result = helper.GetPageLinks(info, i => "page" + i).ToString();

            // Act
            Assert.IsTrue(result.Contains(@"<a href=""page2"">Пред. </a>"));
            Assert.IsTrue(!result.Contains("page0"));
            Assert.IsTrue(result.Contains("page1"));
            Assert.IsTrue(result.Contains("page2"));
            Assert.IsTrue(result.Contains("3"));
            Assert.IsTrue(!result.Contains("page3"));
            Assert.IsTrue(!result.Contains("page4"));
            Assert.IsTrue(!result.Contains(@"След."));
        }

        [TestMethod]
        public void Can_Return_Correct_Paginator_For_20_Page_3()
        {
            // Assert
            PagingInfo info = new PagingInfo
            {
                CurrentPage = 3,
                ItemsPerPage = 2,
                TotalItems = 40
            };
            HtmlHelper helper = null;

            // Arrange
            string result = helper.GetPageLinks(info, i => "page" + i).ToString();

            // Act
            Assert.IsTrue(result.Contains(@"<a href=""page2"">Пред. </a>"));
            Assert.IsTrue(!result.Contains("page0"));
            Assert.IsTrue(result.Contains("page1"));
            Assert.IsTrue(result.Contains("page2"));
            Assert.IsTrue(!result.Contains("page3"));
            Assert.IsTrue(result.Contains("3"));            
            Assert.IsTrue(result.Contains("page4"));
            Assert.IsTrue(!result.Contains("page5"));
            Assert.IsTrue(result.Contains("..."));
            Assert.IsTrue(result.IndexOf("...") == result.LastIndexOf("..."));
            Assert.IsTrue(!result.Contains("page17"));
            Assert.IsTrue(result.Contains("page18"));
            Assert.IsTrue(result.Contains("page19"));
            Assert.IsTrue(result.Contains("page20"));
            Assert.IsTrue(!result.Contains("page21"));
            Assert.IsTrue(result.Contains(@"<a href=""page4""> След.</a>"));
        }

        [TestMethod]
        public void Can_Return_Correct_Paginator_For_20_Page_15()
        {
            // Assert
            PagingInfo info = new PagingInfo
            {
                CurrentPage = 15,
                ItemsPerPage = 2,
                TotalItems = 40
            };
            HtmlHelper helper = null;

            // Arrange
            string result = helper.GetPageLinks(info, i => "page" + i).ToString();

            // Act
            Assert.IsTrue(result.Contains(@"<a href=""page14"">Пред. </a>"));
            Assert.IsTrue(!result.Contains("page0"));
            Assert.IsTrue(result.Contains("page1"));
            Assert.IsTrue(result.Contains("page2"));
            Assert.IsTrue(result.Contains("page3"));
            Assert.IsTrue(!result.Contains("page4"));

            Assert.IsTrue(!result.Contains("page13"));
            Assert.IsTrue(result.Contains("page14"));
            Assert.IsTrue(result.Contains("15"));
            Assert.IsTrue(!result.Contains("page15"));
            Assert.IsTrue(result.Contains("page16"));            
            Assert.IsTrue(result.IndexOf("...") != result.LastIndexOf("..."));

            Assert.IsTrue(!result.Contains("page17"));
            Assert.IsTrue(result.Contains("page18"));
            Assert.IsTrue(result.Contains("page19"));
            Assert.IsTrue(result.Contains("page20"));
            Assert.IsTrue(!result.Contains("page21"));
            Assert.IsTrue(result.Contains(@"<a href=""page16""> След.</a>"));
        }

        [TestMethod]
        public void Can_Return_Correct_Paginator_For_20_Page_20()
        {
            // Assert
            PagingInfo info = new PagingInfo
            {
                CurrentPage = 20,
                ItemsPerPage = 2,
                TotalItems = 40
            };
            HtmlHelper helper = null;

            // Arrange
            string result = helper.GetPageLinks(info, i => "page" + i).ToString();

            // Act
            Assert.IsTrue(result.Contains(@"<a href=""page19"">Пред. </a>"));
            Assert.IsTrue(!result.Contains("page0"));
            Assert.IsTrue(result.Contains("page1"));
            Assert.IsTrue(result.Contains("page2"));            
            Assert.IsTrue(result.Contains("page3"));
            Assert.IsTrue(!result.Contains("page4"));

            Assert.IsTrue(result.Contains("20"));
            Assert.IsTrue(result.Contains("..."));
            Assert.IsTrue(result.IndexOf("...") == result.LastIndexOf("..."));
            Assert.IsTrue(!result.Contains("page17"));
            Assert.IsTrue(result.Contains("page18"));
            Assert.IsTrue(result.Contains("page19"));
            Assert.IsTrue(!result.Contains("page20"));
            Assert.IsTrue(!result.Contains("page21"));
            Assert.IsTrue(!result.Contains(@"След."));
        }

        [TestMethod]
        public void Can_Return_Correct_Paginator_For_20_Page_16()
        {
            // Assert
            PagingInfo info = new PagingInfo
            {
                CurrentPage = 16,
                ItemsPerPage = 2,
                TotalItems = 40
            };
            HtmlHelper helper = null;

            // Arrange
            string result = helper.GetPageLinks(info, i => "page" + i).ToString();

            // Act
            Assert.IsTrue(result.Contains(@"<a href=""page15"">Пред. </a>"));
            Assert.IsTrue(!result.Contains("page0"));
            Assert.IsTrue(result.Contains("page1"));
            Assert.IsTrue(result.Contains("page2"));
            Assert.IsTrue(result.Contains("page3"));                 
            Assert.IsTrue(!result.Contains("page4"));

            Assert.IsTrue(!result.Contains("page14"));
            Assert.IsTrue(result.Contains("page15"));
            Assert.IsTrue(result.Contains("16"));
            Assert.IsTrue(result.Contains("..."));
            Assert.IsTrue(result.IndexOf("...") == result.LastIndexOf("..."));

            Assert.IsTrue(result.Contains("page17"));
            Assert.IsTrue(result.Contains("page18"));
            Assert.IsTrue(result.Contains("page19"));
            Assert.IsTrue(result.Contains("page20"));
            Assert.IsTrue(!result.Contains("page21"));
            Assert.IsTrue(result.Contains(@"<a href=""page17""> След.</a>"));
        }
    }
}
