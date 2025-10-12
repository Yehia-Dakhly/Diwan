using Diwan.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diwan.DAL.Configurations
{
    internal class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasKey(F => F.Id);
            builder.Property(F => F.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.HasOne(F => F.Requester)
                .WithMany(D => D.SentFriendRequests)
                .HasForeignKey(F => F.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(F => F.Addressee)
                .WithMany(D => D.ReceivedFriendRequests)
                .HasForeignKey(F => F.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(F => new { F.RequesterId, F.AddresseeId }).IsUnique();
        }
    }
}
