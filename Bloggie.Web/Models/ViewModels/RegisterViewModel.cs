using System.ComponentModel.DataAnnotations;

namespace Bloggie.Web.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        [MinLength(6, ErrorMessage ="Password must be atleast 6 charachters")]
        public string Password { get; set; }

    }
}
