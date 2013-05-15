using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Moq;
using BlogEngineTK.WebUI.Infrastructure.Filters;

namespace BlogEngineTK.Tests.UnitTests
{
    [TestClass]
    public class FiltersTests
    {
        private ActionExecutingContext GetActionExecutingContext()
        {
            var result = new Mock<ActionExecutingContext>();

            return result.Object;
        }

        //[TestMethod]
        //public void BlogOffAttr_Should_Return_View_If_Blog_Off()
        //{
        //    // Arrange
        //    var filterContext = GetActionExecutingContext();
        //    var filter = new BlogOffAttribute(false);

        //    // Act
        //    filter.OnActionExecuting(filterContext);

        //    // Assert
        //    Assert.IsInstanceOfType(filterContext.Result, typeof(ViewResult));
        //}

        //[TestMethod]
        //public void BlogOffAttr_Should_Return_Null_If_Blog_On()
        //{
        //    // Arrange
        //    var filterContext = GetActionExecutingContext();
        //    var filter = new BlogOffAttribute(true);

        //    // Act
        //    filter.OnActionExecuting(filterContext);

        //    // Assert
        //    Assert.IsNull(filterContext.Result);
        //}
    }
}
