using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PlayBoardGame.Models;
using Xunit;

namespace PlayBoardGames.Tests
{
    public class ShelfEFTests
    {
        [Fact]
        public void Can_Get_Shelf_For_User()
        {
            //Arrange
            var user1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var user2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
            var user3 = new AppUser {Id = "id3", UserName = "user3", Email = "user3@example.com"};
            var game1 = new Game {GameID = 1, Title = "game1"};
            var game2 = new Game {GameID = 2, Title = "game2"};
            var game3 = new Game {GameID = 3, Title = "game3"};
            var game4 = new Game {GameID = 4, Title = "game4"};
            var shelfItem1 = new GameAppUser {UserId = "id1", GameID = 1};
            var shelfItem2 = new GameAppUser {UserId = "id1", GameID = 2};
            var shelfItem3 = new GameAppUser {UserId = "id1", GameID = 3};
            var shelfItem4 = new GameAppUser {UserId = "id1", GameID = 4};
            var shelfItem5 = new GameAppUser {UserId = "id2", GameID = 1};
            var shelfItem6 = new GameAppUser {UserId = "id2", GameID = 4};
            
            var result1 = new List<Game>();
            var result2 = new List<Game>();
            var result3 = new List<Game>();

            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.Users.Add(user3);
                    context.Games.Add(game1);
                    context.Games.Add(game2);
                    context.Games.Add(game3);
                    context.Games.Add(game4);
                    context.GameAppUser.Add(shelfItem1);
                    context.GameAppUser.Add(shelfItem2);
                    context.GameAppUser.Add(shelfItem3);
                    context.GameAppUser.Add(shelfItem4);
                    context.GameAppUser.Add(shelfItem5);
                    context.GameAppUser.Add(shelfItem6);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var shelfRepository = new EFShelfRepository(context);
                    result1 = shelfRepository.GetShelfForUser("id1").OrderBy(g => g.Title).ToList();
                    result2 = shelfRepository.GetShelfForUser("id2").OrderBy(g => g.Title).ToList();
                    result3 = shelfRepository.GetShelfForUser("id3").OrderBy(g => g.Title).ToList();
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var shelf = context.GameAppUser.ToList();
                    Assert.Equal(3, context.Users.Count());
                    Assert.Equal(4, context.Games.Count());
                    Assert.Equal(6, shelf.Count);
                    Assert.Equal(4, result1.Count);
                    Assert.Equal(1, result1[0].GameID);
                    Assert.Equal(2, result1[1].GameID);
                    Assert.Equal(3, result1[2].GameID);
                    Assert.Equal(4, result1[3].GameID);
                    Assert.Equal(2, result2.Count);
                    Assert.Equal(1, result2[0].GameID);
                    Assert.Equal(4, result2[1].GameID);
                    Assert.Empty(result3);
                }
            }
        }
        
        [Fact]
        public void Can_Get_Available_Games_For_User()
        {
            //Arrange
            var user1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var user2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
            var user3 = new AppUser {Id = "id3", UserName = "user3", Email = "user3@example.com"};
            var game1 = new Game {GameID = 1, Title = "game1"};
            var game2 = new Game {GameID = 2, Title = "game2"};
            var game3 = new Game {GameID = 3, Title = "game3"};
            var game4 = new Game {GameID = 4, Title = "game4"};
            var shelfItem1 = new GameAppUser {UserId = "id1", GameID = 1};
            var shelfItem2 = new GameAppUser {UserId = "id1", GameID = 2};
            var shelfItem3 = new GameAppUser {UserId = "id1", GameID = 3};
            var shelfItem4 = new GameAppUser {UserId = "id1", GameID = 4};
            var shelfItem5 = new GameAppUser {UserId = "id2", GameID = 1};
            var shelfItem6 = new GameAppUser {UserId = "id2", GameID = 4};
            
            var result1 = new List<Game>();
            var result2 = new List<Game>();
            var result3 = new List<Game>();

            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.Users.Add(user3);
                    context.Games.Add(game1);
                    context.Games.Add(game2);
                    context.Games.Add(game3);
                    context.Games.Add(game4);
                    context.GameAppUser.Add(shelfItem1);
                    context.GameAppUser.Add(shelfItem2);
                    context.GameAppUser.Add(shelfItem3);
                    context.GameAppUser.Add(shelfItem4);
                    context.GameAppUser.Add(shelfItem5);
                    context.GameAppUser.Add(shelfItem6);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var shelfRepository = new EFShelfRepository(context);
                    result1 = shelfRepository.GetAvailableGamesForUser("id1").OrderBy(g => g.Title).ToList();
                    result2 = shelfRepository.GetAvailableGamesForUser("id2").OrderBy(g => g.Title).ToList();
                    result3 = shelfRepository.GetAvailableGamesForUser("id3").OrderBy(g => g.Title).ToList();
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var shelf = context.GameAppUser.ToList();
                    Assert.Equal(3, context.Users.Count());
                    Assert.Equal(4, context.Games.Count());
                    Assert.Equal(6, shelf.Count);
                    Assert.Empty(result1);
                    Assert.Equal(2, result2.Count);
                    Assert.Equal(2, result2[0].GameID);
                    Assert.Equal(3, result2[1].GameID);
                    Assert.Equal(4, result3.Count);
                    Assert.Equal(1, result3[0].GameID);
                    Assert.Equal(2, result3[1].GameID);
                    Assert.Equal(3, result3[2].GameID);
                    Assert.Equal(4, result3[3].GameID);
                }
            }
        }
        
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
        
        [Fact]
        public void Can_Remove_From_Shelf()
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
                    context.GameAppUser.Add(shelfItem);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var shelfRepository = new EFShelfRepository(context);
                    shelfRepository.RemoveFromShelf(shelfItem);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Empty(context.GameAppUser);
                    Assert.Single(context.Users);
                    Assert.Single(context.Games);
                }
            }
        }
    }
}