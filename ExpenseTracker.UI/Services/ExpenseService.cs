using ExpenseTracker.UI.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services
{
    public class ExpenseService(HttpClient httpClient) : IExpenseService
    {
        private const string PATH = "/api/expenses";

        public async Task<Result> Add(ExpenseDTO dto)
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

        public async Task<Result<List<ExpenseDTO>>> GetAll()
        {
            var response = await httpClient.GetAsync(PATH);
            if (!response.IsSuccessStatusCode)
            {
                return Result<List<ExpenseDTO>>.Failed(await response.ToErrorMessage());
            }
            return Result<List<ExpenseDTO>>.Success(await response.Content.ReadFromJsonAsync<List<ExpenseDTO>>() ?? []);
        }

        public async Task<Result<ExpenseDetailsDTO?>> GetById(int id)
        {
            var response = await httpClient.GetAsync($"{PATH}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return Result<ExpenseDetailsDTO?>.Failed(await response.ToErrorMessage());
            }
            return Result<ExpenseDetailsDTO?>.Success(await response.Content.ReadFromJsonAsync<ExpenseDetailsDTO>());
        }

        public async Task<Result> Update(ExpenseDTO dto)
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
