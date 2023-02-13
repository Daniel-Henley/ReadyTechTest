using Newtonsoft.Json;

namespace ReadyTechTest.API.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey = "a0de4d9ee194eb716c8e52c5046952af";
        public WeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
        }

        public async Task<float> GetWeatherByLocationAsync(string lat, string lon) 
        {
            var queryString = $"weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";
            var weatherResponse = await _httpClient.GetStringAsync(queryString);

            //for most cases i'd use non anonymous objects but we only need 1 field from the response
            dynamic weather = JsonConvert.DeserializeObject(weatherResponse);
            float currentTemp = weather.main.temp;
            return currentTemp;
        }
    }
}
