using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(BeerApplication.Startup))]
namespace BeerApplication {
    class Startup : FunctionsStartup {
        public override void Configure(IFunctionsHostBuilder builder) {
            builder.Services.AddTransient<IWeatherService, WeatherService>();
            builder.Services.AddLogging();
        }
    }
} 
 