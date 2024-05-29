using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ILogger<BlogPostRepository> _logger;
        private readonly BloggieDbContext db;
        public BlogPostRepository(ILogger<BlogPostRepository> logger, BloggieDbContext dataContext)
        {
            _logger = logger;
            db = dataContext;
        }


        public async Task<BlogPost> AddAsync(BlogPost model)
        {
            await db.AddAsync(model);
            await db.SaveChangesAsync();
            return model;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlogPost = await db.BlogPosts.FindAsync(id);
            if (existingBlogPost != null)
            {
                db.BlogPosts.Remove(existingBlogPost);
                await db.SaveChangesAsync();
                return existingBlogPost;
            }
            return null;

        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            //Inlcude is used to inclue the Tags Property,for you have to mention the Tags Property in entity class
            //so that EF should know this during migration and while contacting db
            return await db.BlogPosts.Include(x => x.Tags).ToListAsync();
        }

        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await db.BlogPosts.Include(i => i.Tags).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost model)
        {
            var existingBlogpost = await db.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == model.Id);
            if (existingBlogpost != null)
            {
                existingBlogpost.Id = model.Id;
                existingBlogpost.Tags = model.Tags;
                existingBlogpost.Heading = model.Heading;
                existingBlogpost.Author = model.Author;
                existingBlogpost.Content = model.Content;
                existingBlogpost.ShortDescription = model.ShortDescription;
                existingBlogpost.Visible = model.Visible;
                existingBlogpost.FeaturedImageUrl = model.FeaturedImageUrl;
                existingBlogpost.UrlHandle = model.UrlHandle;
                existingBlogpost.PublishDate = model.PublishDate;
                existingBlogpost.PageTitle = model.PageTitle;

                await db.SaveChangesAsync();
                return existingBlogpost;
            }
            return null;
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await db.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }
    }
}
