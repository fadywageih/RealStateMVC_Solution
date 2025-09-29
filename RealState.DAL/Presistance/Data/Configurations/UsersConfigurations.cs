using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealState.DAL.Models.Users;

namespace RealState.DAL.Presistance.Data.Configurations
{
    public class UsersConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(U => U.Name).IsRequired().HasColumnType("VarChar(50)");
            builder.Property(d => d.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.PhoneNumber).HasColumnType("VarChar(20)").IsUnicode(false);
            builder.HasMany(u => u.Properties).WithOne(p => p.User).
                HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
