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
            var meeting = new Meeting {MeetingId = 1, Title = "meeting1"};
            var result = new List<MeetingInvitedUser>();

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
                    result = invitedUserRepository.GetInvitedUsersList(meeting.MeetingId).OrderBy(mu => mu.UserId).ToList();
                    Assert.Equal(5, result.Count);
                    Assert.Equal(InvitationStatus.Pending, result[0].Status);
                    Assert.Equal(InvitationStatus.Accepted, result[1].Status);
                    Assert.Equal(InvitationStatus.Rejected, result[2].Status);
                    Assert.Equal(InvitationStatus.Cancelled, result[3].Status);
                    Assert.Equal(InvitationStatus.Pending, result[4].Status);
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
            var meeting = new Meeting {MeetingId = 1, Title = "meeting1"};

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
                    invitedUserRepository.AddUserToMeeting(user.Id, meeting.MeetingId, InvitationStatus.Pending);
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
                    Assert.Equal(meeting.MeetingId, invitedUsers.Single().Meeting.MeetingId);
                    Assert.Equal(meeting.MeetingId, invitedUsers.Single().MeetingId);
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
            var meeting = new Meeting {MeetingId = 1, Title = "meeting1"};

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
                    invitedUserRepository.RemoveUserFromMeeting(invitedUser.UserId, invitedUser.MeetingId);
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
            var meeting = new Meeting {MeetingId = 1, Title = "meeting1"};

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
                    invitedUserRepository.ChangeStatus(invitedUser.UserId, invitedUser.MeetingId, InvitationStatus.Accepted);
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
                    invitedUserRepository.ChangeStatus(invitedUser.UserId, invitedUser.MeetingId, InvitationStatus.Rejected);
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
                    invitedUserRepository.ChangeStatus(invitedUser.UserId, invitedUser.MeetingId, InvitationStatus.Cancelled);
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