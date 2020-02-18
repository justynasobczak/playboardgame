using System.Linq;
using Microsoft.EntityFrameworkCore;
using PlayBoardGame.Models;
using Xunit;

namespace PlayBoardGames.Tests
{
    public class FriendInvitationEFTests
    {
        [Fact]
        public void Can_Get_Friends_Of_CurrentUser()
        {
            //Arrange
            var user1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var user2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
            var user3 = new AppUser {Id = "id3", UserName = "user3", Email = "user3@example.com"};
            var user4 = new AppUser {Id = "id4", UserName = "user4", Email = "user4@example.com"};
            var user5 = new AppUser {Id = "id5", UserName = "user5", Email = "user5@example.com"};

            var request1 = new FriendInvitation
            {
                Sender = user1,
                Status = FriendInvitationStatus.Pending,
                Invited = user3,
                InvitedEmail = user3.Email
            };

            var request2 = new FriendInvitation
            {
                Sender = user4,
                Status = FriendInvitationStatus.Rejected,
                Invited = user1,
                InvitedEmail = user1.Email
            };

            var request3 = new FriendInvitation
            {
                Sender = user1,
                Status = FriendInvitationStatus.Pending,
                InvitedEmail = "email@invited.com"
            };

            var request4 = new FriendInvitation
            {
                Sender = user1,
                Status = FriendInvitationStatus.Accepted,
                Invited = user2,
                InvitedEmail = user2.Email
            };

            var request5 = new FriendInvitation
            {
                Sender = user5,
                Status = FriendInvitationStatus.Accepted,
                Invited = user1,
                InvitedEmail = user1.Email
            };

            using var factory = new SQLiteDbContextFactory();
            using (var context = factory.CreateContext())
            {
                context.Users.Add(user1);
                context.Users.Add(user2);
                context.Users.Add(user3);
                context.Users.Add(user4);
                context.Users.Add(user5);
                context.FriendInvitations.Add(request1);
                context.FriendInvitations.Add(request2);
                context.FriendInvitations.Add(request3);
                context.FriendInvitations.Add(request4);
                context.FriendInvitations.Add(request5);
                context.SaveChanges();
            }

            //Act
            using (var context = factory.CreateContext())
            {
                var friendInvitationRepository = new EFFriendInvitationRepository(context);
                var result1 = friendInvitationRepository.GetFriendsOfCurrentUser(user1.Id).OrderBy(u => u.Id).ToList();
                var result2 = friendInvitationRepository.GetFriendsOfCurrentUser(user2.Id).OrderBy(u => u.Id).ToList();
                var result3 = friendInvitationRepository.GetFriendsOfCurrentUser(user3.Id).OrderBy(u => u.Id).ToList();
                //Assert
                Assert.Equal(5, context.Users.Count());
                Assert.Equal(5, context.FriendInvitations.Count());
                Assert.Equal(2, result1.Count);
                Assert.Equal(user2.Id, result1[0].Id);
                Assert.Equal(user5.Id, result1[1].Id);
                Assert.Single(result2);
                Assert.Equal(user1.Id, result2[0].Id);
                Assert.Empty(result3);
            }
        }

        [Fact]
        public void Can_Get_Invitations_Sent_By_CurrentUser()
        {
            //Arrange
            var user1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var user2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
            var user3 = new AppUser {Id = "id3", UserName = "user3", Email = "user3@example.com"};
            var user4 = new AppUser {Id = "id4", UserName = "user4", Email = "user4@example.com"};
            var user5 = new AppUser {Id = "id5", UserName = "user5", Email = "user5@example.com"};

            var request1 = new FriendInvitation
            {
                FriendInvitationId = 1,
                Sender = user1,
                Status = FriendInvitationStatus.Pending,
                Invited = user3,
                InvitedEmail = user3.Email
            };

            var request2 = new FriendInvitation
            {
                FriendInvitationId = 2,
                Sender = user4,
                Status = FriendInvitationStatus.Rejected,
                Invited = user1,
                InvitedEmail = user1.Email
            };

            var request3 = new FriendInvitation
            {
                FriendInvitationId = 3,
                Sender = user1,
                Status = FriendInvitationStatus.Pending,
                InvitedEmail = "email@invited.com"
            };

            var request4 = new FriendInvitation
            {
                FriendInvitationId = 4,
                Sender = user1,
                Status = FriendInvitationStatus.Accepted,
                Invited = user2,
                InvitedEmail = user2.Email
            };

            var request5 = new FriendInvitation
            {
                FriendInvitationId = 5,
                Sender = user5,
                Status = FriendInvitationStatus.Accepted,
                Invited = user1,
                InvitedEmail = user1.Email
            };

            using var factory = new SQLiteDbContextFactory();
            using (var context = factory.CreateContext())
            {
                context.Users.Add(user1);
                context.Users.Add(user2);
                context.Users.Add(user3);
                context.Users.Add(user4);
                context.Users.Add(user5);
                context.FriendInvitations.Add(request1);
                context.FriendInvitations.Add(request2);
                context.FriendInvitations.Add(request3);
                context.FriendInvitations.Add(request4);
                context.FriendInvitations.Add(request5);
                context.SaveChanges();
            }

            //Act
            using (var context = factory.CreateContext())
            {
                var friendInvitationRepository = new EFFriendInvitationRepository(context);
                var result1 = friendInvitationRepository.GetInvitationsSentByCurrentUser(user1.Id)
                    .OrderBy(i => i.FriendInvitationId).ToList();
                var result2 = friendInvitationRepository.GetInvitationsSentByCurrentUser(user2.Id)
                    .OrderBy(i => i.FriendInvitationId).ToList();
                var result3 = friendInvitationRepository.GetInvitationsSentByCurrentUser(user5.Id)
                    .OrderBy(i => i.FriendInvitationId).ToList();
                Assert.Equal(5, context.Users.Count());
                Assert.Equal(5, context.FriendInvitations.Count());
                Assert.Equal(3, result1.Count);
                Assert.Equal(request1.FriendInvitationId, result1[0].FriendInvitationId);
                Assert.Equal(request3.FriendInvitationId, result1[1].FriendInvitationId);
                Assert.Equal(request4.FriendInvitationId, result1[2].FriendInvitationId);
                Assert.Empty(result2);
                Assert.Single(result3);
                Assert.Equal(request5.FriendInvitationId, result3[0].FriendInvitationId);
            }
        }

