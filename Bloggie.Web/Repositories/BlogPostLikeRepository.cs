
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostLikeRepository : IBlogPostLikeRepository
    {
        private readonly ILogger<BlogPostLikeRepository> _logger;
        private readonly BloggieDbContext db;

        public BlogPostLikeRepository(ILogger<BlogPostLikeRepository> logger, BloggieDbContext dataContext)
        {
            _logger = logger;
            db = dataContext;
        }

        public async Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike)
        {
            await db.BlogPostsLike.AddAsync(blogPostLike);
            await db.SaveChangesAsync();
            return blogPostLike;
        }

        public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPosId)
        {
           return await db.BlogPostsLike.Where(x => x.BlogPostId == blogPosId).ToListAsync();
        }

        public async Task<int> GetTotalLikes(Guid blogPostid)
        {
           return await  db.BlogPostsLike.CountAsync(x => x.BlogPostId == blogPostid);

        }


    }
}
