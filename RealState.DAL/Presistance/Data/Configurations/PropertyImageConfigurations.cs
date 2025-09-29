using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealState.DAL.Models.Users;

namespace RealState.DAL.Presistance.Data.Configurations
{
    public class PropertyImageConfigurations : IEntityTypeConfiguration<PropertyImage>
    {
        public void Configure(EntityTypeBuilder<PropertyImage> builder)
        {
            builder.Property(pi => pi.Url)
              .IsRequired()
              .HasColumnType("VARCHAR(500)") // أو NVARCHAR لو ممكن يحتوي روابط بها أحرف غير لاتينية
              .IsUnicode(false); // لأن الروابط عادةً ASCII
            builder.HasOne(pi => pi.Property)
             .WithMany(p => p.Images)
             .HasForeignKey(pi => pi.PropertyId)
             .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(pi => pi.PropertyId);
        }
    }
}
