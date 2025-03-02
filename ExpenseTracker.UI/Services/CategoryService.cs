using ExpenseTracker.UI.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services
{
    public class CategoryService
        (
            HttpClient httpClient
        )
        : ICategoryService
    {
        private const string PATH = "/api/categories";

        public async Task<Result> Add(CategoryDTO dto)
        {
            var response = await httpClient.PostAsJsonAsync(PATH, dto);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failed(await response.ToErrorMessage());
            }
            return Result.Success();
        }

        public async Task<Result> Delete(int id)
        {
            var response = await httpClient.DeleteAsync($"{PATH}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failed(await response.ToErrorMessage());
            }
            return Result.Success();
        }

        public async Task<Result<List<CategoryDTO>>> GetAll()
        {
            var response = await httpClient.GetAsync(PATH);
            if (!response.IsSuccessStatusCode)
            {
                return Result<List<CategoryDTO>>.Failed(await response.ToErrorMessage());
            }
            return Result<List<CategoryDTO>>.Success(await response.Content.ReadFromJsonAsync<List<CategoryDTO>>() ?? []);
        }

        public async Task<Result<CategoryDTO?>> GetById(int id)
        {
            var response = await httpClient.GetAsync($"{PATH}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Result<CategoryDTO?>.Failed(await response.ToErrorMessage());
            }
            return Result<CategoryDTO?>.Success(await response.Content.ReadFromJsonAsync<CategoryDTO>());
        }

        public async Task<Result> Update(CategoryDTO dto)
        {
            var response = await httpClient.PutAsJsonAsync($"{PATH}/{dto.Id}", dto);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failed(await response.ToErrorMessage());
            }
            return Result.Success();
        }
    }
}
