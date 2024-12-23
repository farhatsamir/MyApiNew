using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using samirApp.Data;
using samirApp.Models;
using samirApp.Models.Entities;

namespace samirApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IRoleService _roleService;
        public CategoryController(ApplicationDbContext dbContext, IRoleService _roleService)
        {
            this.dbContext = dbContext;
            this._roleService = _roleService;
        }

        [HttpGet]
        public IActionResult GetAllCategory()
        {
            var category = dbContext.Categories.ToList();
            return Ok(category);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetCategoryByName(Guid id)
        {
            var category = dbContext.Categories.Find(id);

            if (category == null)
            {
                return NotFound("There is no category with this name.");
            }
            return Ok(category);

        }


        [HttpPost("add-category")] 
        public async Task<IActionResult> AddCategory(AddCategory addCategory)
        {
            var email = User.Identity?.Name;
            if (email == null || !await _roleService.UserHasRole(email, "Admin"))
            { 
                return Unauthorized("You do not have access to add product.");
            }
            var add = new Category()
            {
                CategoryName = addCategory.CategoryName,
                CategoryType = addCategory.CategoryType
            }; 
            dbContext.Categories!.Add(add);
            await dbContext.SaveChangesAsync();
            return Ok(add); 
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategory updateCategory)
        {
            var email = User.Identity?.Name; if (email == null || !await _roleService.UserHasRole(email, "Admin"))
            { 
                return Unauthorized("You do not have access to update product.");
            } 
            var update = await dbContext.Categories.FindAsync(id); 
            if (update == null)
            {
                return NotFound("Category not found."); 
            }
            update.CategoryName = updateCategory.CategoryName;
            update.CategoryType = updateCategory.CategoryType;
            await dbContext.SaveChangesAsync();
            return Ok(update);
        }


        [HttpDelete("{id:guid}")] 
        public async Task<IActionResult> DeleteCategory(Guid id)
        { 
            var email = User.Identity?.Name; 
            if (email == null || !await _roleService.UserHasRole(email, "Admin")) 
            { 
                return Unauthorized("You do not have access to delete product."); 
            } 
            var delete = await dbContext.Categories.FindAsync(id);
            if (delete == null)
            {
                return NotFound("Category not found."); 
            } 
            dbContext.Categories.Remove(delete); 
            await dbContext.SaveChangesAsync();
            return Ok(delete); 
        }









    }
}

