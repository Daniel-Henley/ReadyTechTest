namespace ReadyTechTest.API.Services
{
    public interface IWeatherService
    {
        Task<float> GetWeatherByLocationAsync(string lat, string lon);
    }
}
