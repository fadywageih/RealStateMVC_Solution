using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealState.DAL.Common.Enums;
using RealState.DAL.Models.Users;

namespace RealState.DAL.Presistance.Data.Configurations
{
    public class PropertyConfigurations : IEntityTypeConfiguration<Property>
    {

        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.Property(p=>p.Id).ValueGeneratedOnAdd();    
            builder.HasIndex(p => p.City);
            builder.HasIndex(p => p.PropertyType);
            builder.HasIndex(p => p.PriceType);
            builder.HasOne(p => p.User)
    .WithMany(u => u.Properties)
    .HasForeignKey(p => p.UserId)
    .OnDelete(DeleteBehavior.Cascade);
            #region Enum
            builder.Property(p => p.PropertyType).HasConversion((property) => property.ToString(),
                (propertyType) => (PropertyType)Enum.Parse(typeof(PropertyType), propertyType));
            builder.Property(e => e.PriceType).HasConversion(
                type => type.ToString(),
                value => (PriceType)Enum.Parse(typeof(PriceType), value)
            );
            #endregion
        }
    }
}
