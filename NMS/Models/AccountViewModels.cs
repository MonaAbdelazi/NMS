using System.ComponentModel.DataAnnotations;

namespace NMS.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public string langage { get; set; }
        [Display(Name = "langage")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        //
        [Required]
        [Display(Name = "Branch_ID")]
        public int Branch_ID { get; set; }

        [Required]
        [Display(Name = "Warhouse_ID")]
        public int Warhouse_ID { get; set; }

        [Required]
        [Display(Name = "Group")]
        public string Roles { get; set; }
        // [Required]
        // [Display(Name = "CODE")]
        // public string CODE { get; set; }
        ////
        // [Required]
        // [Display(Name = "PHONE NO")]
        // public string PHONE_NO { get; set; }
        ////
        // [Display(Name = "ENG Name")]
        // public string ENG_Name { get; set; }
    }
    //
    public class ForgotPasswordViewModel
    {
        [Required]

        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        //[Required]
        //[Display(Name = "CODE")]
        //public string CODE { get; set; }
        ////
        //[Required]
        //[Display(Name = "PHONE NO")]
        //public string PHONE_NO { get; set; }
    }
}
