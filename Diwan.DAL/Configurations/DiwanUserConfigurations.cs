using Diwan.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diwan.DAL.Configurations
{
    internal class DiwanUserConfigurations : IEntityTypeConfiguration<DiwanUser>
    {
        public void Configure(EntityTypeBuilder<DiwanUser> builder)
        {
            builder.Property(U => U.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
