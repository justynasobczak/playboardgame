using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PlayBoardGame.Models;
using Xunit;

namespace PlayBoardGames.Tests
{
    public class GameEFTests
    {
        [Fact]
        public void Can_Add_Game()
        {
            //Arrange
            var connection = TestBase.OpenConnection();

            try
            {
                var options = TestBase.CreateDbContextOptions(connection);

                // Create the schema in the database
                using (var context = new ApplicationDBContext(options))
                {
                    context.Database.EnsureCreated();
                }
                
                //Act
                // Run the test against one instance of the context
                using (var context = new ApplicationDBContext(options))
                {
                    var gameRepository = new EFGameRepository(context);
                    gameRepository.SaveGame(new Game {Title = "TestToAdd"});
                }

                //Assert
                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new ApplicationDBContext(options))
                {
                    Assert.Equal(1, context.Games.Count());
                    Assert.Equal("TestToAdd", context.Games.Single().Title);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public void Can_Edit_Game()
        {
            //Arrange
            var connection = TestBase.OpenConnection();

            try
            {
                var options = TestBase.CreateDbContextOptions(connection);

                using (var context = new ApplicationDBContext(options))
                {
                    context.Database.EnsureCreated();
                    context.Games.Add(new Game {GameID = 1, Title = "TestToModify"});
                    context.SaveChanges();
                }

                //Act
                using (var context = new ApplicationDBContext(options))
                {
                    var gameRepository = new EFGameRepository(context);
                    gameRepository.SaveGame(new Game {GameID = 1, Title = "TestAfterModification"});
                }

                //Assert
                using (var context = new ApplicationDBContext(options))
                {
                    Assert.Equal(1, context.Games.Count());
                    Assert.Equal("TestAfterModification", context.Games.Single().Title);
                    Assert.Equal(1, context.Games.Single().GameID);
                }
            }
            finally
            {
                connection.Close();
            }
        }
        
        [Fact]
        public void Can_Delete_Game()
        {
            //Arrange
            var connection = TestBase.OpenConnection();

            try
            {
                var options = TestBase.CreateDbContextOptions(connection);

                using (var context = new ApplicationDBContext(options))
                {
                    context.Database.EnsureCreated();
                    context.Games.Add(new Game {GameID = 1, Title = "TestToDelete"});
                    context.SaveChanges();
                }

                //Act
                using (var context = new ApplicationDBContext(options))
                {
                    var gameRepository = new EFGameRepository(context);
                    gameRepository.DeleteGame(1);
                }

                //Assert
                using (var context = new ApplicationDBContext(options))
                {
                    Assert.Equal(0, context.Games.Count());
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}