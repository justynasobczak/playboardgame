using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PlayBoardGame.Controllers;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;
using Xunit;
using Microsoft.AspNetCore.Hosting;

namespace PlayBoardGames.Tests
{
    public class GameControllerTests
    {
        [Fact]
        public void Can_Edit_Game()
        {
            //Arrange
            var game1 = new Game {GameId = 1, Title = "Game1"};
            var game2 = new Game {GameId = 2, Title = "Game2"};
            var game3 = new Game {GameId = 3, Title = "Game3"};
            var mockRepo = new Mock<IGameRepository>();
            mockRepo.Setup(g => g.Games).Returns(new []
            {   game1,
                game2,
                game3
            }.AsQueryable);
            mockRepo.Setup(g => g.GetGame(game1.GameId)).Returns(game1);
            mockRepo.Setup(g => g.GetGame(game2.GameId)).Returns(game2);
            mockRepo.Setup(g => g.GetGame(game3.GameId)).Returns(game3);
            var mockLogger = new Mock<ILogger<GameController>>();
            var mockEnvironment = new Mock<IHostingEnvironment>();
            
            var controller = new GameController(mockRepo.Object, mockLogger.Object, mockEnvironment.Object);

            //Act
            var g1 = GetViewModel<CreateEditGameViewModel>(controller.Edit(game1.GameId));
            var g2 = GetViewModel<CreateEditGameViewModel>(controller.Edit(game2.GameId));
            var g3 = GetViewModel<CreateEditGameViewModel>(controller.Edit(game3.GameId));

            //Assert
            Assert.Equal(game1.GameId, g1.GameId);
            Assert.Equal(game1.Title, g1.Title);
            Assert.Equal(game2.GameId, g2.GameId);
            Assert.Equal(game2.Title, g2.Title);
            Assert.Equal(game3.GameId, g3.GameId);
            Assert.Equal(game3.Title, g3.Title);
        }

        [Fact]
        public void Cannot_Edit_NonExistedGame()
        {
            //Arrange
            var game1 = new Game {GameId = 1, Title = "Game1"};
            var game2 = new Game {GameId = 2, Title = "Game2"};
            var game3 = new Game {GameId = 3, Title = "Game3"};
            var mockRepo = new Mock<IGameRepository>();
            mockRepo.Setup(g => g.Games).Returns(new []
            {   game1,
                game2,
                game3
            }.AsQueryable);
            mockRepo.Setup(g => g.GetGame(game1.GameId)).Returns(game1);
            mockRepo.Setup(g => g.GetGame(game2.GameId)).Returns(game2);
            mockRepo.Setup(g => g.GetGame(game3.GameId)).Returns(game3);
            
            var mockLogger = new Mock<ILogger<GameController>>();
            var mockEnvironment = new Mock<IHostingEnvironment>();

            var controller = new GameController(mockRepo.Object, mockLogger.Object, mockEnvironment.Object);

            //Act
            var result = GetViewModel<CreateEditGameViewModel>(controller.Edit(4));

            //Assert
            Assert.Null(result);
        }
        [Fact]
        public void Can_Delete_Valid_Game()
        {
            //Arrange
            var game = new Game { GameId = 2, Title = "Test"};
            var mockRepo = new Mock<IGameRepository>();
            mockRepo.Setup(m => m.Games).Returns(new [] {
            new Game {GameId = 1, Title = "P1" },
            game,
            new Game { GameId = 3, Title = "P3"}
            }.AsQueryable);
            var mockLogger = new Mock<ILogger<GameController>>();
            var mockEnvironment = new Mock<IHostingEnvironment>();
            
            var controller = new GameController(mockRepo.Object, mockLogger.Object, mockEnvironment.Object);

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
