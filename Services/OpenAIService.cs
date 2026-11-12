using System.Text;
using System.Text.Json;
using HealthOps_Project.Models;

namespace HealthOps_Project.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["OpenAI:ApiKey"];

            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("OpenAI API key is not configured");
            }

            // Set base address for OpenAI API
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<ChatResponse> GetHealthOPSResponseAsync(string userMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey))
                {
                    return GetFallbackResponse("Service is temporarily unavailable. Please contact support.");
                }

                var healthOPSContext = @"
You are HealthOps Assistant, an AI helper for a healthcare management system in South Africa.

IMPORTANT: You are assisting with a healthcare system. Always maintain professionalism and medical accuracy.

Key functionalities you can help with:
1. APPOINTMENTS: Book, reschedule, cancel appointments. Online booking available 24/7
2. BOOKING HOURS: Monday-Friday 8AM-6PM, Saturday 9AM-1PM (phone), Online: 24/7
3. PRESCRIPTIONS: Refill requests, medication questions
4. MEDICAL RECORDS: Access test results, medical history
5. BILLING: Medical aid claims, payment questions, billing inquiries
6. LOCATION: Based in Port Elizabeth, South Africa

EMERGENCY PROTOCOL: For emergencies, always advise to call 112 or visit nearest emergency room.

Always be helpful, professional, and healthcare-focused. If you don't know something, direct them to call the main office at +27 123 456 789 or email info@healthops.com.

Keep responses concise but helpful. Maximum 2-3 sentences for simple questions.
";

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = healthOPSContext },
                        new { role = "user", content = userMessage }
                    },
                    max_tokens = 300,
                    temperature = 0.7
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending request to OpenAI API");
                var response = await _httpClient.PostAsync("chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("OpenAI API error: {StatusCode} - {Content}", response.StatusCode, errorContent);

                    return GetFallbackResponse("I'm experiencing technical difficulties. Please try again in a moment.");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseContent);

                var reply = document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                _logger.LogInformation("Successfully received response from OpenAI");
                return new ChatResponse
                {
                    Reply = reply?.Trim() ?? "I apologize, but I couldn't process your request.",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OpenAIService.GetHealthOPSResponseAsync");
                return GetFallbackResponse("I'm sorry, I'm having trouble connecting right now. Please try again later or contact our support team.");
            }
        }

        private ChatResponse GetFallbackResponse(string message)
        {
            return new ChatResponse
            {
                Reply = message,
                Success = false,
                Error = "Service unavailable"
            };
        }
    }
}