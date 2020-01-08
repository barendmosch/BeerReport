using System.Threading.Tasks;
using System.Net.Http;

namespace BeerAppServerSide {

     internal class WeatherService : IWeatherService{

        public WeatherService(){}
        public async Task<string> GetWeather(int countryCode){
            var url = $"http://dataservice.accuweather.com/forecasts/v1/daily/1day/{countryCode}?apikey=1yh0Gg86WIPFrrl34VFmJ1hrV9B4yC3B&language=en-us&details=false&metric=false";
            
            HttpClient newClient = new HttpClient();
            HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format(url));

            HttpResponseMessage response = await newClient.SendAsync(newRequest);

            return await response.Content.ReadAsStringAsync();
        }
    }
}