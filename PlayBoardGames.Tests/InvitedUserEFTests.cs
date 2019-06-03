using System.Collections.Generic;
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
        public void Can_Get_Invited_Users_List()
        {
            //Arrange
            Mock<UserManager<AppUser>> GetMockUserManager()
            {
                var userStoreMock = new Mock<IUserStore<AppUser>>();
                return new Mock<UserManager<AppUser>>(
                    userStoreMock.Object, null, null, null, null, null, null, null, null);
            }

            var user1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var user2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
            var user3 = new AppUser {Id = "id3", UserName = "user3", Email = "user3@example.com"};
            var user4 = new AppUser {Id = "id4", UserName = "user4", Email = "user4@example.com"};
            var user5 = new AppUser {Id = "id5", UserName = "user5", Email = "user5@example.com"};
            var meeting = new Meeting {MeetingID = 1, Title = "meeting1"};
            var result = new Dictionary<string, InvitationStatus>();

            using (var factory = new SQLiteDbContextFactory())
            {
                var invitedUser1 = new MeetingInvitedUser
                {
                    Meeting = meeting,
                    AppUser = user1,
                    Status = InvitationStatus.Pending
                };
                
                var invitedUser2 = new MeetingInvitedUser
                {
                    Meeting = meeting,
                    AppUser = user2,
                    Status = InvitationStatus.Accepted
                };
                
                var invitedUser3 = new MeetingInvitedUser
                {
                    Meeting = meeting,
                    AppUser = user3,
                    Status = InvitationStatus.Rejected
                };
                
                var invitedUser4 = new MeetingInvitedUser
                {
                    Meeting = meeting,
                    AppUser = user4,
                    Status = InvitationStatus.Cancelled
                };
                
                var invitedUser5 = new MeetingInvitedUser
                {
                    Meeting = meeting,
                    AppUser = user5,
                    Status = InvitationStatus.Pending
                };
                
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.Users.Add(user3);
                    context.Users.Add(user4);
                    context.Users.Add(user5);
                    context.Meetings.Add(meeting);
                    context.MeetingInvitedUser.Add(invitedUser1);
                    context.MeetingInvitedUser.Add(invitedUser2);
                    context.MeetingInvitedUser.Add(invitedUser3);
                    context.MeetingInvitedUser.Add(invitedUser4);
                    context.MeetingInvitedUser.Add(invitedUser5);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {                
                    var invitedUserRepository = new EFInvitedUserRepository(context, GetMockUserManager().Object);
                    result = invitedUserRepository.GetInvitedUsersList(meeting.MeetingID);
                    Assert.Equal(5, result.Count);
                    Assert.Equal(InvitationStatus.Pending, result["id1"]);
                    Assert.Equal(InvitationStatus.Accepted, result["id2"]);
                    Assert.Equal(InvitationStatus.Rejected, result["id3"]);
                    Assert.Equal(InvitationStatus.Cancelled, result["id4"]);
                    Assert.Equal(InvitationStatus.Pending, result["id5"]);
                }
            }
        }
        
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