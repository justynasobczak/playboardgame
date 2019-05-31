using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
                    { Id = "id1", UserName = "organizer", Email = "organizer@example.com" };
                var meetingToAdd = new Meeting
                {
                    Title = "TestToAdd",
                    StartDateTime = DateTime.Today.AddDays(1).AddHours(5),
                    EndDateTime = DateTime.Today.AddDays(1).AddHours(8),
                    Organizer = organizer,
                    City = "City",
                    Street = "Street",
                    PostalCode = "PostalCode",
                    Country = Country.Togo,
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
                    Assert.Equal(1, users.Count);
                    Assert.Equal(1, meetings.Count);

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
                    MeetingID = 1,
                    Title = "TestToEdit",
                    StartDateTime = DateTime.Today.AddDays(2).AddHours(2),
                    EndDateTime = DateTime.Today.AddDays(2).AddHours(4),
                    Organizer = organizer2,
                    City = "City2",
                    Street = "Street2",
                    PostalCode = "PostalCode2",
                    Country = Country.None,
                    Notes = "Notes2"
                };

                using (var context = factory.CreateContext())
                {
                    context.Users.Add(organizer1);
                    context.Users.Add(organizer2);
                    context.SaveChanges();

                    context.Meetings.Add(new Meeting
                    {
                        MeetingID = 1,
                        Title = "TestToAdd",
                        StartDateTime = DateTime.Today.AddDays(1).AddHours(5),
                        EndDateTime = DateTime.Today.AddDays(1).AddHours(8),
                        Organizer = organizer1,
                        City = "City",
                        Street = "Street",
                        PostalCode = "PostalCode",
                        Country = Country.Togo,
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
                    Assert.Equal(1, meetings.Count);

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
        public void Can_See_Meetings_By_User()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    var user1 = new AppUser
                        { Id = "1", UserName = "user1", Email = "user1@example.com" };
                    var user2 = new AppUser
                        { Id = "2", UserName = "user2", Email = "user2@example.com" };
                    var user3 = new AppUser
                        { Id = "3", UserName = "user3", Email = "user3@example.com" };
                    context.Users.Add(user1);
                    context.Users.Add(user2);
                    context.Users.Add(user3);
                    context.SaveChanges();

                    var meeting1 = new Meeting
                        { Title = "Meeting1", Organizer = user1 };
                    var meeting2 = new Meeting
                        { Title = "Meeting2", Organizer = user1 };
                    var meeting3 = new Meeting
                        { Title = "Meeting3", Organizer = user2 };
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
    }
}