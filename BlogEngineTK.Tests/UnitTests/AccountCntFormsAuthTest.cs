using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogEngineTK.WebUI.Controllers;
using System.Collections.Generic;
using Moq;
using System.Linq;
using BlogEngineTK.Domain.Entities;
using System.Web.Mvc;
using BlogEngineTK.WebUI.Models;
using BlogEngineTK.WebUI.Infrastructure;
using BlogEngineTK.Domain.Services.Authorization;
using BlogEngineTK.Domain.Services;
using BlogEngineTK.WebUI.Infrastructure.ModelBinders;

namespace BlogEngineTK.Tests.UnitTests
{
    /// <summary>
    /// Тестирование AccountController (WebUI) 
    /// и исп-мого им FormsAuthProvider (Domain) 
    /// (в некоторых тестах исп-ся mock IAuthProvider)
    /// </summary>
    [TestClass]    
    public class AccountControllerTest
    {
        [TestMethod]
        public void Auth_Correct_User()
        {
            // Arrange
            LoginViewModel userData = new LoginViewModel
            {
                Login = "user",
                Password = "pass"
            };
            string storedPasswordHash = Convert.ToBase64String(HashPassword.HashPass(userData.Password.ToCharArray(), new byte[0]));
            IAuthProvider authProv = new FormsAuthProvider(userData.Login, storedPasswordHash, "");
            AccountController target = new AccountController(authProv);

            // Act
            ActionResult result = target.Login(userData, "#true", SessionStorage.Current);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsTrue(((RedirectResult)result).Url.Contains("#true"));            
        }

