using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BlogEngineTK.Domain.Services.Authorization;
using BlogEngineTK.WebUI.Areas.Admin.Controllers;
using BlogEngineTK.WebUI.Areas.Admin.Models;
using System.Web.Mvc;
using BlogEngineTK.Tests.Environment;

namespace BlogEngineTK.Tests.UnitTests
{
    [TestClass]
    public class SetsCntFormsAuthTest
    {
        [TestMethod]
        public void Change_Pass_If_Data_Valid()
        {
            // Arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.IsValidUser(It.IsAny<string>(), It.Is<char[]>(s => new string(s) == "correct")))
                .Returns(true);
            SettingsController target = new SettingsController(mock.Object);
            SettingsIndexViewModel model = new SettingsIndexViewModel
            {
                Settings = new Domain.BlogSettings(),
                CurrentPassword = "correct",
                NewPassword = "newcorrect",
                ConfirmNewPassword = "newcorrect"
            };

            // Act
            ViewResult result = (ViewResult)target.IndexPost(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Dont_Change_Pass_If_Data_Invalid_1()
        {
            // Arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.IsValidUser(It.IsAny<string>(), It.Is<char[]>(s => new string(s) == "correct")))
                .Returns(true);
            SettingsController target = new SettingsController(mock.Object);
            SettingsIndexViewModel model = new SettingsIndexViewModel
            {
                CurrentPassword = "uncorrect",
                NewPassword = "newcorrect",
                ConfirmNewPassword = "newcorrect"
            };

            // Act
            ViewResult result = (ViewResult)target.IndexPost(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Dont_Change_Pass_If_Data_Invalid_2()
        {
            // Arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.IsValidUser(It.IsAny<string>(), It.Is<char[]>(s => new string(s) == "correct")))
                .Returns(true);
            SettingsController target = new SettingsController(mock.Object);
            SettingsIndexViewModel model = new SettingsIndexViewModel
            {
                CurrentPassword = "correct",
            };

            // Act
            ViewResult result = (ViewResult)target.IndexPost(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }
                
        [TestMethod]
        public void Dont_Change_Pass_If_Data_Invalid_3()
        {
            // Arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.IsValidUser(It.IsAny<string>(), It.Is<char[]>(s => new string(s) == "correct")))
                .Returns(true);
            SettingsController target = new SettingsController(mock.Object);
            SettingsIndexViewModel model = new SettingsIndexViewModel
            {
                CurrentPassword = "correct",
                NewPassword = "newcorrect",
                ConfirmNewPassword = "newuncorrect"
            };

            // Act
            if (!Helpers.IsValidModel<SettingsIndexViewModel>(model))
            {
                target.ModelState.AddModelError("", "");
            }
            ViewResult result = (ViewResult)target.IndexPost(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }
    }
}
