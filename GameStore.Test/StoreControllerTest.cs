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
using Microsoft.AspNetCore.Http;
using System.IO;

namespace GameStore.Test
{
    public class StoreControllerTest
    {
        [Fact]
        public void IndexReturnsViewResultWithListOfModels()
        {
            //Arrange
            var mock = new Mock<IStore>();
            mock.Setup(m => m.GetGames()).Returns(new TestContext().Games);
            var controller = new StoreController(mock.Object);
            //Act
            var result = controller.Index();
            //Assert
            var model = Assert.IsType<ViewResult>(result);
            Assert.NotNull(model.Model);
            Assert.NotEmpty((System.Collections.IEnumerable)model.Model);
            Assert.Equal("Index", model.ViewData["action"]);
        }

        [Fact]
        public void AboutReturnsViewResultWithModel()
        {
            //Arrange
            Game game = new TestContext().Games.FirstOrDefault();

            var mock = new Mock<IStore>();
            mock.Setup(m => m.GetGame(0)).Returns(game);
            var controller = new StoreController(mock.Object);
            //Act
            var result = controller.About(0);
            //Assert
            var model = Assert.IsType<ViewResult>(result);
            Assert.NotNull(model.Model);
            Assert.Equal(game, model.Model);
        }

        [Fact]
        public void AddRedirectsToAdd()
        {
            //Arrange
            var mock = new Mock<IStore>();
            var controller = new StoreController(mock.Object);
            //Act
            controller.ModelState.AddModelError("", "Wrong values");
            var result = controller.Add(new GameViewModel());
            //Assert
            var model = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Add", model.ActionName);
        }

        [Fact]
        public void AddSetGameAndRedirectToIndex()
        {
            //Arrange
            Game game = new TestContext().Games.FirstOrDefault();
            var img = new Mock<IFormFile>();
            img.Setup(m => m.OpenReadStream()).Returns(new FileStream("1.jpg", FileMode.Open));

            GameViewModel gv = new GameViewModel()
            {
                ID = game.ID,
                Description = game.Description,
                Genre = game.Genre,
                Name = game.Name,
                Negative = game.Negative,
                Positive = game.Positive,
                Year = game.Year,
                Img = img.Object
            };

            var mock = new Mock<IStore>();
            var controller = new StoreController(mock.Object);
            //Act
            var result = controller.Add(gv);
            //Assert
            var model = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", model.ActionName);
            mock.Verify(m => m.SetGame(It.IsAny<Game>()));
        }

        [Fact]
        public void EditReturnsViewModel()
        {
            //Arrange
            Game game = new TestContext().Games.FirstOrDefault();
            var img = new Mock<IFormFile>();
            img.Setup(m => m.OpenReadStream()).Returns(new FileStream("1.jpg", FileMode.Open));

            GameViewModel gv = new GameViewModel()
            {
                ID = game.ID,
                Description = game.Description,
                Genre = game.Genre,
                Name = game.Name,
                Negative = game.Negative,
                Positive = game.Positive,
                Year = game.Year,
                Img = img.Object
            };

            var mock = new Mock<IStore>();
            mock.Setup(m => m.GetGame(game.ID)).Returns(game);
            var controller = new StoreController(mock.Object);
            //Act
            var result = controller.Edit(game.ID);
            //Assert
            var model = Assert.IsType<ViewResult>(result);
            var g = Assert.IsType<GameViewModel>(model.Model);
            Assert.Equal(gv.Name, g.Name);
        }

        [Fact]
        public void EditRedirectsToIndex()
        {
            //Arrange
            Game game = new TestContext().Games.FirstOrDefault();

            var mock = new Mock<IStore>();
            mock.Setup(m => m.GetGame(game.ID)).Returns(new TestContext().Games.FirstOrDefault(x => x.ID == 999));
            var controller = new StoreController(mock.Object);
            //Act
            var result = controller.Edit(game.ID);
            //Assert
            var model = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", model.ActionName);
        }

