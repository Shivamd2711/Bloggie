using System.ComponentModel.DataAnnotations;

namespace Bloggie.Web.Models.ViewModels
{
    public class LogInViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(6, ErrorMessage ="Password atlest to be 6 charachters")]
        public string Password { get; set; }

        public string? returnUrl {  get; set; } 
    }
}
