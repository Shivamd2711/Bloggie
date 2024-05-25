using Azure;
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly ILogger<AdminTagsController> logger;
        private readonly ITagRepository tagRepository;
        public AdminTagsController(ILogger<AdminTagsController> logger, ITagRepository tagRepository)
        {

            this.logger = logger;
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTagRequest model)
        {
            if (model != null)
            {
                var tag = new Tag
                {
                    Name = model.Name,
                    DisplayName = model.DisplayName,
                };
                await tagRepository.AddAsync(tag);
            }
            return RedirectToAction("List"); // explicitly returning add view 
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tags = await tagRepository.GetAllAsync();
            return View(tags);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var tag = await tagRepository.GetAsync(id);
            return View(tag);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest model)
        {
            var tag = new Tag
            {
                Id = model.Id,
                Name = model.Name,
                DisplayName = model.DisplayName,
            };
            var updatedTag = await tagRepository.UpdateAsync(tag);
            if (updatedTag != null)
            {
                return RedirectToAction("Edit", new { id = updatedTag.Id });
            }
            else
            {
                return RedirectToAction("Edit", new { id = model.Id });
            }

        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest model)
        {
            var tag = await tagRepository.DeleteAsync(model.Id);
            if (tag != null)
            {
                return RedirectToAction("List");
            }
            return RedirectToAction("Edit", new { id = model.Id });
        }
    }
}
