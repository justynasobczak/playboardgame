using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PlayBoardGame;
using PlayBoardGame.Models;
using Xunit;

namespace PlayBoardGames.Tests
{
    public class MeetingEFTests
    {
        [Fact]
        public void Can_Add_Meeting()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                //Arrange
                var organizer = new AppUser
                    {Id = "id1", UserName = "organizer", Email = "organizer@example.com"};
                var meetingToAdd = new Meeting
                {
                    Title = "TestToAdd",
                    StartDateTime = DateTime.Today.AddDays(1).AddHours(5),
                    EndDateTime = DateTime.Today.AddDays(1).AddHours(8),
                    Organizer = organizer,
                    City = "City",
                    Street = "Street",
                    PostalCode = "PostalCode",
                    Country = "AU",
                    Notes = "notest test notest test"
                };

                //Act
                // Run the test against one instance of the context
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(organizer);
                    context.SaveChanges();
                    var meetingRepository = new EFMeetingRepository(context);
                    meetingRepository.SaveMeeting(meetingToAdd);
                }

                using (var context = factory.CreateContext())
                {
                    var meetings = context.Meetings
                        .Include(m => m.Organizer)
                        .ToList();
                    var users = context.Users.ToList();
                    Assert.Single(users);
                    Assert.Single(meetings);

                    Assert.Equal(meetingToAdd.Title, meetings.Single().Title);
                    Assert.Equal(meetingToAdd.StartDateTime, meetings.Single().StartDateTime);
                    Assert.Equal(meetingToAdd.EndDateTime, meetings.Single().EndDateTime);
                    Assert.Equal(meetingToAdd.City, meetings.Single().City);
                    Assert.Equal(meetingToAdd.Street, meetings.Single().Street);
                    Assert.Equal(meetingToAdd.PostalCode, meetings.Single().PostalCode);
                    Assert.Equal(meetingToAdd.Country, meetings.Single().Country);
                    Assert.Equal(meetingToAdd.Notes, meetings.Single().Notes);
                    Assert.NotNull(meetings.Single().Organizer);
                    Assert.Equal(organizer.Id, meetings.Single().Organizer.Id);
                }
            }
        }

        [Fact]
        public void Can_Edit_Meeting()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var organizer1 = new AppUser
                    {Id = "1", UserName = "organizer1", Email = "organizer1@example.com"};
                var organizer2 = new AppUser
                    {Id = "2", UserName = "organizer2", Email = "organizer2@example.com"};

                var meetingToEdit = new Meeting
                {
                    MeetingId = 1,
                    Title = "TestToEdit",
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(2),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(4),
                    Organizer = organizer2,
                    City = "City2",
                    Street = "Street2",
                    PostalCode = "PostalCode2",
                    Country = "PL",
                    Notes = "Notes2"
                };

                using (var context = factory.CreateContext())
                {
                    context.Users.Add(organizer1);
                    context.Users.Add(organizer2);
                    context.SaveChanges();

                    context.Meetings.Add(new Meeting
                    {
                        MeetingId = 1,
                        Title = "TestToAdd",
                        StartDateTime = DateTime.Today.AddDays(1).AddHours(5),
                        EndDateTime = DateTime.Today.AddDays(1).AddHours(8),
                        Organizer = organizer1,
                        City = "City",
                        Street = "Street",
                        PostalCode = "PostalCode",
                        Country = "PL",
                        Notes = "Notes"
                    });
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    meetingRepository.SaveMeeting(meetingToEdit);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var meetings = context.Meetings
                        .Include(m => m.Organizer)
                        .ToList();
                    var users = context.Users.ToList();
                    Assert.Equal(2, users.Count);
                    Assert.Single(meetings);

                    Assert.Equal(meetingToEdit.Title, meetings.Single().Title);
                    Assert.Equal(meetingToEdit.StartDateTime, meetings.Single().StartDateTime);
                    Assert.Equal(meetingToEdit.EndDateTime, meetings.Single().EndDateTime);
                    Assert.Equal(meetingToEdit.City, meetings.Single().City);
                    Assert.Equal(meetingToEdit.Street, meetings.Single().Street);
                    Assert.Equal(meetingToEdit.PostalCode, meetings.Single().PostalCode);
                    Assert.Equal(meetingToEdit.Country, meetings.Single().Country);
                    Assert.NotNull(meetings.Single().Organizer);
                    Assert.Equal(organizer2.Id, meetings.Single().Organizer.Id);
                    Assert.Equal(meetingToEdit.Notes, meetings.Single().Notes);
                }
            }
        }

        [Fact]
        public void Can_Add_Game_To_Meeting()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var game = new Game
                    {GameId = 1, Title = "game1"};

                var meeting = new Meeting
                {
                    MeetingId = 1,
                    Title = "TestToAddGame"
                };

                var gameInMeeting = new MeetingGame
                {
                    Game = game,
                    Meeting = meeting
                };

                using (var context = factory.CreateContext())
                {
                    context.Games.Add(game);
                    context.Meetings.Add(meeting);
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    meetingRepository.AddGameToMeeting(gameInMeeting);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var gameInMeetings = context.MeetingGame
                        .Include(mg => mg.Meeting)
                        .Include(mg => mg.Game)
                        .ToList();
                    Assert.Single(gameInMeetings);
                    Assert.Equal(meeting.MeetingId, gameInMeetings.Single().MeetingId);
                    Assert.Equal(game.GameId, gameInMeetings.Single().GameId);
                }
            }
        }

        [Fact]
        public void Can_Save_Description_For_Meeting()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var description =
                    "some test description some test description some test description some test description some test description some test description " +
                    "some test description some test description some test description some test description some test description some test description " +
                    "some test description some test description some test description some test description some test description some test description ";

                var meeting = new Meeting
                {
                    MeetingId = 1,
                    Title = "TestToAddDescription"
                };

                using (var context = factory.CreateContext())
                {
                    context.Meetings.Add(meeting);
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    meetingRepository.SaveDescriptionForMeeting(description, 1);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var meetings = context.Meetings
                        .ToList();
                    Assert.Single(meetings);
                    Assert.Equal(meeting.MeetingId, meetings.Single().MeetingId);
                    Assert.Equal(description, meetings.Single().Description);
                }
            }
        }

        [Fact]
        public void Can_Remove_Game_From_Meeting()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var game = new Game
                    {GameId = 1, Title = "game1"};

                var meeting = new Meeting
                {
                    MeetingId = 1,
                    Title = "TestToAddGame"
                };

                var gameInMeeting = new MeetingGame
                {
                    Game = game,
                    Meeting = meeting
                };

                using (var context = factory.CreateContext())
                {
                    context.Games.Add(game);
                    context.Meetings.Add(meeting);
                    context.SaveChanges();
                    context.MeetingGame.Add(gameInMeeting);
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    meetingRepository.RemoveGameFromMeeting(game.GameId, meeting.MeetingId);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var gameInMeetings = context.MeetingGame
                        .Include(mg => mg.Meeting)
                        .Include(mg => mg.Game)
                        .ToList();
                    Assert.Empty(gameInMeetings);
                }
            }
        }

        [Fact]
        public void Can_See_Meetings_By_User()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var user1 = new AppUser
                        {Id = "1", UserName = "user1", Email = "user1@example.com"};
                    var user2 = new AppUser
                        {Id = "2", UserName = "user2", Email = "user2@example.com"};
                    var user3 = new AppUser
                        {Id = "3", UserName = "user3", Email = "user3@example.com"};
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.Users.Add(user3);
                    context.SaveChanges();

                    var meeting1 = new Meeting
                        {Title = "Meeting1", Organizer = user1};
                    var meeting2 = new Meeting
                        {Title = "Meeting2", Organizer = user1};
                    var meeting3 = new Meeting
                        {Title = "Meeting3", Organizer = user2};
                    context.Meetings.Add(meeting1);
                    context.Meetings.Add(meeting2);
                    context.Meetings.Add(meeting3);
                    context.SaveChanges();

                    var invitedUsers1 = new MeetingInvitedUser
                    {
                        Meeting = meeting1,
                        AppUser = user3
                    };

                    var invitedUsers2 = new MeetingInvitedUser
                    {
                        Meeting = meeting1,
                        AppUser = user2
                    };

                    var invitedUsers3 = new MeetingInvitedUser
                    {
                        Meeting = meeting2,
                        AppUser = user2
                    };
                    context.MeetingInvitedUser.Add(invitedUsers1);
                    context.MeetingInvitedUser.Add(invitedUsers2);
                    context.MeetingInvitedUser.Add(invitedUsers3);
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    var result1 = meetingRepository.GetMeetingsForUser(user1.Id).ToList();
                    var list1 = new List<Meeting>
                    {
                        meeting1,
                        meeting2
                    };

                    var result2 = meetingRepository.GetMeetingsForUser(user2.Id).ToList();
                    var list2 = new List<Meeting>
                    {
                        meeting1,
                        meeting2,
                        meeting3
                    };

                    var result3 = meetingRepository.GetMeetingsForUser(user3.Id).ToList();
                    var list3 = new List<Meeting>
                    {
                        meeting1
                    };

                    //Assert
                    Assert.Equal(3, context.Meetings.Count());
                    Assert.Equal(3, context.Users.Count());
                    Assert.Equal(3, context.MeetingInvitedUser.Count());

                    Assert.Equal(2, result1.Count);
                    Assert.Equal(result1.OrderBy(m => m.Title), list1.OrderBy(m => m.Title));

                    Assert.Equal(3, result2.Count);
                    Assert.Equal(result2.OrderBy(m => m.Title), list2.OrderBy(m => m.Title));

                    Assert.Single(result3);
                    Assert.Equal(result3, list3);
                }
            }
        }

        [Fact]
        public void Can_Get_Overlapping_Meetings_For_User()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var user1 = new AppUser
                    {Id = "1", UserName = "user1", Email = "user1@example.com"};
                var user2 = new AppUser
                    {Id = "2", UserName = "user2", Email = "user2@example.com"};
                var user3 = new AppUser
                    {Id = "3", UserName = "user3", Email = "user3@example.com"};
                var user4 = new AppUser
                    {Id = "4", UserName = "user4", Email = "user4@example.com"};
                var user5 = new AppUser
                    {Id = "5", UserName = "user5", Email = "user5@example.com"};
                var user6 = new AppUser
                    {Id = "6", UserName = "user6", Email = "user6@example.com"};


                var meeting1 = new Meeting
                {
                    MeetingId = 1, Title = "Meeting1",
                    StartDateTime = DateTime.Now.AddDays(-2).ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddDays(-1).ToUniversalTime(),
                    Organizer = user1
                };
                var meeting2 = new Meeting
                {
                    MeetingId = 2, Title = "Meeting2",
                    StartDateTime = DateTime.Now.AddHours(-6).ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddHours(-1).AddMinutes(1).ToUniversalTime(),
                    Organizer = user2
                };
                var meeting3 = new Meeting
                {
                    MeetingId = 3, Title = "Meeting3",
                    StartDateTime = DateTime.Now.AddHours(-5).ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddMinutes(1).ToUniversalTime(),
                    Organizer = user3
                };
                var meeting4 = new Meeting
                {
                    MeetingId = 4, Title = "Meeting4",
                    StartDateTime = DateTime.Now.AddHours(-1).ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddHours(2).ToUniversalTime(),
                    Organizer = user1
                };
                var meeting5 = new Meeting
                {
                    MeetingId = 5, Title = "Meeting5",
                    StartDateTime = DateTime.Now.ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddHours(2).ToUniversalTime(),
                    Organizer = user2
                };
                var meeting6 = new Meeting
                {
                    MeetingId = 6, Title = "Meeting6",
                    StartDateTime = DateTime.Now.AddHours(1).ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddHours(3).ToUniversalTime(),
                    Organizer = user1
                };
                var meeting7 = new Meeting
                {
                    MeetingId = 7, Title = "Meeting7",
                    StartDateTime = DateTime.Now.AddHours(6).ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddHours(23).ToUniversalTime(),
                    Organizer = user4
                };
                var meeting8 = new Meeting
                {
                    MeetingId = 8, Title = "Meeting8",
                    StartDateTime = DateTime.Now.AddHours(24).ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddHours(30).ToUniversalTime(),
                    Organizer = user4
                };
                var meeting9 = new Meeting
                {
                    MeetingId = 9, Title = "Meeting9",
                    StartDateTime = DateTime.Now.AddHours(-5).ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddMinutes(1).ToUniversalTime(),
                    Organizer = user5
                };
                var meeting10 = new Meeting
                {
                    MeetingId = 10, Title = "Meeting10",
                    StartDateTime = DateTime.Now.AddHours(-1).ToUniversalTime(),
                    EndDateTime = DateTime.Now.AddHours(2).ToUniversalTime(),
                    Organizer = user5
                };

                var invitedUsers1 = new MeetingInvitedUser
                {
                    Meeting = meeting3,
                    AppUser = user1
                };

                var invitedUsers2 = new MeetingInvitedUser
                {
                    Meeting = meeting5,
                    AppUser = user1
                };

                var invitedUsers3 = new MeetingInvitedUser
                {
                    Meeting = meeting3,
                    AppUser = user2
                };

                var invitedUsers4 = new MeetingInvitedUser
                {
                    Meeting = meeting4,
                    AppUser = user2
                };

                var invitedUsers5 = new MeetingInvitedUser
                {
                    Meeting = meeting6,
                    AppUser = user2
                };

                var invitedUsers6 = new MeetingInvitedUser
                {
                    Meeting = meeting2,
                    AppUser = user3
                };

                var invitedUsers7 = new MeetingInvitedUser
                {
                    Meeting = meeting4,
                    AppUser = user3
                };

                var invitedUsers8 = new MeetingInvitedUser
                {
                    Meeting = meeting5,
                    AppUser = user3
                };

                var invitedUsers9 = new MeetingInvitedUser
                {
                    Meeting = meeting6,
                    AppUser = user3
                };

                var invitedUsers10 = new MeetingInvitedUser
                {
                    Meeting = meeting6,
                    AppUser = user4
                };

                var invitedUsers11 = new MeetingInvitedUser
                {
                    Meeting = meeting10,
                    AppUser = user6
                };

                var result1 = new List<Meeting>();
                var resultList1 = new List<Meeting> {meeting3, meeting4, meeting5, meeting6};
                var result2 = new List<Meeting>();
                var resultList2 = new List<Meeting> {meeting2, meeting3, meeting4, meeting5, meeting6};
                var result3 = new List<Meeting>();
                var result4 = new List<Meeting>();
                var resultList4 = new List<Meeting> {meeting6, meeting7, meeting8};
                var result5 = new List<Meeting>();

                //Act
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.Users.Add(user3);
                    context.Users.Add(user4);
                    context.Users.Add(user5);
                    context.Users.Add(user6);
                    context.SaveChanges();
                    context.Meetings.Add(meeting1);
                    context.Meetings.Add(meeting2);
                    context.Meetings.Add(meeting3);
                    context.Meetings.Add(meeting4);
                    context.Meetings.Add(meeting5);
                    context.Meetings.Add(meeting6);
                    context.Meetings.Add(meeting7);
                    context.Meetings.Add(meeting8);
                    context.Meetings.Add(meeting9);
                    context.Meetings.Add(meeting10);
                    context.SaveChanges();
                    context.MeetingInvitedUser.Add(invitedUsers1);
                    context.MeetingInvitedUser.Add(invitedUsers2);
                    context.MeetingInvitedUser.Add(invitedUsers3);
                    context.MeetingInvitedUser.Add(invitedUsers4);
                    context.MeetingInvitedUser.Add(invitedUsers5);
                    context.MeetingInvitedUser.Add(invitedUsers6);
                    context.MeetingInvitedUser.Add(invitedUsers7);
                    context.MeetingInvitedUser.Add(invitedUsers8);
                    context.MeetingInvitedUser.Add(invitedUsers9);
                    context.MeetingInvitedUser.Add(invitedUsers10);
                    context.MeetingInvitedUser.Add(invitedUsers11);
                    context.SaveChanges();

                    var meetingRepository = new EFMeetingRepository(context);
                    result1 = meetingRepository.GetOverlappingMeetingsForUser(DateTime.Now.ToUniversalTime(),
                        DateTime.Now.AddHours(1).ToUniversalTime(), user1.Id).ToList();
                    result2 = meetingRepository.GetOverlappingMeetingsForUser(
                        DateTime.Now.AddHours(-1).ToUniversalTime(),
                        DateTime.Now.AddHours(3).ToUniversalTime(), user2.Id).ToList();
                    result3 = meetingRepository.GetOverlappingMeetingsForUser(
                        DateTime.Now.AddHours(-4).ToUniversalTime(),
                        DateTime.Now.AddHours(4).ToUniversalTime(), user3.Id).ToList();
                    result4 = meetingRepository.GetOverlappingMeetingsForUser(
                        DateTime.Now.AddHours(2).ToUniversalTime(),
                        DateTime.Now.AddDays(1).ToUniversalTime(), user4.Id).ToList();
                    result5 = meetingRepository.GetOverlappingMeetingsForUser(DateTime.Now.AddDays(2).ToUniversalTime(),
                        DateTime.Now.AddDays(3).ToUniversalTime(), user1.Id).ToList();

                    //Assert
                    Assert.Equal(6, context.Users.Count());
                    Assert.Equal(10, context.Meetings.Count());
                    Assert.Equal(11, context.MeetingInvitedUser.Count());

                    Assert.Equal(4, result1.Count);
                    Assert.Equal(resultList1.OrderBy(m => m.MeetingId), result1.OrderBy(m => m.MeetingId));
                    Assert.Equal(5, result2.Count);
                    Assert.Equal(resultList2.OrderBy(m => m.MeetingId), result2.OrderBy(m => m.MeetingId));
                    Assert.Equal(5, result3.Count);
                    Assert.Equal(resultList2.OrderBy(m => m.MeetingId), result3.OrderBy(m => m.MeetingId));
                    Assert.Equal(3, result4.Count);
                    Assert.Equal(resultList4.OrderBy(m => m.MeetingId), result4.OrderBy(m => m.MeetingId));
                    Assert.Empty(result5);
                }
            }
        }

        [Fact]
        public void Can_Get_Overlapping_Meetings_For_Meeting()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                //Arrange
                var user1 = new AppUser
                    {Id = "id1", UserName = "user1", Email = "user1@example.com"};
                var user2 = new AppUser
                    {Id = "id2", UserName = "user2", Email = "user2@example.com"};
                var user3 = new AppUser
                    {Id = "id3", UserName = "user3", Email = "user3@example.com"};
                var user4 = new AppUser
                    {Id = "id4", UserName = "user4", Email = "user4@example.com"};
                var user5 = new AppUser
                    {Id = "id5", UserName = "user5", Email = "user5@example.com"};
                var meeting1 = new Meeting
                {
                    MeetingId = 1,
                    Title = "Meeting1",
                    StartDateTime = DateTime.Today.AddDays(3),
                    EndDateTime = DateTime.Today.AddDays(4),
                    Organizer = user1
                };
                var meeting2 = new Meeting
                {
                    MeetingId = 2,
                    Title = "Meeting2",
                    StartDateTime = DateTime.Today.AddDays(1),
                    EndDateTime = DateTime.Today.AddDays(1).AddHours(3),
                    Organizer = user3
                };
                var meeting3 = new Meeting
                {
                    MeetingId = 3,
                    Title = "Meeting3",
                    StartDateTime = DateTime.Today.AddDays(1),
                    EndDateTime = DateTime.Today.AddDays(1).AddHours(3),
                    Organizer = user3
                };
                var meeting4 = new Meeting
                {
                    MeetingId = 4,
                    Title = "Meeting4",
                    StartDateTime = DateTime.Today.AddDays(1),
                    EndDateTime = DateTime.Today.AddDays(1).AddHours(3),
                    Organizer = user4
                };
                var meeting5 = new Meeting
                {
                    MeetingId = 5,
                    Title = "Meeting5",
                    StartDateTime = DateTime.Today.AddDays(1),
                    EndDateTime = DateTime.Today.AddDays(1).AddHours(3),
                    Organizer = user5
                };
                var meeting6 = new Meeting
                {
                    MeetingId = 6,
                    Title = "Meeting6",
                    StartDateTime = DateTime.Today.AddDays(1),
                    EndDateTime = DateTime.Today.AddDays(1).AddHours(3),
                    Organizer = user2
                };
                var meeting7 = new Meeting
                {
                    MeetingId = 7,
                    Title = "Meeting7",
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(1),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(4),
                    Organizer = user1
                };
                var meeting8 = new Meeting
                {
                    MeetingId = 8,
                    Title = "Meeting8",
                    StartDateTime = DateTime.Today.AddDays(-3),
                    EndDateTime = DateTime.Today.AddDays(-2),
                    Organizer = user1
                };
                var meeting9 = new Meeting
                {
                    MeetingId = 9,
                    Title = "Meeting9",
                    StartDateTime = DateTime.Today.AddDays(1),
                    EndDateTime = DateTime.Today.AddDays(1).AddHours(3),
                    Organizer = user1
                };
                var invitedUser1 = new MeetingInvitedUser
                {
                    Meeting = meeting1,
                    AppUser = user2
                };
                var invitedUser2 = new MeetingInvitedUser
                {
                    Meeting = meeting1,
                    AppUser = user3
                };
                var invitedUser3 = new MeetingInvitedUser
                {
                    Meeting = meeting2,
                    AppUser = user2
                };
                var invitedUser4 = new MeetingInvitedUser
                {
                    Meeting = meeting3,
                    AppUser = user1
                };
                var invitedUser5 = new MeetingInvitedUser
                {
                    Meeting = meeting3,
                    AppUser = user4
                };
                var invitedUser6 = new MeetingInvitedUser
                {
                    Meeting = meeting4,
                    AppUser = user5
                };
                var invitedUser7 = new MeetingInvitedUser
                {
                    Meeting = meeting5,
                    AppUser = user4
                };
                var invitedUser8 = new MeetingInvitedUser
                {
                    Meeting = meeting5,
                    AppUser = user1
                };
                var invitedUser9 = new MeetingInvitedUser
                {
                    Meeting = meeting6,
                    AppUser = user5
                };
                var invitedUser10 = new MeetingInvitedUser
                {
                    Meeting = meeting7,
                    AppUser = user2
                };
                var result = new List<Meeting>();
                var expectedResultList = new List<Meeting>
                {
                    meeting2,
                    meeting3,
                    meeting5,
                    meeting6,
                    meeting9
                };

                //Act
                // Run the test against one instance of the context
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.Users.Add(user3);
                    context.Users.Add(user4);
                    context.Users.Add(user5);
                    context.SaveChanges();
                    context.Meetings.Add(meeting1);
                    context.Meetings.Add(meeting2);
                    context.Meetings.Add(meeting3);
                    context.Meetings.Add(meeting4);
                    context.Meetings.Add(meeting5);
                    context.Meetings.Add(meeting6);
                    context.Meetings.Add(meeting7);
                    context.Meetings.Add(meeting8);
                    context.Meetings.Add(meeting9);
                    context.SaveChanges();
                    context.MeetingInvitedUser.Add(invitedUser1);
                    context.MeetingInvitedUser.Add(invitedUser2);
                    context.MeetingInvitedUser.Add(invitedUser3);
                    context.MeetingInvitedUser.Add(invitedUser4);
                    context.MeetingInvitedUser.Add(invitedUser5);
                    context.MeetingInvitedUser.Add(invitedUser6);
                    context.MeetingInvitedUser.Add(invitedUser7);
                    context.MeetingInvitedUser.Add(invitedUser8);
                    context.MeetingInvitedUser.Add(invitedUser9);
                    context.MeetingInvitedUser.Add(invitedUser10);
                    context.SaveChanges();
                    var meetingRepository = new EFMeetingRepository(context);
                    result = meetingRepository.GetOverlappingMeetingsForMeeting(DateTime.Today.AddDays(1),
                        DateTime.Today.AddDays(2), 1).ToList();
                }

                using (var context = factory.CreateContext())
                {
                    var meetings = context.Meetings
                        .Include(m => m.Organizer)
                        .Include(m => m.MeetingInvitedUser)
                        .ToList();
                    var users = context.Users.ToList();
                    var invitedUsers = context.MeetingInvitedUser.ToList();
                    Assert.Equal(5, users.Count);
                    Assert.Equal(9, meetings.Count);
                    Assert.Equal(10, invitedUsers.Count);
                    Assert.Equal(expectedResultList.Count, result.Count);
                    Assert.Equal(expectedResultList.OrderBy(m => m.MeetingId), result.OrderBy(m => m.MeetingId));
                }
            }
        }

        [Fact]
        public void Can_Get_Description_From_Meeting()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var description =
                    "some test description some test description some test description some test description some test description some test description " +
                    "some test description some test description some test description some test description some test description some test description " +
                    "some test description some test description some test description some test description some test description some test description ";

                var meeting = new Meeting
                {
                    MeetingId = 1,
                    Title = "TestToAddDescription",
                    Description = description
                };

                string result;

                using (var context = factory.CreateContext())
                {
                    context.Meetings.Add(meeting);
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    result = meetingRepository.GetDescriptionFromMeeting(1);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var meetings = context.Meetings
                        .ToList();
                    Assert.Single(meetings);
                    Assert.Equal(meeting.MeetingId, meetings.Single().MeetingId);
                    Assert.Equal(description, result);
                }
            }
        }

        [Fact]
        public void Can_Get_Games_For_Organizer()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var user1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
                var user2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
                var user3 = new AppUser {Id = "id3", UserName = "user3", Email = "user3@example.com"};

                var game1 = new Game {GameId = 1, Title = "game1"};
                var game2 = new Game {GameId = 2, Title = "game2"};
                var game3 = new Game {GameId = 3, Title = "game3"};
                var game4 = new Game {GameId = 4, Title = "game4"};
                var game5 = new Game {GameId = 5, Title = "game5"};
                var game6 = new Game {GameId = 6, Title = "game6"};

                var shelfItem1 = new GameAppUser {UserId = "id1", GameId = 1};
                var shelfItem2 = new GameAppUser {UserId = "id1", GameId = 2};
                var shelfItem3 = new GameAppUser {UserId = "id1", GameId = 3};
                var shelfItem4 = new GameAppUser {UserId = "id1", GameId = 4};
                var shelfItem5 = new GameAppUser {UserId = "id2", GameId = 1};
                var shelfItem6 = new GameAppUser {UserId = "id2", GameId = 4};

                var meeting1 = new Meeting {MeetingId = 1, Title = "Meeting1", Organizer = user1};
                var meeting2 = new Meeting {MeetingId = 2, Title = "Meeting2", Organizer = user1};
                var meeting3 = new Meeting {MeetingId = 3, Title = "Meeting3", Organizer = user1};
                var meeting4 = new Meeting {MeetingId = 4, Title = "Meeting4", Organizer = user1};
                var meeting5 = new Meeting {MeetingId = 5, Title = "Meeting5", Organizer = user1};
                var meeting6 = new Meeting {MeetingId = 6, Title = "Meeting6", Organizer = user2};

                var gameInMeeting1 = new MeetingGame {Game = game1, Meeting = meeting1};
                var gameInMeeting2 = new MeetingGame {Game = game5, Meeting = meeting2};
                var gameInMeeting3 = new MeetingGame {Game = game5, Meeting = meeting3};
                var gameInMeeting4 = new MeetingGame {Game = game3, Meeting = meeting3};
                var gameInMeeting5 = new MeetingGame {Game = game5, Meeting = meeting4};
                var gameInMeeting6 = new MeetingGame {Game = game6, Meeting = meeting4};
                var gameInMeeting7 = new MeetingGame {Game = game3, Meeting = meeting4};
                var gameInMeeting8 = new MeetingGame {Game = game4, Meeting = meeting4};
                var gameInMeeting9 = new MeetingGame {Game = game1, Meeting = meeting6};
                var gameInMeeting10 = new MeetingGame {Game = game6, Meeting = meeting6};

                var result1 = new List<Game>();
                var result2 = new List<Game>();
                var result3 = new List<Game>();
                var result4 = new List<Game>();
                var result5 = new List<Game>();
                var result6 = new List<Game>();
                var result7 = new List<Game>();
                var result8 = new List<Game>();
                var result9 = new List<Game>();

                using (var context = factory.CreateContext())
                {
                    context.Users.AddRange(user1, user2, user3);
                    context.Games.AddRange(game1, game2, game3, game4, game5, game6);
                    context.SaveChanges();
                    context.GameAppUser.AddRange(shelfItem1, shelfItem2, shelfItem3, shelfItem4, shelfItem5,
                        shelfItem6);
                    context.Meetings.AddRange(meeting1, meeting2, meeting3, meeting4, meeting5, meeting6);
                    context.SaveChanges();
                    context.MeetingGame.AddRange(gameInMeeting1, gameInMeeting2, gameInMeeting3, gameInMeeting4,
                        gameInMeeting5, gameInMeeting6, gameInMeeting7, gameInMeeting8, gameInMeeting9,
                        gameInMeeting10);
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    //Add
                    result1 = meetingRepository.GetGamesForOrganizer(0, user1.Id).OrderBy(g => g.Title).ToList();
                    result2 = meetingRepository.GetGamesForOrganizer(0, user2.Id).OrderBy(g => g.Title).ToList();
                    result3 = meetingRepository.GetGamesForOrganizer(0, user3.Id).OrderBy(g => g.Title).ToList();
                    //Edit
                    result4 = meetingRepository.GetGamesForOrganizer(1, user1.Id).OrderBy(g => g.Title).ToList();
                    result5 = meetingRepository.GetGamesForOrganizer(2, user1.Id).OrderBy(g => g.Title).ToList();
                    result6 = meetingRepository.GetGamesForOrganizer(3, user1.Id).OrderBy(g => g.Title).ToList();
                    result7 = meetingRepository.GetGamesForOrganizer(4, user1.Id).OrderBy(g => g.Title).ToList();
                    result8 = meetingRepository.GetGamesForOrganizer(5, user1.Id).OrderBy(g => g.Title).ToList();
                    result9 = meetingRepository.GetGamesForOrganizer(6, user2.Id).OrderBy(g => g.Title).ToList();
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(3, context.Users.Count());
                    Assert.Equal(6, context.Games.Count());
                    Assert.Equal(6, context.GameAppUser.Count());
                    Assert.Equal(6, context.Meetings.Count());
                    Assert.Equal(10, context.MeetingGame.Count());
                    //Add for user1
                    Assert.Equal(4, result1.Count);
                    Assert.Equal(1, result1[0].GameId);
                    Assert.Equal(2, result1[1].GameId);
                    Assert.Equal(3, result1[2].GameId);
                    Assert.Equal(4, result1[3].GameId);
                    //Add for user2
                    Assert.Equal(2, result2.Count);
                    Assert.Equal(1, result2[0].GameId);
                    Assert.Equal(4, result2[1].GameId);
                    //Add for user3
                    Assert.Empty(result3);

                    //Edit
                    Assert.Equal(4, result4.Count);
                    Assert.Equal(1, result4[0].GameId);
                    Assert.Equal(2, result4[1].GameId);
                    Assert.Equal(3, result4[2].GameId);
                    Assert.Equal(4, result4[3].GameId);

                    Assert.Equal(5, result5.Count);
                    Assert.Equal(1, result5[0].GameId);
                    Assert.Equal(2, result5[1].GameId);
                    Assert.Equal(3, result5[2].GameId);
                    Assert.Equal(4, result5[3].GameId);
                    Assert.Equal(5, result5[4].GameId);

                    Assert.Equal(5, result6.Count);
                    Assert.Equal(1, result6[0].GameId);
                    Assert.Equal(2, result6[1].GameId);
                    Assert.Equal(3, result6[2].GameId);
                    Assert.Equal(4, result6[3].GameId);
                    Assert.Equal(5, result6[4].GameId);

                    Assert.Equal(6, result7.Count);
                    Assert.Equal(1, result7[0].GameId);
                    Assert.Equal(2, result7[1].GameId);
                    Assert.Equal(3, result7[2].GameId);
                    Assert.Equal(4, result7[3].GameId);
                    Assert.Equal(5, result7[4].GameId);
                    Assert.Equal(6, result7[5].GameId);

                    Assert.Equal(4, result8.Count);
                    Assert.Equal(1, result8[0].GameId);
                    Assert.Equal(2, result8[1].GameId);
                    Assert.Equal(3, result8[2].GameId);
                    Assert.Equal(4, result8[3].GameId);

                    Assert.Equal(3, result9.Count);
                    Assert.Equal(1, result9[0].GameId);
                    Assert.Equal(4, result9[1].GameId);
                    Assert.Equal(6, result9[2].GameId);
                }
            }
        }

        [Fact]
        public void Can_See_Meetings_By_User_For_N_Days()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var user1 = new AppUser
                        {Id = "1", UserName = "user1", Email = "user1@example.com"};
                    var user2 = new AppUser
                        {Id = "2", UserName = "user2", Email = "user2@example.com"};
                    var user3 = new AppUser
                        {Id = "3", UserName = "user3", Email = "user3@example.com"};
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.Users.Add(user3);
                    context.SaveChanges();

                    var meeting1 = new Meeting
                        {Title = "Meeting1", Organizer = user1, StartDateTime = DateTime.UtcNow.AddDays(-4)};
                    var meeting2 = new Meeting
                        {Title = "Meeting2", Organizer = user1, StartDateTime = DateTime.UtcNow.AddMinutes(-2)};
                    var meeting3 = new Meeting
                        {Title = "Meeting3", Organizer = user2, StartDateTime = DateTime.UtcNow.AddMinutes(1)};
                    var meeting4 = new Meeting
                        {Title = "Meeting4", Organizer = user3, StartDateTime = DateTime.UtcNow.AddDays(4)};
                    var meeting5 = new Meeting
                    {
                        Title = "Meeting5", Organizer = user1,
                        StartDateTime = DateTime.UtcNow.AddDays(6).AddHours(23).AddMinutes(59)
                    };
                    var meeting6 = new Meeting
                        {Title = "Meeting6", Organizer = user1, StartDateTime = DateTime.UtcNow.AddDays(8)};
                    context.Meetings.Add(meeting1);
                    context.Meetings.Add(meeting2);
                    context.Meetings.Add(meeting3);
                    context.Meetings.Add(meeting4);
                    context.Meetings.Add(meeting5);
                    context.Meetings.Add(meeting6);
                    context.SaveChanges();

                    var invitedUsers1 = new MeetingInvitedUser {Meeting = meeting1, AppUser = user3};
                    var invitedUsers2 = new MeetingInvitedUser {Meeting = meeting1, AppUser = user2};
                    var invitedUsers3 = new MeetingInvitedUser {Meeting = meeting2, AppUser = user2};
                    var invitedUsers4 = new MeetingInvitedUser {Meeting = meeting4, AppUser = user2};
                    context.MeetingInvitedUser.Add(invitedUsers1);
                    context.MeetingInvitedUser.Add(invitedUsers2);
                    context.MeetingInvitedUser.Add(invitedUsers3);
                    context.MeetingInvitedUser.Add(invitedUsers4);
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    var result1 = meetingRepository.GetMeetingsForUserForNextDays(user1.Id, 7).ToList();
                    var list1 = new List<Meeting> {meeting5};

                    var result2 = meetingRepository.GetMeetingsForUserForNextDays(user2.Id, 7).ToList();
                    var list2 = new List<Meeting> {meeting3, meeting4};

                    var result3 = meetingRepository.GetMeetingsForUserForNextDays(user3.Id, 7).ToList();
                    var list3 = new List<Meeting> {meeting4};

                    //Assert
                    Assert.Equal(6, context.Meetings.Count());
                    Assert.Equal(3, context.Users.Count());
                    Assert.Equal(4, context.MeetingInvitedUser.Count());

                    Assert.Single(result1);
                    Assert.Equal(result1.OrderBy(m => m.Title), list1.OrderBy(m => m.Title));

                    Assert.Equal(2, result2.Count);
                    Assert.Equal(result2.OrderBy(m => m.Title), list2.OrderBy(m => m.Title));

                    Assert.Single(result3);
                    Assert.Equal(result3, list3);
                }
            }
        }

        [Fact]
        public void Can_Get_Tomorrows_Meetings()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var user1 = new AppUser
                        {Id = "1", UserName = "user1", Email = "user1@example.com"};
                    var user2 = new AppUser
                        {Id = "2", UserName = "user2", Email = "user2@example.com"};
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.SaveChanges();

                    var meeting1 = new Meeting
                        {Title = "Meeting1", Organizer = user1, StartDateTime = DateTime.UtcNow.AddDays(-4)};
                    var meeting2 = new Meeting
                        {Title = "Meeting2", Organizer = user1, StartDateTime = DateTime.UtcNow.AddMinutes(-2)};
                    var meeting3 = new Meeting
                        {Title = "Meeting3", Organizer = user2, StartDateTime = DateTime.UtcNow.AddMinutes(1)};
                    var meeting4 = new Meeting
                    {
                        Title = "Meeting4", Organizer = user1,
                        StartDateTime = DateTime.UtcNow.AddHours(23).AddMinutes(58)
                    };
                    var meeting5 = new Meeting
                    {
                        Title = "Meeting5", Organizer = user2,
                        StartDateTime = DateTime.UtcNow.AddHours(24).AddMinutes(1)
                    };
                    var meeting6 = new Meeting
                        {Title = "Meeting6", Organizer = user1, StartDateTime = DateTime.UtcNow.AddDays(3)};
                    context.Meetings.Add(meeting1);
                    context.Meetings.Add(meeting2);
                    context.Meetings.Add(meeting3);
                    context.Meetings.Add(meeting4);
                    context.Meetings.Add(meeting5);
                    context.Meetings.Add(meeting6);
                    context.SaveChanges();

                    var invitedUsers1 = new MeetingInvitedUser {Meeting = meeting1, AppUser = user2};
                    var invitedUsers2 = new MeetingInvitedUser {Meeting = meeting2, AppUser = user2};
                    context.MeetingInvitedUser.Add(invitedUsers1);
                    context.MeetingInvitedUser.Add(invitedUsers2);
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    var result = meetingRepository.GetTomorrowsMeetings();
                    var list = new List<Meeting> {meeting3, meeting4};


                    //Assert
                    Assert.Equal(6, context.Meetings.Count());
                    Assert.Equal(2, context.Users.Count());
                    Assert.Equal(2, context.MeetingInvitedUser.Count());

                    Assert.Equal(2, result.Count());
                    Assert.Equal(result.OrderBy(m => m.Title), list.OrderBy(m => m.Title));
                }
            }
        }

        [Fact]
        public void Can_Get_Users_To_Send_Tomorrows_Notification()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var user1 = new AppUser
                        {Id = "1", UserName = "user1", Email = "user1@example.com"};
                    var user2 = new AppUser
                        {Id = "2", UserName = "user2", Email = "user2@example.com"};
                    var user3 = new AppUser
                        {Id = "3", UserName = "user3", Email = "user3@example.com"};
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.Users.Add(user3);
                    context.SaveChanges();

                    var meeting1 = new Meeting
                        {Title = "Meeting1", Organizer = user1, StartDateTime = DateTime.UtcNow.AddHours(4)};
                    var meeting2 = new Meeting
                        {Title = "Meeting2", Organizer = user2, StartDateTime = DateTime.UtcNow.AddHours(4)};
                    var meeting3 = new Meeting
                        {Title = "Meeting3", Organizer = user2, StartDateTime = DateTime.UtcNow.AddHours(4)};
                    var meeting4 = new Meeting
                        {Title = "Meeting4", Organizer = user1, StartDateTime = DateTime.UtcNow.AddHours(4)};
                    var meeting5 = new Meeting
                        {Title = "Meeting5", Organizer = user1, StartDateTime = DateTime.UtcNow.AddHours(4)};
                    var meeting6 = new Meeting
                        {Title = "Meeting6", Organizer = user1, StartDateTime = DateTime.UtcNow.AddHours(4)};
                    var meeting7 = new Meeting
                        {Title = "Meeting7", Organizer = user1, StartDateTime = DateTime.UtcNow.AddHours(4)};

                    context.Meetings.Add(meeting1);
                    context.Meetings.Add(meeting2);
                    context.Meetings.Add(meeting3);
                    context.Meetings.Add(meeting4);
                    context.Meetings.Add(meeting5);
                    context.Meetings.Add(meeting6);
                    context.Meetings.Add(meeting7);
                    context.SaveChanges();

                    var invitedUsers1 = new MeetingInvitedUser {Meeting = meeting1, AppUser = user2};
                    var invitedUsers2 = new MeetingInvitedUser {Meeting = meeting1, AppUser = user3};
                    var invitedUsers3 = new MeetingInvitedUser {Meeting = meeting2, AppUser = user1};
                    var invitedUsers4 = new MeetingInvitedUser {Meeting = meeting3, AppUser = user1};
                    context.MeetingInvitedUser.Add(invitedUsers1);
                    context.MeetingInvitedUser.Add(invitedUsers2);
                    context.MeetingInvitedUser.Add(invitedUsers3);
                    context.MeetingInvitedUser.Add(invitedUsers4);
                    context.SaveChanges();

                    var notification1 = new TomorrowsMeetingsNotification()
                    {
                        Meeting = meeting3, Participant = user2, IfSent = true, PostDate = DateTime.UtcNow,
                        MeetingStartDateTime = meeting3.StartDateTime
                    };
                    var notification2 = new TomorrowsMeetingsNotification()
                    {
                        Meeting = meeting3, Participant = user1, IfSent = true, PostDate = DateTime.UtcNow,
                        MeetingStartDateTime = meeting3.StartDateTime
                    };
                    var notification3 = new TomorrowsMeetingsNotification()
                    {
                        Meeting = meeting4, Participant = user1, IfSent = true, PostDate = DateTime.UtcNow,
                        MeetingStartDateTime = DateTime.UtcNow.AddDays(4)
                    };
                    var notification4 = new TomorrowsMeetingsNotification()
                    {
                        Meeting = meeting5, Participant = user1, IfSent = false, PostDate = DateTime.UtcNow,
                        MeetingStartDateTime = meeting5.StartDateTime, NumberOfTries = 1
                    };
                    var notification5 = new TomorrowsMeetingsNotification()
                    {
                        Meeting = meeting6, Participant = user1, IfSent = false, PostDate = DateTime.UtcNow,
                        MeetingStartDateTime = meeting6.StartDateTime, NumberOfTries = 2
                    };
                    var notification6 = new TomorrowsMeetingsNotification()
                    {
                        Meeting = meeting7, Participant = user1, IfSent = false, PostDate = DateTime.UtcNow,
                        MeetingStartDateTime = meeting7.StartDateTime,
                        NumberOfTries = Constants.NumberOfTriesSendNotification
                    };
                    context.TomorrowsMeetingsNotifications.Add(notification1);
                    context.TomorrowsMeetingsNotifications.Add(notification2);
                    context.TomorrowsMeetingsNotifications.Add(notification3);
                    context.TomorrowsMeetingsNotifications.Add(notification4);
                    context.TomorrowsMeetingsNotifications.Add(notification5);
                    context.TomorrowsMeetingsNotifications.Add(notification6);
                    context.SaveChanges();

                    //Act
                    var meetingRepository = new EFMeetingRepository(context);
                    var result1 = meetingRepository.GetUsersToSendTomorrowsNotification()
                        .OrderBy(n => n.Meeting.MeetingId)
                        .OrderBy(n => n.User.Id).ToList();
                    var expectedResult = new List<NotificationList>()
                    {
                        new NotificationList {Meeting = meeting1, User = user1},
                        new NotificationList {Meeting = meeting1, User = user2},
                        new NotificationList {Meeting = meeting1, User = user3},
                        new NotificationList {Meeting = meeting2, User = user1},
                        new NotificationList {Meeting = meeting2, User = user2},
                        new NotificationList {Meeting = meeting4, User = user1},
                        new NotificationList {Meeting = meeting5, User = user1},
                        new NotificationList {Meeting = meeting6, User = user1},

                    }.OrderBy(n => n.Meeting.MeetingId).OrderBy(n => n.User.Id).ToList();

                    //Assert
                    Assert.Equal(7, context.Meetings.Count());
                    Assert.Equal(3, context.Users.Count());
                    Assert.Equal(4, context.MeetingInvitedUser.Count());
                    Assert.Equal(6, context.TomorrowsMeetingsNotifications.Count());

                    Assert.Equal(8, result1.Count());
                    Assert.Equal(expectedResult[0].Meeting.MeetingId, result1[0].Meeting.MeetingId);
                    Assert.Equal(expectedResult[0].User.Id, result1[0].User.Id);
                    Assert.Equal(expectedResult[1].Meeting.MeetingId, result1[1].Meeting.MeetingId);
                    Assert.Equal(expectedResult[1].User.Id, result1[1].User.Id);
                    Assert.Equal(expectedResult[2].Meeting.MeetingId, result1[2].Meeting.MeetingId);
                    Assert.Equal(expectedResult[2].User.Id, result1[2].User.Id);
                    Assert.Equal(expectedResult[3].Meeting.MeetingId, result1[3].Meeting.MeetingId);
                    Assert.Equal(expectedResult[3].User.Id, result1[3].User.Id);
                    Assert.Equal(expectedResult[4].Meeting.MeetingId, result1[4].Meeting.MeetingId);
                    Assert.Equal(expectedResult[4].User.Id, result1[4].User.Id);
                    Assert.Equal(expectedResult[5].Meeting.MeetingId, result1[5].Meeting.MeetingId);
                    Assert.Equal(expectedResult[5].User.Id, result1[5].User.Id);
                    Assert.Equal(expectedResult[6].Meeting.MeetingId, result1[6].Meeting.MeetingId);
                    Assert.Equal(expectedResult[6].User.Id, result1[6].User.Id);
                    Assert.Equal(expectedResult[7].Meeting.MeetingId, result1[7].Meeting.MeetingId);
                    Assert.Equal(expectedResult[7].User.Id, result1[7].User.Id);
                    
                }
            }
        }
    }
}