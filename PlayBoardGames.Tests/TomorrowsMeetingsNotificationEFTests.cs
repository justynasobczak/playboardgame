using System;
using System.Linq;
using PlayBoardGame.Models;
using Xunit;

namespace PlayBoardGames.Tests
{
    public class TomorrowsMeetingsNotificationEFTests
    {
        [Fact]
        public void Can_Add_Notification()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var participant = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
                var meeting = new Meeting
                    {MeetingId = 1, Organizer = participant, StartDateTime = DateTime.UtcNow.AddDays(1)};

                //Act
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(participant);
                    context.SaveChanges();
                    context.Meetings.Add(meeting);
                    context.SaveChanges();
                    var notificationRepository = new EFTomorrowsMeetingsNotificationRepository(context);
                    notificationRepository.SaveNotification(new TomorrowsMeetingsNotification
                    {
                        Meeting = meeting,
                        Participant = participant,
                        IfSent = true
                    });
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(1, context.Users.Count());
                    Assert.Equal(1, context.Meetings.Count());
                    Assert.Equal(1, context.TomorrowsMeetingsNotifications.Count());
                    var notification = context.TomorrowsMeetingsNotifications.Single();
                    Assert.Equal(participant.Id, notification.ParticipantId);
                    Assert.Equal(meeting.MeetingId, notification.MeetingId);
                    Assert.True(notification.IfSent);
                    Assert.Equal(meeting.StartDateTime, notification.MeetingStartDateTime);
                    Assert.Equal(1, notification.NumberOfTries);
                }
            }
        }

        [Fact]
        public void Can_Edit_Notification()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var participant = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
                var meeting = new Meeting
                    {MeetingId = 1, Organizer = participant, StartDateTime = DateTime.UtcNow.AddDays(1)};
                var notificationToEdit = new TomorrowsMeetingsNotification
                {
                    Participant = participant,
                    Meeting = meeting,
                    MeetingStartDateTime = meeting.StartDateTime,
                    IfSent = false,
                    NumberOfTries = 1,
                };
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(participant);
                    context.SaveChanges();
                    context.Meetings.Add(meeting);
                    context.SaveChanges();
                    context.TomorrowsMeetingsNotifications.Add(notificationToEdit);
                    context.SaveChanges();
                    var notificationRepository = new EFTomorrowsMeetingsNotificationRepository(context);
                    notificationToEdit.IfSent = false;
                    notificationRepository.SaveNotification(notificationToEdit);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(1, context.Users.Count());
                    Assert.Equal(1, context.Meetings.Count());
                    Assert.Equal(1, context.TomorrowsMeetingsNotifications.Count());
                    var notification = context.TomorrowsMeetingsNotifications.Single();
                    Assert.Equal(participant.Id, notification.ParticipantId);
                    Assert.Equal(meeting.MeetingId, notification.MeetingId);
                    Assert.False(notification.IfSent);
                    Assert.Equal(meeting.StartDateTime, notification.MeetingStartDateTime);
                    Assert.Equal(2, notification.NumberOfTries);
                }
            }
        }

        [Fact]
        public void Can_Get_Notification()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var participant1 = new AppUser {Id = "id1", UserName = "user1", Email = "user1@example.com"};
                var meeting1 = new Meeting
                    {MeetingId = 1, Organizer = participant1, StartDateTime = DateTime.UtcNow.AddDays(1)};
                var participant2 = new AppUser {Id = "id2", UserName = "user2", Email = "user2@example.com"};
                var meeting2 = new Meeting
                    {MeetingId = 2, Organizer = participant2, StartDateTime = DateTime.UtcNow.AddDays(2)};
                var result1 = new TomorrowsMeetingsNotification();
                var result2 = new TomorrowsMeetingsNotification();
                var result3 = new TomorrowsMeetingsNotification();
                var result4 = new TomorrowsMeetingsNotification();
                var notification1 = new TomorrowsMeetingsNotification
                {
                    Meeting = meeting1,
                    Participant = participant1,
                    IfSent = true,
                    MeetingStartDateTime = meeting1.StartDateTime,
                    NumberOfTries = 1
                };

                //Act
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(participant1);
                    context.Users.Add(participant2);
                    context.SaveChanges();
                    context.Meetings.Add(meeting1);
                    context.Meetings.Add(meeting2);
                    context.SaveChanges();
                    context.TomorrowsMeetingsNotifications.Add(notification1);
                    context.SaveChanges();

                    var notificationRepository = new EFTomorrowsMeetingsNotificationRepository(context);
                    result1 = notificationRepository.GetNotification(meeting1.MeetingId, participant1.Id, meeting1.StartDateTime);
                    result2 = notificationRepository.GetNotification(meeting1.MeetingId, participant2.Id, meeting1.StartDateTime);
                    result3 = notificationRepository.GetNotification(meeting2.MeetingId, participant1.Id, meeting1.StartDateTime);
                    result4 = notificationRepository.GetNotification(meeting2.MeetingId, participant1.Id, meeting2.StartDateTime);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(2, context.Users.Count());
                    Assert.Equal(2, context.Meetings.Count());
                    Assert.Equal(1, context.TomorrowsMeetingsNotifications.Count());
                    Assert.Equal(notification1, result1);
                    Assert.Null(result2);
                    Assert.Null(result3);
                    Assert.Null(result4);
                }
            }
        }
    }
}