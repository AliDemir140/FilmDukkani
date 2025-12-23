using Application.DTOs.UserDTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MVC.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/Auth/login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LoginResponseDTO>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }
}
