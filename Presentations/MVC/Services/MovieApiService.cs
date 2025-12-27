using Application.DTOs.MovieDTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MVC.Services
{
    public class MovieApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MovieApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private string BaseUrl
        {
            get
            {
                var url = _configuration["ApiSettings:BaseUrl"];
                if (string.IsNullOrWhiteSpace(url))
                    throw new InvalidOperationException("ApiSettings:BaseUrl is missing.");

                return url.TrimEnd('/');
            }
        }

        public async Task<List<MovieDto>> GetEditorsChoiceAsync()
        {
            var url = $"{BaseUrl}/api/movie/showcase/editors-choice";
            var result = await _httpClient.GetFromJsonAsync<List<MovieDto>>(url);
            return result ?? new List<MovieDto>();
        }

        public async Task<List<MovieDto>> GetNewReleasesAsync()
        {
            var url = $"{BaseUrl}/api/movie/showcase/new-releases";
            var result = await _httpClient.GetFromJsonAsync<List<MovieDto>>(url);
            return result ?? new List<MovieDto>();
        }

        public async Task<List<MovieDto>> GetTopRentedAsync(int take = 10)
        {
            if (take <= 0) take = 10;

            var url = $"{BaseUrl}/api/movie/showcase/top-rented?take={take}";
            var result = await _httpClient.GetFromJsonAsync<List<MovieDto>>(url);
            return result ?? new List<MovieDto>();
        }

        public async Task<List<MovieDto>> GetAwardWinnersAsync(int take = 10)
        {
            try
            {
                var url = $"{BaseUrl}/api/Movie/showcase/award-winners?take={take}";
                var data = await _httpClient.GetFromJsonAsync<List<MovieDto>>(url);
                return data ?? new List<MovieDto>();
            }
            catch
            {
                return new List<MovieDto>();
            }
        }

    }
}
