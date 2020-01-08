using System.Threading.Tasks;

namespace BeerApplication{
    public interface IWeatherService {
        Task<string> GetWeather(int countryCode);
    }
}