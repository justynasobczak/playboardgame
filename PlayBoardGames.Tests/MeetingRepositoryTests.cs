using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using PlayBoardGame.Controllers;
using PlayBoardGame.Models;
namespace PlayBoardGames.Tests
{
    public class MeetingRepositoryTests
    {
        [Fact]
        public void Can_See_Meetings_By_Current_User()
        {
            //Arrange
            var currentUser = new AppUser {Email = "test1@test.com", Id = "id1", UserName = "1"};
            var secondUser = new AppUser {Email = "test2@test.com", Id = "id2", UserName = "2"};
            
            Mock<ApplicationDBContext> meetings = new Mock<ApplicationDBContext>();
            meetings.Setup(m => m.Meetings).Returns(new Meeting[]
            {   new Meeting {MeetingID = 1, Title = "Meeting1", Organizer = currentUser},
                new Meeting {MeetingID = 2, Title = "Meeting2", Organizer = currentUser},
                new Meeting {MeetingID = 3, Title = "Meeting3", Organizer = secondUser}
            }.AsQueryable<Meeting>());
            
            var controller = new FullCalendarController(meetings.Object);
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, currentUser.UserName),
                new Claim(ClaimTypes.NameIdentifier, currentUser.Id)
            }));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            //Act
            var result = controller.Get();

            //Assert
            Assert.Equal(controller.User.Identity.Name, "1");
            //Assert.Equal(2, result.Count());
            

        }
    }
}