        [Fact]
        public void EditPostReturnsSameModel()
        {
            //Arrange
            Game game = new TestContext().Games.FirstOrDefault();

            var img = new Mock<IFormFile>();
            img.Setup(m => m.OpenReadStream()).Returns(new FileStream("1.jpg", FileMode.Open));
            GameViewModel gv = new GameViewModel()
            {
                ID = game.ID,
                Description = game.Description,
                Genre = game.Genre,
                Name = game.Name,
                Negative = game.Negative,
                Positive = game.Positive,
                Year = game.Year,
                Img = img.Object
            };

            var mock = new Mock<IStore>();
            var controller = new StoreController(mock.Object);
            //Act
            controller.ModelState.AddModelError("", "Wrong model");
            var result = controller.Edit(gv);
            //Assert
            var model = Assert.IsType<ViewResult>(result);
            Assert.Equal(gv, model.Model);
        }

        [Fact]
        public void EditRedirectsToIndexAndChangeData()
        {
            //Arrange
            Game game = new TestContext().Games.FirstOrDefault();

            var img = new Mock<IFormFile>();
            img.Setup(m => m.OpenReadStream()).Returns(new FileStream("1.jpg", FileMode.Open));
            GameViewModel gv = new GameViewModel()
            {
                ID = game.ID,
                Description = game.Description,
                Genre = game.Genre,
                Name = game.Name,
                Negative = game.Negative,
                Positive = game.Positive,
                Year = game.Year,
                Img = img.Object
            };

            var mock = new Mock<IStore>();
            mock.Setup(m => m.GetGame(It.IsAny<int>())).Returns(game);
            var controller = new StoreController(mock.Object);
            //Act
            var result = controller.Edit(gv);
            //Assert
            var model = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", model.ActionName);
            mock.Verify(m => m.DeleteGame((int)gv.ID));
            mock.Verify(m => m.SetGame(It.IsAny<Game>()));
        }

        [Fact]
        public void DeleteRemovesGameAndRedirectsToIndex()
        {
            //Arrange
            var mock = new Mock<IStore>();
            var controller = new StoreController(mock.Object);
            //Act
            var result = controller.Delete(1);
            //Assert
            var model = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", model.ActionName);
            mock.Verify(m => m.DeleteGame(1));
        }

        [Fact]
        public void SearchTest()
        {
            //Arrange
            var mock = new Mock<IStore>();
            mock.Setup(m => m.GetGames()).Returns(new TestContext().Games);
            var controller = new StoreController(mock.Object);
            //Act
            var result = controller.Search("2");
            //Assert
            var model = Assert.IsType<ViewResult>(result);
            Assert.Equal("Search", model.ViewData["action"]);
            Assert.Equal("2", model.ViewData["str"]);
            List<Game> g = Assert.IsType<List<Game>>(model.Model);
            Assert.Equal(new TestContext().Games.Where(g => g.Name.Contains("2")).FirstOrDefault().Name, g.FirstOrDefault().Name);
        }

        [Fact]
        public void FilterTest()
        {
            //Arrange
            var mock = new Mock<IStore>();
            mock.Setup(m => m.GetGames()).Returns(new TestContext().Games);
            var controller = new StoreController(mock.Object);
            //Act
            var result = controller.Filter(Genre.Action);
            //Assert
            var model = Assert.IsType<ViewResult>(result);
            Assert.Equal("Filter", model.ViewData["action"]);
            Assert.Equal(Genre.Action, model.ViewData["str"]);
            List<Game> g = Assert.IsType<List<Game>>(model.Model);
            Assert.Equal(new TestContext().Games.Where(g => g.Genre == Genre.Action).FirstOrDefault().Name, g.FirstOrDefault().Name);
        }

        [Fact]
        public void PageTest()
        {
            //Arrange
            List<Game> games = new List<Game>();
            for (int i = 0; i < 18; i++)
                games.Add(new Game() { ID = i });

            var mock = new Mock<IStore>();
            var controller = new StoreController(mock.Object);
            //Act
            List<Game> result = controller.Page(2, games);
            //Assert
            Assert.Equal(9, result.Count());
            Assert.InRange<int>(result.FirstOrDefault().ID, 9, 18);
        }
    }
}
