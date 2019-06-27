using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NLog;
using PlayBoardGame.Controllers;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;
using Xunit;

namespace PlayBoardGames.Tests
{
    public class GameControllerTests
    {
        [Fact]
        public void Can_Edit_Game()
        {
            //Arrange
            var mockRepo = new Mock<IGameRepository>();
            mockRepo.Setup(g => g.Games).Returns(new []
            {   new Game {GameId = 1, Title = "Game1"},
                new Game {GameId = 2, Title = "Game2"},
                new Game {GameId = 3, Title = "Game3"}
            }.AsQueryable);
            var mockLogger = new Mock<ILogger<GameController>>();
            
            var controller = new GameController(mockRepo.Object, mockLogger.Object);

            //Act
            var g1 = GetViewModel<CreateEditGameViewModel>(controller.Edit(1));
            var g2 = GetViewModel<CreateEditGameViewModel>(controller.Edit(2));
            var g3 = GetViewModel<CreateEditGameViewModel>(controller.Edit(3));

            //Assert
            Assert.Equal(1, g1.GameID);
            Assert.Equal("Game1", g1.Title);
            Assert.Equal(2, g2.GameID);
            Assert.Equal("Game2", g2.Title);
            Assert.Equal(3, g3.GameID);
            Assert.Equal("Game3", g3.Title);
        }

        [Fact]
        public void Cannot_Edit_NonExistedGame()
        {
            //Arrange
            var mockRepo = new Mock<IGameRepository>();
            mockRepo.Setup(g => g.Games).Returns(new []
            {   new Game {GameId = 1, Title = "Game1"},
                new Game {GameId = 2, Title = "Game2"},
                new Game {GameId = 3, Title = "Game3"}
            }.AsQueryable);
            var mockLogger = new Mock<ILogger<GameController>>();

            var controller = new GameController(mockRepo.Object, mockLogger.Object);

            //Act
            var result = GetViewModel<CreateEditGameViewModel>(controller.Edit(4));

            //Assert
            Assert.Null(result);
        }
        [Fact]
        public void Can_Delete_Valid_Game()
        {
            //Arrange
            var game = new Game { GameId = 2, Title = "Test" };
            var mockRepo = new Mock<IGameRepository>();
            mockRepo.Setup(m => m.Games).Returns(new [] {
            new Game {GameId = 1, Title = "P1" },
            game,
            new Game { GameId = 3, Title = "P3"},
            }.AsQueryable);
            var mockLogger = new Mock<ILogger<GameController>>();
            
            var controller = new GameController(mockRepo.Object, mockLogger.Object);

            //Act
            controller.Delete(game.GameId);

            //Assert
            mockRepo.Verify(m => m.DeleteGame(game.GameId));
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }
    }
}
