using Diwan.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.DAL.Configurations
{
    internal class NotificationConfirurations : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(N => N.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.HasOne(N => N.RecipientUser)
                .WithMany(U => U.ReceivedNotifications)
                .HasForeignKey(N => N.RecipientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasOne(N => N.ActorUser)
                  .WithMany(U => U.CausedNotifications)
                  .HasForeignKey(N => N.ActorUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
