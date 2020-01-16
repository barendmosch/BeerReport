using System.IO;
using System.Threading.Tasks;

namespace BeerAppServerSide {
    public interface IWeatherService {
        Task<MemoryStream> GetBeerReport();

        Task CreateBeerReport();
    }
}