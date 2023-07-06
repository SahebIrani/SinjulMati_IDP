using System.Net.Http.Headers;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SinjulMati.Client.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _client;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory factory)
        {
            _client = factory.CreateClient("w");
            _logger = logger;
        }

        public IList<WeatherForecast>? WeatherForecasts { get; set; }

        public async Task OnGet()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            _logger.LogInformation("accessToken: {accessToken}", accessToken);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var resutlString = await _client.GetStringAsync("/WeatherForecast");

            var result = JsonSerializer.Deserialize<List<WeatherForecast>>(resutlString);

            WeatherForecasts = result;
        }

        public IActionResult OnGetLogout()
        {
            return SignOut("Cookies", "oidc");
        }
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}