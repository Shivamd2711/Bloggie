namespace Bloggie.Web.Models.ViewModels
{
    public class AddLikeRequestModel
    {
        public Guid BlogPostId { get; set; }
        public Guid UserId { get; set; }
    }
}
