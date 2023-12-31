﻿using JobHubBot.Db.Entities;
using JobHubBot.Interfaces.IDbInterfaces;
using Microsoft.EntityFrameworkCore;
using User = JobHubBot.Db.Entities.User;

namespace JobHubBot.Db.DbContexts
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Skill> Skills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(x => x.Phone).IsUnique();
            modelBuilder.Entity<Skill>().HasIndex(x => x.Name).IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(x => x.Skills)
                .WithMany(y => y.Users);

            modelBuilder.Entity<Skill>()
                .HasMany(x => x.Users)
                .WithMany(y => y.Skills);

            modelBuilder.Entity<Skill>()
                .HasMany(x => x.Jobs)
                .WithMany(y => y.Skills);
        }
    }
}
