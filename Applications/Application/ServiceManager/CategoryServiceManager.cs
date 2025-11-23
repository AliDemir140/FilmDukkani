using Application.Repositories;
using Application.DTOs.CategoryDTOs;
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

        public async Task<List<CreateCategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories
                .Select(x => new CreateCategoryDto
                {
                    Name = x.Name
                })
                .ToList();
        }

        public async Task AddCategory(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
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
                Name = category.Name
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

        public async Task UpdateCategory(UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.Id);
            if (category == null) return;

            category.Name = dto.Name;
            await _categoryRepository.UpdateAsync(category);
        }
    }
}
