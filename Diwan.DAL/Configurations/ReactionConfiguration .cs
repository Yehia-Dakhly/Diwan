using Diwan.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diwan.DAL.Configurations
{
    internal class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder.HasOne(R => R.Post)
                .WithMany(P => P.Reactions)
                .HasForeignKey(R => R.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(R => R.User)
                .WithMany(U => U.Reactions)
                .HasForeignKey(R => R.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(R => new { R.UserId, R.PostId }).IsUnique();
        }
    }
}