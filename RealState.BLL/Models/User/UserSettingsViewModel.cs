using System.ComponentModel.DataAnnotations;

namespace RealState.BLL.Models.User
{
    // ViewModels/UserSettingsViewModel.cs
    public class UserSettingsViewModel
    {
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        [Required, Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        // الحقول دي للباسوورد (اختياري)
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password), Compare("NewPassword")]
        public string? ConfirmNewPassword { get; set; }
    }
}
