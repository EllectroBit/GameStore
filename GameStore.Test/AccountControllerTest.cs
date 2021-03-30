using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using GameStore.Models;
using Microsoft.Data.SqlClient;
using GameStore.Controllers;
using Microsoft.AspNetCore.Mvc;
using GameStore.Interfaces;
using GameStore.Models.ViewModels;

namespace GameStore.Test
{
    public class AccountControllerTest
    {
        [Fact]
        public void IndexReturnViewResult()
        {
            //Arrange
            var mock = new Mock<IStore>();
            var controller = new AccountController(mock.Object);
            //Act
            var model = controller.Index();
            //Assert
            Assert.IsType<ViewResult>(model);
        }

        [Fact]
        public async Task LoginReturnsIndexViewResultAsync()
        {
            //Arrange
            LoginViewModel login = new LoginViewModel();

            var mock = new Mock<IStore>();
            var controller = new AccountController(mock.Object);
            controller.ModelState.AddModelError("Email", "Wrong Email");
            //Act
            ViewResult result = (ViewResult)await controller.Login(login);
            //Assert
            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public async Task LoginReturnsIndexViewResultAndAddModelErorrAsync()
        {
            //Arrange
            LoginViewModel login = new LoginViewModel();

            var mock = new Mock<IStore>();
            mock.Setup(m => m.GetUserAsync(login)).ReturnsAsync(new TestContext().Users.FirstOrDefault(u => u.ID == 999));
            var controller = new AccountController(mock.Object);
            //Act
            ViewResult result = (ViewResult)await controller.Login(login);
            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Index", result.ViewName);
        }

        //[Fact]
        //public async Task LoginAuthenticateAndRedirectsToStoreControllerIndexAsync()
        //{
        //    //Arrange
        //    LoginViewModel login = new LoginViewModel();
        //    User user = new TestContext().Users.FirstOrDefault();

        //    var mock = new Mock<IStore>();
        //    mock.Setup(m => m.GetUserAsync(login)).ReturnsAsync(user);
        //    var controller = new AccountController(mock.Object);
        //    //Act
        //    RedirectToActionResult result = (RedirectToActionResult)await controller.Login(login);
        //    //Assert
        //    Assert.False(controller.ModelState.IsValid);
        //    Assert.Equal("Store", result.ControllerName);
        //    Assert.Equal("Index", result.ActionName);
        //}

        [Fact]
        public async Task RegisterReturnsIndexViewResultAsync()
        {
            //Arrange
            RegisterViewModels reg = new RegisterViewModels();

            var mock = new Mock<IStore>();
            var controller = new AccountController(mock.Object);
            controller.ModelState.AddModelError("Email", "Wrong Email");
            //Act
            ViewResult result = (ViewResult)await controller.Register(reg);
            //Assert
            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public async Task RegisterReturnsIndexViewResultAndAddModelErorrAsync()
        {
            //Arrange
            RegisterViewModels reg = new RegisterViewModels() {Email = "admin@gmail.com" };

            var mock = new Mock<IStore>();
            mock.Setup(m => m.GetUsersAsync()).ReturnsAsync(new TestContext().Users);
            var controller = new AccountController(mock.Object);
            //Act
            ViewResult result = (ViewResult)await controller.Register(reg);
            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Index", result.ViewName);
        }
    }
}
