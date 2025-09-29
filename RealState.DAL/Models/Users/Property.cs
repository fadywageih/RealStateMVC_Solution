using RealState.DAL.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealState.DAL.Models.Users
{
    public class Property:ModelBase
    {
        [Required]
        [MaxLength(200)]
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
        #region Attact
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        #endregion
        // Foreign Key
        [Required]
        public int UserId { get; set; } // ← صحيح: int لأن User.Id = int
        // Navigation Property
        public User User { get; set; } = null!;
    }
}
