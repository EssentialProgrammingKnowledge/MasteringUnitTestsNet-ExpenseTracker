using ExpenseTracker.UI.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services
{
    public static class ResponseExtensions
    {
        public static async Task<ErrorMessage?> ToErrorMessage(this HttpResponseMessage response)
        {
            if (response == null)
            {
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return new ErrorMessage("GENERAL_ERROR", "Something Bad Happen");
            }

            return await response.Content.ReadFromJsonAsync<ErrorMessage>();
        }
    }
}
