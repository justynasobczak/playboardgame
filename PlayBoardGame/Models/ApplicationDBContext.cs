﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            
            builder.Entity<AppUser>()
                .HasMany(u => u.OrganizedMeetings)
                .WithOne(m => m.Organizer);
        }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameAppUser> GameAppUser { get; set; }
        
        public  DbSet<Meeting> Meetings { get; set; }
    }
}
