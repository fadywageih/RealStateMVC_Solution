// RealState.BLL.Models.Property/PropertyDetailsDto.cs
using RealState.DAL.Common.Enums;

namespace RealState.BLL.Models.Property
{
    public class PropertyDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string PriceType { get; set; } = null!; // string بدل enum للعرض
        public string PropertyType { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? ContactPhone { get; set; }
        public DateTime CreatedOn { get; set; }

        // معلومات المالك (اختياري لكن مفيد)
        public string OwnerName { get; set; } = "N/A";

        // قائمة روابط الصور
        public List<string> ImageUrls { get; set; } = new();
    }
}