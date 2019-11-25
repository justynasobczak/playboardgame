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
                    context.Games.Add(new Game {GameId = 1, Title = "TestToModify"});
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var gameRepository = new EFGameRepository(context);
                    gameRepository.SaveGame(new Game {GameId = 1, Title = "TestAfterModification"});
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(1, context.Games.Count());
                    Assert.Equal("TestAfterModification", context.Games.Single().Title);
                    Assert.Equal(1, context.Games.Single().GameId);
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
                    context.Games.Add(new Game {GameId = 1, Title = "TestToDelete"});
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

        [Fact]
        public void Can_Check_Is_Game_In_Meeting()
        {
            //Arrange
            var game1 = new Game {GameId = 1, Title = "game1"};
            var game2 = new Game {GameId = 2, Title = "game2"};
            var game3 = new Game {GameId = 3, Title = "game3"};

            var user1 = new AppUser
                {Id = "1", UserName = "user1", Email = "user1@example.com"};
            var user2 = new AppUser
                {Id = "2", UserName = "user2", Email = "user2@example.com"};

            var meeting1 = new Meeting
                {Title = "Meeting1", Organizer = user1};
            var meeting2 = new Meeting
                {Title = "Meeting2", Organizer = user1};

            var meetingGame1 = new MeetingGame
                {Game = game2, Meeting = meeting1};
            var meetingGame2 = new MeetingGame
                {Game = game3, Meeting = meeting1};
            var meetingGame3 = new MeetingGame
                {Game = game3, Meeting = meeting2};

            using (var factory = new SQLiteDbContextFactory())
            {
                bool result1;
                bool result2;
                bool result3;
                using (var context = factory.CreateContext())
                {
                    context.Games.Add(game1);
                    context.Games.Add(game2);
                    context.Games.Add(game3);
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.SaveChanges();
                    context.Meetings.Add(meeting1);
                    context.Meetings.Add(meeting2);
                    context.SaveChanges();
                    context.MeetingGame.Add(meetingGame1);
                    context.MeetingGame.Add(meetingGame2);
                    context.MeetingGame.Add(meetingGame3);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var gameRepository = new EFGameRepository(context);
                    result1 = gameRepository.IsGameInMeeting(game1.GameId);
                    result2 = gameRepository.IsGameInMeeting(game2.GameId);
                    result3 = gameRepository.IsGameInMeeting(game3.GameId);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(3, context.Games.Count());
                    Assert.Equal(2, context.Users.Count());
                    Assert.Equal(2, context.Meetings.Count());
                    Assert.Equal(3, context.MeetingGame.Count());
                    Assert.False(result1);
                    Assert.True(result2);
                    Assert.True(result3);
                }
            }
        }
    }
}