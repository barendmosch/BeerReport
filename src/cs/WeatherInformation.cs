using System;
using System.Collections.Generic;
using System.Text;

namespace BeerAppServerSide {
    public class WeatherInformation {

        public WeatherInformation(DateTime WeatherDate, double AverageCelcius) {
            this.WeatherDate = WeatherDate;
            this.AverageCelcius = AverageCelcius;
        }

        public DateTime WeatherDate { get; set; }

        public double AverageCelcius { get; set; }

        public override string ToString() {
            return $"WeatherInformation: (WeatherDate={WeatherDate}; AverageCelcius={AverageCelcius})";
        }

    }
}
