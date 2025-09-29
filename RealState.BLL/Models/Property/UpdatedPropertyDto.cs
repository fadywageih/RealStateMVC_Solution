// RealState.BLL.Models.Property/UpdatedPropertyDto.cs

using Microsoft.AspNetCore.Http;
using RealState.DAL.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealState.BLL.Models.Property
{
    public class UpdatedPropertyDto
    {
        public int Id { get; set; }

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

        // الصور الجديدة (اختياري)
        public List<IFormFile>? NewImages { get; set; }

        // IDs للصور القديمة اللي عايز يمسحها (من checkbox)
        public List<int>? ImagesToDelete { get; set; } = new();
    }
}