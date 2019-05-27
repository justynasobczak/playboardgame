using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PlayBoardGame.Models
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<GameAppUser>()
                .HasKey(bc => new { bc.GameID, bc.UserId });
            builder.Entity<GameAppUser>()
                .HasOne(bc => bc.Game)
                .WithMany(b => b.GameAppUser)
                .HasForeignKey(bc => bc.GameID);
            builder.Entity<GameAppUser>()
                .HasOne(bc => bc.AppUser)
                .WithMany(c => c.GameAppUser)
                .HasForeignKey(bc => bc.UserId);

            builder.Entity<Meeting>()
                .HasOne(m => m.Organizer)
                .WithMany(u => u.OrganizedMeetings)
                .HasForeignKey(m => m.OrganizerId);
            
            builder.Entity<MeetingInvitedUser>()
                .HasKey(mu => new { mu.MeetingID, mu.UserId });
            builder.Entity<MeetingInvitedUser>()
                .HasOne(mu => mu.Meeting)
                .WithMany(m => m.MeetingInvitedUser)
                .HasForeignKey(mu => mu.MeetingID);
            builder.Entity<MeetingInvitedUser>()
                .HasOne(mu => mu.AppUser)
                .WithMany(u => u.MeetingInvitedUser)
                .HasForeignKey(mu => mu.UserId);
            
            builder.Entity<MeetingInvitedUser>()
                .Property(e => e.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (InvitationStatus)Enum.Parse(typeof(InvitationStatus), v));
            
            builder.Entity<AppUser>()
                .Property(e => e.Country)
                .HasConversion(
                    v => v.ToString(),
                    v => (Country)Enum.Parse(typeof(Country), v));
            
            builder.Entity<Meeting>()
                .Property(e => e.Country)
                .HasConversion(
                    v => v.ToString(),
                    v => (Country)Enum.Parse(typeof(Country), v));
        }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameAppUser> GameAppUser { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingInvitedUser> MeetingInvitedUser { get; set; }
    }
}
