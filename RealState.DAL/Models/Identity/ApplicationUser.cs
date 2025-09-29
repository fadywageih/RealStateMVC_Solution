using Microsoft.AspNetCore.Identity;

namespace RealState.DAL.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public bool ISAgress { get; set; }

    }
}
