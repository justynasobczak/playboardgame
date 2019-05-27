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
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new ApplicationDBContext(options))
                {
                    context.Database.EnsureCreated();
                }

                // Run the test against one instance of the context
                using (var context = new ApplicationDBContext(options))
                {   
                    var gameRepository = new EFGameRepository(context);
                    gameRepository.SaveGame(new Game { Title = "Test"});
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new ApplicationDBContext(options))
                {
                    Assert.Equal(1, context.Games.Count());
                    Assert.Equal("Test", context.Games.Single().Title);
                }
            }
            finally

            {
                connection.Close();
            }
        }
    }
}