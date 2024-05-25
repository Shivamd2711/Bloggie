using Bloggie.Web.Controllers;
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bloggie.Web.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ILogger<AdminTagsController> logger;
        private readonly BloggieDbContext db;
        public TagRepository(ILogger<AdminTagsController> logger, BloggieDbContext bloggieDbContext)
        {
            this.logger = logger;
            this.db = bloggieDbContext;
        }
        public async Task<Tag> AddAsync(Tag tag)
        {
            await db.Tags.AddAsync(tag);
            await db.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var existingTag = await db.Tags.FindAsync(id);
            if (existingTag != null)
            {
                db.Tags.Remove(existingTag);
                await db.SaveChangesAsync();
                return existingTag;
            }
            return null;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            var tagsList = await db.Tags.ToListAsync<Tag>();
            return tagsList;
        }

        public async Task<Tag?> GetAsync(Guid id)
        {
            var tag = await db.Tags.FirstOrDefaultAsync(i => i.Id == id);
            if (tag != null)
            {
                EditTagRequest model = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };
                return tag;
            }
            return null;
        }

        public async Task<Tag?> UpdateAsync(Tag model)
        {
            var tag = await db.Tags.FirstOrDefaultAsync(i => i.Id == model.Id);
            if (tag != null)
            {
                tag.Name = model.Name;
                tag.DisplayName = model.DisplayName;
                await db.SaveChangesAsync();
                return model;
            }
            return null;
        }
    }
}
