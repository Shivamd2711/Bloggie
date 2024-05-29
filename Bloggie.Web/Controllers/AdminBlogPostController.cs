using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
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
                    ShortDescription = model.ShortDescription,
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
            var tags = await _tagRepository.GetAllAsync();
            if (blogPost != null)
            {
                EditBlogPostRequest model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    ShortDescription = blogPost.ShortDescription,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    PublishDate = blogPost.PublishDate,
                    UrlHandle = blogPost.UrlHandle,
                    Author = blogPost.Author,
                    Visible = blogPost.Visible,
                    Tags = tags.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };
                return View(model);
            }
            return View();

        }

 
        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest model)
        {
            var blogPost = new BlogPost
            {
                Id = model.Id,
                Heading = model.Heading,
                PageTitle = model.PageTitle,
                Content = model.Content,
                ShortDescription = model.ShortDescription,
                FeaturedImageUrl = model.FeaturedImageUrl,
                PublishDate = model.PublishDate,
                UrlHandle = model.UrlHandle,
                Author = model.Author,
                Visible = model.Visible,

            };
            var tagList = new List<Tag>();
            foreach (var item in model.SelectedTags)
            {
                if(Guid.TryParse(item, out var tag))
                {
                    var foundtag = await _tagRepository.GetAsync(tag);
                    if(foundtag != null)
                    {
                        tagList.Add(foundtag);
                    }
                }
              
            }
            blogPost.Tags = tagList;
            var updatedBlogPost = await _blogPostRepository.UpdateAsync(blogPost);
            if (updatedBlogPost != null)
            {
                return RedirectToAction("Edit", new {Id = updatedBlogPost.Id});
            }
            return RedirectToAction("List"); 

        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedBlogPost = await _blogPostRepository.DeleteAsync(id);
            if(deletedBlogPost != null)
            {
                return RedirectToAction("List");
            }
            return RedirectToAction("Edit", new { id = id });
        }
    }
}
