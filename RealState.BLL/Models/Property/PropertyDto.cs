using RealState.DAL.Common.Enums;
using RealState.DAL.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace RealState.BLL.Models.Property
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string PriceType { get; set; } = null!; // نحول الـ enum لـ string
        public string PropertyType { get; set; } = null!;
        public string City { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<string> ImageUrls { get; set; } = new(); // URLs فقط
    }
}
