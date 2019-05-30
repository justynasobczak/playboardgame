using System.Linq;
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
            using (var factory = new SQLiteDbContextFactory())
            {
                //Act
                // Run the test against one instance of the context
                using (var context = factory.CreateContext())
                {
                    var gameRepository = new EFGameRepository(context);
                    gameRepository.SaveGame(new Game {Title = "TestToAdd"});
                }

                //Assert
                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(1, context.Games.Count());
                    Assert.Equal("TestToAdd", context.Games.Single().Title);
                }
            }
        }

        [Fact]
        public void Can_Edit_Game()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    context.Games.Add(new Game {GameID = 1, Title = "TestToModify"});
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var gameRepository = new EFGameRepository(context);
                    gameRepository.SaveGame(new Game {GameID = 1, Title = "TestAfterModification"});
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(1, context.Games.Count());
                    Assert.Equal("TestAfterModification", context.Games.Single().Title);
                    Assert.Equal(1, context.Games.Single().GameID);
                }
            }
        }

        [Fact]
        public void Can_Delete_Game()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    context.Games.Add(new Game {GameID = 1, Title = "TestToDelete"});
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var gameRepository = new EFGameRepository(context);
                    gameRepository.DeleteGame(1);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(0, context.Games.Count());
                }
            }
        }
    }
}