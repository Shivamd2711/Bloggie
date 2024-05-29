using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostCommentRepository : IBlogPostCommentRepository
    {
        private readonly ILogger<BlogPostCommentRepository> _logger;
        private readonly BloggieDbContext db;
        public BlogPostCommentRepository(ILogger<BlogPostCommentRepository> logger, BloggieDbContext dataContext )
        {
            this._logger= logger;
            this.db= dataContext;
        }
        public async Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment)
        {
            await db.BlogPostComment.AddAsync(blogPostComment);
            await db.SaveChangesAsync();
            return blogPostComment;
        }

        public async Task<IEnumerable<BlogPostComment>> GetAllAsync(Guid blogPostId)
        {
           return await db.BlogPostComment.Where(x=> x.BlogPostId == blogPostId).OrderByDescending(x=> x.DateAdded).ToListAsync(); 
        }
    }
}
