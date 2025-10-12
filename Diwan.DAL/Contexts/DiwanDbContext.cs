using Diwan.DAL.Configurations;
using Diwan.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.DAL.Contexts
{
    public class DiwanDbContext : IdentityDbContext<DiwanUser>
    {
        public DiwanDbContext(DbContextOptions<DiwanDbContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new DiwanUserConfigurations());
            builder.ApplyConfiguration(new FriendshipConfiguration());
            builder.ApplyConfiguration(new NotificationConfirurations());
            builder.ApplyConfiguration(new PostConfigurations());
            builder.ApplyConfiguration(new CommentConfigurations());
            builder.ApplyConfiguration(new ReactionConfiguration());

        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
