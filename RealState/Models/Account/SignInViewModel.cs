using System.ComponentModel.DataAnnotations;

namespace RealState.Models.Account
{
    public class SignInViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
