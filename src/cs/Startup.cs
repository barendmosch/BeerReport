using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(BeerAppServerSide.Startup))]
namespace BeerAppServerSide {
    class Startup : FunctionsStartup {
        public override void Configure(IFunctionsHostBuilder builder) {
            builder.Services.AddTransient<IWeatherService, WeatherService>();
            builder.Services.AddTransient<IExceptionHandler, ExceptionHandler>();

            builder.Services.AddLogging();
        }
    }
} 
 