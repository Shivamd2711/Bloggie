using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
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

        public Task<BlogPost?> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            //Inlcude is used to inclue the Tags Property,for you have to mention the Tags Property in entity class
            //so that EF should know this during migration and while contacting db
            return await db.BlogPosts.Include(x=> x.Tags).ToListAsync();    
        }

        public Task<BlogPost?> GetAsync(Guid id)
        {
            var blogPost = db.BlogPosts.Include(i=>i.Tags).FirstOrDefaultAsync(i=>i.Id == id);
            if(blogPost == null)
            {

            }
        }

        public Task<BlogPost?> UpdateAsync(BlogPost model)
        {
            throw new NotImplementedException();
        }
    }
}
