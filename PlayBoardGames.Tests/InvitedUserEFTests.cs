using System.Linq;
using PlayBoardGame.Models;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace PlayBoardGames.Tests
{
    public class InvitedUserEFTests
    {
        [Fact]
        public void Can_Add_User_To_Meeting()
        {
            //Arrange
            Mock<UserManager<AppUser>> GetMockUserManager()
            {
                var userStoreMock = new Mock<IUserStore<AppUser>>();
                return new Mock<UserManager<AppUser>>(
                    userStoreMock.Object, null, null, null, null, null, null, null, null);
            }

            var user = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var meeting = new Meeting {MeetingID = 1, Title = "meeting1"};

            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(user);
                    context.Meetings.Add(meeting);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var invitedUserRepository = new EFInvitedUserRepository(context, GetMockUserManager().Object);
                    invitedUserRepository.AddUserToMeeting(user.Id, meeting.MeetingID, InvitationStatus.Pending);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var invitedUsers = context.MeetingInvitedUser
                        .Include(mi => mi.AppUser)
                        .Include(mi => mi.Meeting)
                        .ToList();
                    Assert.Single(invitedUsers);
                    Assert.Equal(user.Id, invitedUsers.Single().AppUser.Id);
                    Assert.Equal(user.Id, invitedUsers.Single().UserId);
                    Assert.Equal(meeting.MeetingID, invitedUsers.Single().Meeting.MeetingID);
                    Assert.Equal(meeting.MeetingID, invitedUsers.Single().MeetingID);
                }
            }
        }

        [Fact]
        public void Can_Remove_User_From_Meeting()
        {
            //Arrange
            Mock<UserManager<AppUser>> GetMockUserManager()
            {
                var userStoreMock = new Mock<IUserStore<AppUser>>();
                return new Mock<UserManager<AppUser>>(
                    userStoreMock.Object, null, null, null, null, null, null, null, null);
            }

            var user = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var meeting = new Meeting {MeetingID = 1, Title = "meeting1"};

            using (var factory = new SQLiteDbContextFactory())
            {
                var invitedUser = new MeetingInvitedUser
                {
                    Meeting = meeting,
                    AppUser = user,
                    Status = InvitationStatus.Pending
                };
                
                using (var context = factory.CreateContext())
                {
                    context.MeetingInvitedUser.Add(invitedUser);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var invitedUserRepository = new EFInvitedUserRepository(context, GetMockUserManager().Object);
                    invitedUserRepository.RemoveUserFromMeeting(invitedUser.UserId, invitedUser.MeetingID);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var invitedUsers = context.MeetingInvitedUser.ToList();
                    Assert.Empty(invitedUsers);
                }
            }
        }
        
        [Fact]
        public void Can_Change_Status()
        {
            //Arrange
            Mock<UserManager<AppUser>> GetMockUserManager()
            {
                var userStoreMock = new Mock<IUserStore<AppUser>>();
                return new Mock<UserManager<AppUser>>(
                    userStoreMock.Object, null, null, null, null, null, null, null, null);
            }

            var user = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var meeting = new Meeting {MeetingID = 1, Title = "meeting1"};

            using (var factory = new SQLiteDbContextFactory())
            {
                var invitedUser = new MeetingInvitedUser
                {
                    Meeting = meeting,
                    AppUser = user,
                    Status = InvitationStatus.Pending
                };
                
                using (var context = factory.CreateContext())
                {
                    context.MeetingInvitedUser.Add(invitedUser);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var invitedUserRepository = new EFInvitedUserRepository(context, GetMockUserManager().Object);
                    invitedUserRepository.ChangeStatus(invitedUser.UserId, invitedUser.MeetingID, InvitationStatus.Accepted);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var invitedUsers = context.MeetingInvitedUser.ToList();
                    Assert.Equal(InvitationStatus.Accepted, invitedUsers.Single().Status);
                }
                
                //Act
                using (var context = factory.CreateContext())
                {
                    var invitedUserRepository = new EFInvitedUserRepository(context, GetMockUserManager().Object);
                    invitedUserRepository.ChangeStatus(invitedUser.UserId, invitedUser.MeetingID, InvitationStatus.Rejected);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var invitedUsers = context.MeetingInvitedUser.ToList();
                    Assert.Equal(InvitationStatus.Rejected, invitedUsers.Single().Status);
                }
                
                //Act
                using (var context = factory.CreateContext())
                {
                    var invitedUserRepository = new EFInvitedUserRepository(context, GetMockUserManager().Object);
                    invitedUserRepository.ChangeStatus(invitedUser.UserId, invitedUser.MeetingID, InvitationStatus.Cancelled);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var invitedUsers = context.MeetingInvitedUser.ToList();
                    Assert.Equal(InvitationStatus.Cancelled, invitedUsers.Single().Status);
                }
            }
        }
    }
}