using Application.DTOs.CategoryDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryServiceManager _categoryServiceManager;

        public CategoryController(CategoryServiceManager categoryServiceManager)
        {
            _categoryServiceManager = categoryServiceManager;
        }

        //GET api/Category

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryServiceManager.GetCategoriesAsync();
            return Ok(categories);
        }

        //POST api/Category

        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDto categoryDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _categoryServiceManager.AddCategory(categoryDto);
            return Ok("Kategori Eklendi.");
        }

        //GET api/Category/{id}

        [HttpGet("get-category")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryServiceManager.GetCategory(id);
            if(category == null)
            {
                return NotFound("Kategori bulunamadı.");
            }
            return Ok(category);
        }

        //DELETE api/Category
        [HttpDelete("delete-category")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deletedCategory = await _categoryServiceManager.GetCategory(id);
            if (deletedCategory == null)
                return NotFound("Kategori bulunamadı.");

            await _categoryServiceManager.DeleteCategory(deletedCategory);
            return Ok("Kategori silindi.");
        }

        // PUT: api/category/update-category
        [HttpPut("update-category")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto updateCategory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryServiceManager.UpdateCategory(updateCategory);
            return Ok("Kategori güncellendi.");
        }
    }
}
