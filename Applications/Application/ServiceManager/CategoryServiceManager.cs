using Application.DTOs.CategoryDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class CategoryServiceManager
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryServiceManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories
                .Select(x => new CategoryDto
                {
                    Id = x.ID,
                    CategoryName = x.CategoryName
                })
                .ToList();
        }

        public async Task AddCategory(CreateCategoryDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName
            };

            await _categoryRepository.AddAsync(category);
        }

        public async Task<UpdateCategoryDto?> GetCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new UpdateCategoryDto
            {
                Id = category.ID,
                CategoryName = category.CategoryName
            };
        }

        public async Task DeleteCategory(UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.Id);
            if (category != null)
            {
                await _categoryRepository.DeleteAsync(category);
            }
        }

        public async Task<bool> UpdateCategory(UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.Id);
            if (category == null)
                return false;

            category.CategoryName = dto.CategoryName;
            await _categoryRepository.UpdateAsync(category);
            return true;
        }

    }
}
