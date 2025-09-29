using Microsoft.AspNetCore.Http;
using RealState.DAL.Common.Enums;
using RealState.DAL.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace RealState.BLL.Models.Property
{
    public class CreatePropertyDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        [Required]
        public PriceType PriceType { get; set; }

        [Required]
        public PropertyType PropertyType { get; set; }

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Phone]
        public string? ContactPhone { get; set; }

        [Required]
        public List<IFormFile> Images { get; set; } = new();
    }
}
