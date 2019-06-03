using System.Linq;
using Microsoft.EntityFrameworkCore;
using PlayBoardGame.Models;
using Xunit;

namespace PlayBoardGames.Tests
{
    public class ShelfEFTests
    {
        [Fact]
        public void Can_Add_To_Shelf()
        {
            //Arrange
            var user = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var game = new Game {GameID = 1, Title = "game1"};
            var shelfItem = new GameAppUser {UserId = "id1", GameID = 1};

            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(user);
                    context.Games.Add(game);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var shelfRepository = new EFShelfRepository(context);
                    shelfRepository.AddToShelf(shelfItem);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var shelf = context.GameAppUser
                        .Include(gu => gu.AppUser)
                        .Include(gu => gu.Game)
                        .ToList();
                    Assert.Single(shelf);
                    Assert.Equal(user.Id, shelf.Single().AppUser.Id);
                    Assert.Equal(user.Id, shelf.Single().UserId);
                    Assert.Equal(game.GameID, shelf.Single().Game.GameID);
                    Assert.Equal(game.GameID, shelf.Single().GameID);
                }
            }
        }
    }
}