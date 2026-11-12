using HealthOps_Project.Models;

namespace HealthOps_Project.Services
{
    public interface IOpenAIService
    {
        Task<ChatResponse> GetHealthOPSResponseAsync(string userMessage);
    }
}