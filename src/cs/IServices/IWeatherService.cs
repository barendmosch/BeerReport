using System.Threading.Tasks;

namespace BeerAppServerSide {
    public interface IWeatherService {
        Task<string> GetWeather(int countryCode);
    }
}