        [TestMethod]
        public void Dont_Auth_Incorrect_User()
        {
            // Arrange
            LoginViewModel userData = new LoginViewModel
            {
                Login = "user",
                Password = "badpass"
            };
            string storedPasswordHash = Convert.ToBase64String(HashPassword.HashPass("pass".ToCharArray(), new byte[0]));
            IAuthProvider authProv = new FormsAuthProvider(userData.Login, storedPasswordHash, "");
            AccountController target = new AccountController(authProv);

            // Act
            ActionResult result = target.Login(userData, "#true", SessionStorage.Current);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Auth_Correct_User_With_Valid_Captcha()
        {
            // Arrange
            string captcha = "54asd#213_54WQExz";
            LoginViewModel userData = new LoginViewModel
            {
                Login = "user",
                Password = "pass",
                Captcha = captcha,
            };
            string storedPasswordHash = Convert.ToBase64String(HashPassword.HashPass(userData.Password.ToCharArray(), new byte[0]));
            IAuthProvider authProv = new FormsAuthProvider(userData.Login, storedPasswordHash, "");
            AccountController target = new AccountController(authProv);
            SessionStorage storage = SessionStorage.Current;
            
            storage.CaptchaCode = captcha;

            // Act
            ActionResult result = target.Login(userData, "#true", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsTrue(((RedirectResult)result).Url.Contains("#true"));
            Assert.IsTrue(storage.UnsucLoginAttempts == 0);
        }

        [TestMethod]
        public void Dont_Auth_Correct_User_With_Invalid_Captcha_1()
        {
            // Arrange
            LoginViewModel userData = new LoginViewModel
            {
                Login = "user",
                Password = "pass",
                Captcha = "bad"
            };
            string storedPasswordHash = Convert.ToBase64String(HashPassword.HashPass(userData.Password.ToCharArray(), new byte[0]));
            IAuthProvider authProv = new FormsAuthProvider(userData.Login, storedPasswordHash, "");
            AccountController target = new AccountController(authProv);
            SessionStorage storage = SessionStorage.Current;
            string captcha = "54asd#213_54WQExz";
            storage.CaptchaCode = captcha;

            // Act
            ActionResult result = target.Login(userData, "#true", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsTrue(storage.UnsucLoginAttempts == 1);
        }

        [TestMethod]
        public void Dont_Auth_Correct_User_With_Invalid_Captcha_2()
        {
            // Arrange
            LoginViewModel userData = new LoginViewModel
            {
                Login = "user",
                Password = "pass",
                Captcha = ""
            };
            string storedPasswordHash = Convert.ToBase64String(HashPassword.HashPass(userData.Password.ToCharArray(), new byte[0]));
            IAuthProvider authProv = new FormsAuthProvider(userData.Login, storedPasswordHash, "");
            AccountController target = new AccountController(authProv);
            SessionStorage storage = SessionStorage.Current;
            string captcha = "54asd#213_54WQExz";
            storage.CaptchaCode = captcha;

            // Act
            ActionResult result = target.Login(userData, "#true", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsTrue(storage.UnsucLoginAttempts == 1);
        }

        [TestMethod]
        public void Dont_Auth_Correct_User_With_Invalid_Captcha_3()
        {
            // Arrange
            LoginViewModel userData = new LoginViewModel
            {
                Login = "user",
                Password = "pass"
            };
            string storedPasswordHash = Convert.ToBase64String(HashPassword.HashPass(userData.Password.ToCharArray(), new byte[0]));
            IAuthProvider authProv = new FormsAuthProvider(userData.Login, storedPasswordHash, "");
            AccountController target = new AccountController(authProv);
            SessionStorage storage = SessionStorage.Current;
            string captcha = "54asd#213_54WQExz";
            storage.CaptchaCode = captcha;

            // Act
            ActionResult result = target.Login(userData, "#true", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsTrue(storage.UnsucLoginAttempts == 1);
        }

        [TestMethod]
        public void Dont_Auth_Correct_User_With_Invalid_Captcha_4()
        {
            // Arrange
            string captcha = "54asd#213_54WQExz";
            LoginViewModel userData = new LoginViewModel
            {
                Login = "user",
                Password = "pass",
                Captcha = captcha + "x"
            };
            string storedPasswordHash = Convert.ToBase64String(HashPassword.HashPass(userData.Password.ToCharArray(), new byte[0]));
            IAuthProvider authProv = new FormsAuthProvider(userData.Login, storedPasswordHash, "");
            AccountController target = new AccountController(authProv);
            SessionStorage storage = SessionStorage.Current;            
            storage.CaptchaCode = captcha;

            // Act
            ActionResult result = target.Login(userData, "#true", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsTrue(storage.UnsucLoginAttempts == 1);
        }

        [TestMethod]
        public void Logout_Redirect_User()
        {
            // Arrange
            LoginViewModel userData = new LoginViewModel
            {
                Login = "user",
                Password = "pass"
            };
            string storedPasswordHash = Convert.ToBase64String(HashPassword.HashPass(userData.Password.ToCharArray(), new byte[0]));
            IAuthProvider authProv = new FormsAuthProvider(userData.Login, storedPasswordHash, "");
            AccountController target = new AccountController(authProv);

            // Act
            ActionResult result = target.Logout("#true");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsTrue(((RedirectResult)result).Url.Contains("#true"));
        }

        [TestMethod]
        public void Pass_Remind_To_Correct_Email()
        {
            // Arrange
            PassReminderViewModel model = new PassReminderViewModel
            {
                Email = "correct",
            };
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.IsEmailExists(It.Is<string>(s => s == model.Email)))
                .Returns(true);
            AccountController target = new AccountController(mock.Object);

            // Act
            ActionResult result = target.PassReminder(model, "#true", SessionStorage.Current);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            mock.Verify(m => m.IsEmailExists(It.IsAny<string>()), Times.Once());
            mock.Verify(m => m.RemindPass(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            // тест может не проходить на слабых процессорах, изза фонового потока в контролллере
        }

        [TestMethod]
        public void Dont_Pass_Remind_To_Invalid_Email()
        {
            // Arrange
            PassReminderViewModel model = new PassReminderViewModel
            {
                Email = "invalid",
            };
            IAuthProvider authProv = new FormsAuthProvider("", "", "correct");
            AccountController target = new AccountController(authProv);

            // Act
            ActionResult result = target.PassReminder(model, "#true", SessionStorage.Current);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Dont_Pass_Remind_To_Correct_Email_With_Invalid_Captcha_1()
        {
            // Arrange
            PassReminderViewModel model = new PassReminderViewModel
            {
                Email = "correct",
                Captcha = "incorrect"
            };
            IAuthProvider authProv = new FormsAuthProvider("", "", model.Email);
            AccountController target = new AccountController(authProv);
            SessionStorage storage = SessionStorage.Current;
            storage.CaptchaCode = "correct";

            // Act
            ActionResult result = target.PassReminder(model, "#true", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsTrue(storage.UnsucLoginAttempts == 1);
        }

        [TestMethod]
        public void Pass_Remind_To_Correct_Email_With_Valid_Captcha()
        {
            // Arrange
            PassReminderViewModel model = new PassReminderViewModel
            {
                Email = "correct",
                Captcha = "correct"
            };
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.IsEmailExists(It.Is<string>(s => s == model.Email)))
                .Returns(true);
            AccountController target = new AccountController(mock.Object);
            SessionStorage storage = SessionStorage.Current;
            storage.CaptchaCode = model.Captcha;

            // Act
            ActionResult result = target.PassReminder(model, "#true", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            mock.Verify(m => m.IsEmailExists(It.IsAny<string>()), Times.Once());
            mock.Verify(m => m.RemindPass(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            // тест может не проходить на слабых процессорах, изза фонового потока в контролллере
            Assert.IsTrue(storage.UnsucLoginAttempts == 0);
        }

        [TestMethod]
        public void Dont_Pass_Remind_To_Correct_Email_With_Invalid_Captcha_2()
        {
            // Arrange
            PassReminderViewModel model = new PassReminderViewModel
            {
                Email = "correct",
                Captcha = ""
            };
            IAuthProvider authProv = new FormsAuthProvider("", "", model.Email);
            AccountController target = new AccountController(authProv);
            SessionStorage storage = SessionStorage.Current;
            storage.CaptchaCode = "correct";

            // Act
            ActionResult result = target.PassReminder(model, "#true", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsTrue(storage.UnsucLoginAttempts == 1);
        }

        [TestMethod]
        public void Dont_Pass_Remind_To_Correct_Email_With_Invalid_Captcha_3()
        {
            // Arrange
            PassReminderViewModel model = new PassReminderViewModel
            {
                Email = "correct"
            };
            IAuthProvider authProv = new FormsAuthProvider("", "", model.Email);
            AccountController target = new AccountController(authProv);
            SessionStorage storage = SessionStorage.Current;
            storage.CaptchaCode = "correct";

            // Act
            ActionResult result = target.PassReminder(model, "#true", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsTrue(storage.UnsucLoginAttempts == 1);
        }

        [TestMethod]
        public void Accept_Valid_ConfirmCode()
        {
            // Arrange
            string code = "correct";
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.IsCorrectCode(It.Is<string>(s => s == code)))
                .Returns(true);
            AccountController target = new AccountController(mock.Object);
            SessionStorage storage = SessionStorage.Current;

            // Act
            ActionResult result = target.ConfirmNewPass(code, storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            mock.Verify(m => m.IsCorrectCode(It.IsAny<string>()), Times.Once());
            Assert.IsTrue(storage.UnsucNumOfConfirmCode == 0);
        }

        [TestMethod]
        public void Deny_Invalid_ConfirmCode()
        {
            // Arrange
            string code = "correct";
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.IsCorrectCode(It.Is<string>(s => s == code)))
                .Returns(true);
            AccountController target = new AccountController(mock.Object);
            SessionStorage storage = SessionStorage.Current;

            // Act
            ActionResult result = target.ConfirmNewPass("bad", storage);

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
            mock.Verify(m => m.IsCorrectCode(It.IsAny<string>()), Times.Once());
            Assert.IsTrue(storage.UnsucNumOfConfirmCode == 1);
        }
    }
}
