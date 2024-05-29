using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ILogger<BlogsController> _logger;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IBlogPostLikeRepository _blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBlogPostCommentRepository _blogPostCommentRepository;
        public BlogsController(ILogger<BlogsController> logger, IBlogPostRepository blogPostRepository,
            IBlogPostLikeRepository blogPostLikeRepository, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManger, IBlogPostCommentRepository blogPostCommentRepository)
        {
            _logger = logger;
            _blogPostRepository = blogPostRepository;
            _blogPostLikeRepository = blogPostLikeRepository;
            _signInManager = signInManager;
            _userManager = userManger;
            _blogPostCommentRepository = blogPostCommentRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var liked = false;
            var blogPost = await _blogPostRepository.GetByUrlHandleAsync(urlHandle);
            var blogPostViewModel = new BlogDetailsViewModel();
            if (blogPost != null)
            {
                if (_signInManager.IsSignedIn(User))
                {
                    var likesForBlog = await _blogPostLikeRepository.GetLikesForBlog(blogPost.Id);
                    var userId = _userManager.GetUserId(User);
                    if (userId != null)
                    {
                        var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                        liked = likeFromUser != null;
                    }
                }

                var blogCommentsDomainModel = await _blogPostCommentRepository.GetAllAsync(blogPost.Id);
                var blodCommentViewModel = new List<BlogCommentViewModel>();

                foreach (var blogComment in blogCommentsDomainModel)
                {
                    blodCommentViewModel.Add(
                       new BlogCommentViewModel
                       {
                           Description = blogComment.Description,
                           DateAdded = blogComment.DateAdded,
                           UserName = (await _userManager.FindByIdAsync(blogComment.UserId.ToString())).UserName
                       });
                }

                var totalLikes = await _blogPostLikeRepository.GetTotalLikes(blogPost.Id);
                
                blogPostViewModel.Id = blogPost.Id;
                blogPostViewModel.Heading = blogPost.Heading;
                blogPostViewModel.Content = blogPost.Content;
                blogPostViewModel.ShortDescription = blogPost.ShortDescription;
                blogPostViewModel.UrlHandle = blogPost.UrlHandle;
                blogPostViewModel.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                blogPostViewModel.Visible = blogPost.Visible;
                blogPostViewModel.PublishDate = blogPost.PublishDate;
                blogPostViewModel.Author = blogPost.Author;
                blogPostViewModel.Tags = blogPost.Tags;
                blogPostViewModel.PageTitle = blogPost.PageTitle;
                blogPostViewModel.TotalLikes = totalLikes;
                blogPostViewModel.Liked = liked;
                blogPostViewModel.Comment = blodCommentViewModel;

            }
            return View(blogPostViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BlogDetailsViewModel model)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var domainModel = new BlogPostComment()
                {
                    BlogPostId = model.Id,
                    Description = model.CommentDescription,
                    UserId = Guid.Parse(_userManager.GetUserId(User)),
                    DateAdded = DateTime.Now,

                };
                await _blogPostCommentRepository.AddAsync(domainModel);
                return RedirectToAction("Index", "Blogs", new { urlHandle = model.UrlHandle });
            }
            //return 403  forbidden for this operation
            return View();

        }
    }
}
