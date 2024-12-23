using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using samirApp.Data;
using samirApp.Models;
using samirApp.Models.Entities;

namespace samirApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IRoleService _roleService;
        public ProductController(ApplicationDbContext dbContext, IRoleService _roleService)
        {
            this.dbContext = dbContext;
            this._roleService = _roleService;
        }

        [HttpGet]
        public IActionResult GetAllProduct()
        {
            var product = dbContext.Products.ToList();
            return Ok(product);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetCategoryByName(Guid id)
        {
            var product = dbContext.Products.Find(id);

            if (product == null)
            {
                return NotFound("There is no product with this name.");
            }
            return Ok(product);

        }


        [HttpPost]
        public async Task<IActionResult> AddProductAsync(AddProduct addProduct)
        {
            var email = User.Identity?.Name;
            if (email == null || !await _roleService.UserHasRole(email, "Admin"))
            {
                return Unauthorized("You do not have access to add categories.");
            }
            var add = new Product()
            {
                ProductName = addProduct.ProductName,
                ProductDescription = addProduct.ProductDescription,
                ProductType = addProduct.ProductType,
                Price = addProduct.Price,
                
            }; dbContext.Products!.Add(add);
            await dbContext.SaveChangesAsync();
            return Ok(add);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateProductAsync(Guid id, UpdateProduct updateProduct)
        {
            var email = User.Identity?.Name;
            if (email == null || !await _roleService.UserHasRole(email, "Admin"))
            {
                return Unauthorized("You do not have access to add categories.");
            }
            var add = new Product()
            {
                ProductName = updateProduct.ProductName,
                ProductDescription = updateProduct.ProductDescription,
                ProductType = updateProduct.ProductType,
                Price = updateProduct.Price,

            }; dbContext.Products!.Add(add);
            await dbContext.SaveChangesAsync();
            return Ok(add);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteProductAsync(Guid id)
        {
            var email = User.Identity?.Name; if (email == null || !await _roleService.UserHasRole(email, "Admin"))
            {
                return Unauthorized("You do not have access to delete categories.");
            }
            var delete = await dbContext.Products.FindAsync(id); if (delete == null)
            {
                return NotFound("Category not found.");
            }
            dbContext.Products.Remove(delete);
            await dbContext.SaveChangesAsync();
            return Ok(delete);
        }
    }
}
        
    

