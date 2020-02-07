using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PlayBoardGame.Models
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GameAppUser>()
                .HasKey(bc => new {bc.GameId, bc.UserId});
            builder.Entity<GameAppUser>()
                .HasOne(bc => bc.Game)
                .WithMany(b => b.GameAppUser)
                .HasForeignKey(bc => bc.GameId);
            builder.Entity<GameAppUser>()
                .HasOne(bc => bc.AppUser)
                .WithMany(c => c.GameAppUser)
                .HasForeignKey(bc => bc.UserId);

            builder.Entity<Meeting>()
                .HasOne(m => m.Organizer)
                .WithMany(u => u.OrganizedMeetings)
                .HasForeignKey(m => m.OrganizerId);

            builder.Entity<TomorrowsMeetingsNotification>()
                .HasOne(n => n.Participant)
                .WithMany(u => u.TomorrowsMeetingsNotifications)
                .HasForeignKey(n => n.ParticipantId);
            
            builder.Entity<TomorrowsMeetingsNotification>()
                .HasOne(n => n.Meeting)
                .WithMany(m => m.TomorrowsMeetingsNotifications)
                .HasForeignKey(n => n.MeetingId);

            builder.Entity<MeetingInvitedUser>()
                .HasKey(mu => new {mu.MeetingId, mu.UserId});
            builder.Entity<MeetingInvitedUser>()
                .HasOne(mu => mu.Meeting)
                .WithMany(m => m.MeetingInvitedUser)
                .HasForeignKey(mu => mu.MeetingId);
            builder.Entity<MeetingInvitedUser>()
                .HasOne(mu => mu.AppUser)
                .WithMany(u => u.MeetingInvitedUser)
                .HasForeignKey(mu => mu.UserId);

            builder.Entity<MeetingInvitedUser>()
                .Property(e => e.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (InvitationStatus) Enum.Parse(typeof(InvitationStatus), v));

            builder.Entity<MeetingGame>()
                .HasKey(mg => new {mg.GameId, mg.MeetingId});
            builder.Entity<MeetingGame>()
                .HasOne(mg => mg.Game)
                .WithMany(g => g.MeetingGame)
                .HasForeignKey(mg => mg.GameId);
            builder.Entity<MeetingGame>()
                .HasOne(mg => mg.Meeting)
                .WithMany(m => m.MeetingGame)
                .HasForeignKey(mg => mg.MeetingId);

            builder.Entity<Message>()
                .HasOne(m => m.Author)
                .WithMany(u => u.WrittenMessages)
                .HasForeignKey(m => m.AuthorId);

            builder.Entity<Message>()
                .HasOne(m => m.Meeting)
                .WithMany(m => m.Messages)
                .HasForeignKey(m => m.MeetingId);

            builder.Entity<Meeting>()
                .Property(m => m.StartDateTime)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            builder.Entity<Meeting>()
                .Property(m => m.EndDateTime)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            builder.Entity<Message>()
                .Property(m => m.Created)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            
            builder.Entity<FriendInvitation>()
                .HasOne(fi => fi.Sender)
                .WithMany(u => u.SentFriendInvitations)
                .HasForeignKey(fi => fi.SenderId);
            builder.Entity<FriendInvitation>()
                .HasOne(fi => fi.Invited)
                .WithMany(u => u.ReceivedFriendInvitations)
                .HasForeignKey(fi => fi.InvitedId);
            
            builder.Entity<FriendInvitation>()
                .Property(fi => fi.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (FriendInvitationStatus) Enum.Parse(typeof(FriendInvitationStatus), v));
            
            builder.Entity<FriendInvitation>()
                .Property(fi => fi.PostDateTime)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<GameAppUser> GameAppUser { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingInvitedUser> MeetingInvitedUser { get; set; }
        public DbSet<MeetingGame> MeetingGame { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<TomorrowsMeetingsNotification> TomorrowsMeetingsNotifications { get; set; }
        public DbSet<FriendInvitation> FriendInvitations { get; set; }
    }
}