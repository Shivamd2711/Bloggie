using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostLikeController : ControllerBase
    {
        private readonly IBlogPostLikeRepository _blogPostLikeRepostory;

        public BlogPostLikeController(IBlogPostLikeRepository blogPostLikeRepository)
        {
            this._blogPostLikeRepostory = blogPostLikeRepository;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddLike([FromBody] AddLikeRequestModel request)
        {
            var model = new BlogPostLike()
            {
                BlogPostId = request.BlogPostId,
                UserId = request.UserId,
            };
            await _blogPostLikeRepostory.AddLikeForBlog(model);

            return Ok();

        }

        [HttpGet]
        [Route("{blogPostId:Guid}/totalLikes")]
        public async Task<IActionResult> GetTotalLikeForBlog([FromRoute] Guid blogPostId)
        {
            var totalLikes = await _blogPostLikeRepostory.GetTotalLikes(blogPostId); 
            return Ok(totalLikes);
        }

    }
}
