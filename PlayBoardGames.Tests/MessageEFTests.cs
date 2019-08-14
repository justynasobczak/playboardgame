using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PlayBoardGame.Models;
using Xunit;

namespace PlayBoardGames.Tests
{
    public class MessageEFTests
    {
        [Fact]
        public void Can_Add_Message()
        {
            //Arrange
            var author = new AppUser
                {Id = "id1", UserName = "author", Email = "author@example.com"};
            var meeting = new Meeting
            {
                MeetingId = 1,
                Title = "Meeting for message"
            };
            var message = new Message
            {
                Text = "This is a text of message which is test message",
                Author = author,
                Created = DateTime.Now,
                Meeting = meeting
            };
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(author);
                    context.Meetings.Add(meeting);
                    context.SaveChanges();
                    //Act
                    var messageRepository = new EFMessageRepository(context);
                    messageRepository.SaveMessage(message);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var result = context.Messages
                        .Include(m => m.Author)
                        .Include(m => m.Meeting)
                        .FirstOrDefault();
                    Assert.Equal(1, context.Users.Count());
                    Assert.Equal(1, context.Meetings.Count());
                    Assert.Equal(1, context.Messages.Count());
                    Assert.Equal(message.Text, result.Text);
                    Assert.Equal(message.AuthorId, result.Author.Id);
                    Assert.Equal(message.Created, result.Created);
                    Assert.Equal(message.MeetingId, result.Meeting.MeetingId);
                }
            }
        }

        [Fact]
        public void Can_Edit_Message()
        {
            //Arrange
            var textMessage = "This is a new text of the message.";
            var author = new AppUser
                {Id = "id1", UserName = "author", Email = "author@example.com"};
            var message = new Message
            {
                Text = "This is a text of the message which is a test message",
                Author = author,
                Created = DateTime.Now.AddDays(1)
            };
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(author);
                    context.Messages.Add(message);
                    context.SaveChanges();
                    //Act
                    var messageRepository = new EFMessageRepository(context);
                    messageRepository.SaveMessage(new Message {Text = textMessage, MessageId = 1});
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    var result = context.Messages
                        .Include(m => m.Author)
                        .FirstOrDefault();
                    Assert.Equal(1, context.Users.Count());
                    Assert.Equal(1, context.Messages.Count());
                    Assert.Equal(textMessage, result.Text);
                    Assert.Equal(message.AuthorId, result.Author.Id);
                    Assert.Equal(message.Created, result.Created);
                }
            }
        }

        [Fact]
        public void Can_Delete_Message()
        {
            //Arrange
            using (var factory = new SQLiteDbContextFactory())
            {
                var author = new AppUser
                    {Id = "id1", UserName = "author", Email = "author@example.com"};
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(author);
                    context.SaveChanges();
                    context.Messages.Add(new Message
                    {
                        MessageId = 1,
                        Text = "This is a text of the message which is a test message",
                        Author = author,
                        Created = DateTime.Now.AddDays(1)
                    });
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var messageRepository = new EFMessageRepository(context);
                    messageRepository.DeleteMessage(1);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(0, context.Messages.Count());
                }
            }
        }

        [Fact]
        public void Can_Get_Message()
        {
            //Arrange
            var result = new Message();
            using (var factory = new SQLiteDbContextFactory())
            {
                var author = new AppUser
                    {Id = "id1", UserName = "author", Email = "author@example.com"};
                var meeting = new Meeting
                {
                    MeetingId = 1,
                    Title = "Meeting for message"
                };
                var message = new Message
                {
                    MessageId = 1,
                    Text = "This is a text of the message which is a test message",
                    Author = author,
                    Meeting = meeting,
                    Created = DateTime.Now.AddDays(1)
                };
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(author);
                    context.Meetings.Add(meeting);
                    context.SaveChanges();
                    context.Messages.Add(message);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var messageRepository = new EFMessageRepository(context);
                    result = messageRepository.GetMessage(1);
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(1, context.Users.Count());
                    Assert.Equal(1, context.Meetings.Count());
                    Assert.Equal(1, context.Messages.Count());
                    Assert.Equal(message.Text, result.Text);
                    Assert.Equal(message.AuthorId, result.AuthorId);
                    Assert.Equal(message.Created, result.Created);
                    Assert.Equal(message.MeetingId, result.MeetingId);
                }
            }
        }

        [Fact]
        public void Can_Get_MessagesForMeeting()
        {
            //Arrange
            var author = new AppUser
                {Id = "id1", UserName = "author", Email = "author@example.com"};
            var meeting1 = new Meeting
            {
                MeetingId = 1,
                Title = "Meeting1 for messages"
            };
            var meeting2 = new Meeting
            {
                MeetingId = 2,
                Title = "Meeting2 for messages"
            };
            var message1 = new Message
            {
                MessageId = 1,
                Text = "Message1",
                Author = author,
                Meeting = meeting1,
                Created = DateTime.Now
            };
            var message2 = new Message
            {
                MessageId = 2,
                Text = "Message2",
                Author = author,
                Meeting = meeting1,
                Created = DateTime.Now
            };
            var message3 = new Message
            {
                MessageId = 3,
                Text = "Message3",
                Author = author,
                Meeting = meeting2,
                Created = DateTime.Now
            };
            var message4 = new Message
            {
                MessageId = 4,
                Text = "Message4",
                Author = author,
                Created = DateTime.Now
            };
            var expectedResult = new List<Message>
            {
                message1,
                message2
            };
            var result = new List<Message>();
            using (var factory = new SQLiteDbContextFactory())
            {
                using (var context = factory.CreateContext())
                {
                    context.Users.Add(author);
                    context.Meetings.Add(meeting1);
                    context.Meetings.Add(meeting2);
                    context.SaveChanges();
                    context.Messages.Add(message1);
                    context.Messages.Add(message2);
                    context.Messages.Add(message3);
                    context.Messages.Add(message4);
                    context.SaveChanges();
                }

                //Act
                using (var context = factory.CreateContext())
                {
                    var messageRepository = new EFMessageRepository(context);
                    result = messageRepository.GetMessagesForMeeting(1).ToList();
                }

                //Assert
                using (var context = factory.CreateContext())
                {
                    Assert.Equal(1, context.Users.Count());
                    Assert.Equal(2, context.Meetings.Count());
                    Assert.Equal(4, context.Messages.Count());
                    Assert.Equal(expectedResult.Count, result.Count);
                    Assert.Equal(expectedResult.OrderBy(m => m.MessageId).Select(m => m.MessageId),
                        result.OrderBy(m => m.MessageId).Select(m => m.MessageId));
                }
            }
        }
    }
}