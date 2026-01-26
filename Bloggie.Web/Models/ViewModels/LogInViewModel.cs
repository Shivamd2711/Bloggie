using System.ComponentModel.DataAnnotations;

namespace Bloggie.Web.Models.ViewModels
{
    public class LogInViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password at least to be 6 characters")]
        public string Password { get; set; }
        public string? returnUrl { get; set; }
        public bool RememberMe { get; set; } // Add this line
    }
}
