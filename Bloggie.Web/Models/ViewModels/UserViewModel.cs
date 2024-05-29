namespace Bloggie.Web.Models.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
         this.Users = new List<User>(); 
        }
        public List<User> Users;

        public string UserName { get; set; }    
        public string Password { get; set; }
        public string Email { get; set; }
        public bool AdminRoleCheckbox {  get; set; }    
    }
}