        [Fact]
        public void Can_Get_Invitations_Received_By_CurrentUser()
        {
            //Arrange
            var user1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var user2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
            var user3 = new AppUser {Id = "id3", UserName = "user3", Email = "user3@example.com"};
            var user4 = new AppUser {Id = "id4", UserName = "user4", Email = "user4@example.com"};
            var user5 = new AppUser {Id = "id5", UserName = "user5", Email = "user5@example.com"};

            var request1 = new FriendInvitation
            {
                FriendInvitationId = 1,
                Sender = user1,
                Status = FriendInvitationStatus.Pending,
                Invited = user3,
                InvitedEmail = user3.Email
            };

            var request2 = new FriendInvitation
            {
                FriendInvitationId = 2,
                Sender = user4,
                Status = FriendInvitationStatus.Rejected,
                Invited = user1,
                InvitedEmail = user1.Email
            };

            var request3 = new FriendInvitation
            {
                FriendInvitationId = 3,
                Sender = user1,
                Status = FriendInvitationStatus.Pending,
                InvitedEmail = "user1@example.com"
            };

            var request4 = new FriendInvitation
            {
                FriendInvitationId = 4,
                Sender = user1,
                Status = FriendInvitationStatus.Accepted,
                Invited = user2,
                InvitedEmail = user2.Email
            };

            var request5 = new FriendInvitation
            {
                FriendInvitationId = 5,
                Sender = user5,
                Status = FriendInvitationStatus.Accepted,
                Invited = user1,
                InvitedEmail = user1.Email
            };

            using var factory = new SQLiteDbContextFactory();
            using (var context = factory.CreateContext())
            {
                context.Users.Add(user1);
                context.Users.Add(user2);
                context.Users.Add(user3);
                context.Users.Add(user4);
                context.Users.Add(user5);
                context.FriendInvitations.Add(request1);
                context.FriendInvitations.Add(request2);
                context.FriendInvitations.Add(request3);
                context.FriendInvitations.Add(request4);
                context.FriendInvitations.Add(request5);
                context.SaveChanges();
            }

            //Act
            using (var context = factory.CreateContext())
            {
                var friendInvitationRepository = new EFFriendInvitationRepository(context);
                var result1 = friendInvitationRepository.GetInvitationsReceivedByCurrentUser(user1.Email)
                    .OrderBy(i => i.FriendInvitationId).ToList();
                var result2 = friendInvitationRepository.GetInvitationsReceivedByCurrentUser(user3.Email)
                    .OrderBy(i => i.FriendInvitationId).ToList();
                var result3 = friendInvitationRepository.GetInvitationsReceivedByCurrentUser(user5.Email)
                    .OrderBy(i => i.FriendInvitationId).ToList();
                Assert.Equal(5, context.Users.Count());
                Assert.Equal(5, context.FriendInvitations.Count());
                Assert.Equal(3, result1.Count);
                Assert.Equal(request2.FriendInvitationId, result1[0].FriendInvitationId);
                Assert.Equal(request3.FriendInvitationId, result1[1].FriendInvitationId);
                Assert.Equal(request5.FriendInvitationId, result1[2].FriendInvitationId);
                Assert.Single(result2);
                Assert.Equal(request1.FriendInvitationId, result2[0].FriendInvitationId);
                Assert.Empty(result3);
            }
        }

