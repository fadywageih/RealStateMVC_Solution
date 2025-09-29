using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RealState.DAL.Models.Users
{
    public class User : ModelBase // Id: int
    {
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        // ✅ رابط مع Identity
        [Required]
        public string IdentityUserId { get; set; } // ← GUID من Identity

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
