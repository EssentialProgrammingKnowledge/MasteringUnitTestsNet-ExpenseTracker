using ExpenseTracker.UI.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services
{
    public class CategoryService(HttpClient httpClient) : ICategoryService
    {
        private const string PATH = "/api/categories";

        public async Task Add(CategoryDTO dto)
        {
            var response = await httpClient.PostAsJsonAsync(PATH, dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task Delete(int id)
        {
            var response = await httpClient.DeleteAsync($"{PATH}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<CategoryDTO>> GetAll()
        {
            return await httpClient.GetFromJsonAsync<List<CategoryDTO>>(PATH) ?? [];
        }

        public async Task<CategoryDTO?> GetById(int id)
        {
            var response = await httpClient.GetAsync($"{PATH}/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<CategoryDTO>();
            }

            return null;
        }

        public async Task Update(CategoryDTO dto)
        {
            var response = await httpClient.PutAsJsonAsync($"{PATH}/{dto.Id}", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