        [Fact]
        public void Can_Add_Invitation()
        {
            //Arrange
            using var factory = new SQLiteDbContextFactory();
            //Arrange
            var user1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var user2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
            var user3 = new AppUser {Id = "id3", UserName = "user3", Email = "user3@example.com"};
            var request1ToAdd = new FriendInvitation
            {
                FriendInvitationId = 1,
                Sender = user1,
                Invited = user2,
                InvitedEmail = user2.Email
            };

            var request2ToAdd = new FriendInvitation
            {
                FriendInvitationId = 2,
                Sender = user3,
                InvitedEmail = "user4@example.com"
            };

            //Act
            using (var context = factory.CreateContext())
            {
                context.Users.Add(user1);
                context.Users.Add(user2);
                context.Users.Add(user3);
                context.SaveChanges();
                var friendInvitationRepository = new EFFriendInvitationRepository(context);
                friendInvitationRepository.AddInvitation(request1ToAdd);
                friendInvitationRepository.AddInvitation(request2ToAdd);
            }

            using (var context = factory.CreateContext())
            {
                var invitations = context.FriendInvitations
                    .Include(i => i.Invited)
                    .Include(i => i.Sender)
                    .OrderBy(i => i.FriendInvitationId).ToList();
                Assert.Equal(3, context.Users.Count());
                Assert.Equal(2, context.FriendInvitations.Count());
                Assert.Equal(request1ToAdd.FriendInvitationId, invitations[0].FriendInvitationId);
                Assert.Equal(request2ToAdd.FriendInvitationId, invitations[1].FriendInvitationId);

                Assert.Equal(FriendInvitationStatus.Pending, invitations[0].Status);
                Assert.Equal(FriendInvitationStatus.Pending, invitations[1].Status);

                Assert.Equal(request1ToAdd.Sender.Id, invitations[0].SenderId);
                Assert.Equal(request2ToAdd.Sender.Id, invitations[1].SenderId);

                Assert.Equal(request1ToAdd.Invited.Id, invitations[0].InvitedId);
                Assert.Null(invitations[1].InvitedId);

                Assert.Equal(request1ToAdd.InvitedEmail, invitations[0].InvitedEmail);
                Assert.Equal("user4@example.com", invitations[1].InvitedEmail);
            }
        }
        
        [Fact]
        public void Can_Change_Status()
        {
            //Arrange
            using var factory = new SQLiteDbContextFactory();
            //Arrange
            var user1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
            var user2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
            var user3 = new AppUser {Id = "id3", UserName = "user3", Email = "user3@example.com"};
            var user4 = new AppUser {Id = "id4", UserName = "user4", Email = "user4@example.com"};
            var request1 = new FriendInvitation
            {
                FriendInvitationId = 1,
                Sender = user1,
                Invited = user2,
                InvitedEmail = user2.Email,
                Status = FriendInvitationStatus.Pending
            };

            var request2 = new FriendInvitation
            {
                FriendInvitationId = 2,
                Sender = user3,
                InvitedEmail = "user4@example.com",
                Status = FriendInvitationStatus.Pending
            };

            //Act
            using (var context = factory.CreateContext())
            {
                context.Users.Add(user1);
                context.Users.Add(user2);
                context.Users.Add(user3);
                context.Users.Add(user4);
                context.SaveChanges();
                context.FriendInvitations.Add(request1);
                context.FriendInvitations.Add(request2);
                context.SaveChanges();
                var friendInvitationRepository = new EFFriendInvitationRepository(context);
                friendInvitationRepository.ChangeStatus(request1.FriendInvitationId, FriendInvitationStatus.Accepted, user2);
                friendInvitationRepository.ChangeStatus(request2.FriendInvitationId, FriendInvitationStatus.Rejected, user4);
                
            }

            using (var context = factory.CreateContext())
            {
                var invitations = context.FriendInvitations
                    .Include(i => i.Invited)
                    .Include(i => i.Sender)
                    .OrderBy(i => i.FriendInvitationId).ToList();
                Assert.Equal(4, context.Users.Count());
                Assert.Equal(2, context.FriendInvitations.Count());
                Assert.Equal(request1.FriendInvitationId, invitations[0].FriendInvitationId);
                Assert.Equal(request2.FriendInvitationId, invitations[1].FriendInvitationId);

                Assert.Equal(FriendInvitationStatus.Accepted, invitations[0].Status);
                Assert.Equal(FriendInvitationStatus.Rejected, invitations[1].Status);

                Assert.Equal(request1.Sender.Id, invitations[0].SenderId);
                Assert.Equal(request2.Sender.Id, invitations[1].SenderId);

                Assert.Equal(request1.Invited.Id, invitations[0].InvitedId);
                Assert.Equal(user4.Id, invitations[1].InvitedId);

                Assert.Equal(request1.InvitedEmail, invitations[0].InvitedEmail);
                Assert.Equal(user4.Email, invitations[1].InvitedEmail);
            }
        }
    }
}