using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    public class AdminBlogPostController : Controller
    {
        private readonly ILogger<AdminBlogPostController> _logger;
        private readonly ITagRepository _tagRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        public AdminBlogPostController(ILogger<AdminBlogPostController> logger, ITagRepository tagRepo, IBlogPostRepository blogPostRepository)
        {
            _logger = logger;
            _tagRepository = tagRepo;
            _blogPostRepository = blogPostRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var tags = await _tagRepository.GetAllAsync();
            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest model)
        {
            if (model != null)
            {
                var blogPost = new BlogPost
                {
                    Heading = model.Heading,
                    PageTitle = model.PageTitle,
                    Content = model.Content,
                    ShortDesciption = model.ShortDesciption,
                    UrlHandle = model.UrlHandle,
                    FeaturedImageUrl = model.FeaturedImageUrl,
                    Visible = model.Visible,
                    PublishDate = model.PublishDate,
                    Author = model.Author,
                };
                //for mapping tags from selected tags
                var tagList = new List<Tag>();
                foreach (var item in model.SelectedTags)
                {
                    var itemAsGuid = Guid.Parse(item);
                    var tag = await _tagRepository.GetAsync(itemAsGuid);
                    if (tag != null)
                    {
                        tagList.Add(tag);
                    }
                }
                blogPost.Tags = tagList;
                await _blogPostRepository.AddAsync(blogPost);

                return RedirectToAction("Add");
            }
            return RedirectToAction("Add");
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var blogPosts = await _blogPostRepository.GetAllAsync();
            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var blogPost = await _blogPostRepository.GetAsync(id);
            if (blogPost != null)
            {
                EditBlogPostTagRequestcs model = new EditBlogPostTagRequestcs()
                {
                    Id = blogPost.Id,
                    Heading= blogPost.Heading,
                    Content = blogPost.Content,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,

                    Tags = blogPost.Tags
                };
            }
            return View(blogPost);

        }
    }
}
