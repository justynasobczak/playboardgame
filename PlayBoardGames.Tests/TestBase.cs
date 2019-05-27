using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PlayBoardGame.Models;

namespace PlayBoardGames.Tests
{
    public class TestBase
    {
        public static SqliteConnection OpenConnection()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        }

        public static DbContextOptions<ApplicationDBContext> CreateDbContextOptions(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseSqlite(connection)
                .Options;
            return options;
        }
    }
}