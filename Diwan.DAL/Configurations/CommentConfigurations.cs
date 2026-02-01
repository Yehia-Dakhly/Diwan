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
    internal class CommentConfigurations : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Property(C => C.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.HasOne(C => C.User)
               .WithMany(U => U.Comments)
               .HasForeignKey(C => C.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(C => C.Post)
                .WithMany(P => P.Comments)
                .HasForeignKey(C => C.PostId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
