using Newtonsoft.Json.Linq;
using System;

namespace BeerAppServerSide {
    public class WeatherInformation {

        public WeatherInformation(DateTime WeatherDate, double AverageCelcius) {
            this.WeatherDate = WeatherDate;
            this.AverageCelcius = AverageCelcius;
        }

        public DateTime WeatherDate { get; set; }

        public double AverageCelcius { get; set; }

        public double Longitude { get; set; } = 4.899431;

        public double Latitude { get; set; } = 52.379189;


        //public override string ToString() {
        //    return $"WeatherInformation: (WeatherDate={WeatherDate}; AverageCelcius={AverageCelcius})";
        //}

        public void GetJsonObject(string information) {
            JObject.Parse(information);
        }

    }
}
