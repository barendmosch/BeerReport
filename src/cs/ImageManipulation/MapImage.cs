using System;
using SixLabors.ImageSharp;
using System.IO;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace BeerAppServerSide {
    public class MapImage {

        public readonly WeatherInformation weatherInformation;

        public MapImage(WeatherInformation weatherInformation) {
            this.weatherInformation = weatherInformation;
        }

        public MemoryStream MakeImage(Stream imageStream) {
            string beerAdviceString = GetAdviceToDrinkBeer();
            try {
                Image image = Image.Load(imageStream);
                return ImageHelper.AddTextToImage(imageStream, (beerAdviceString, (10, 20)));
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
                throw new FileLoadException();
            }
        }

        public void UploadImageToBlob(Stream imageStream) {
            string path = "AmsterdamMapWeather.png";
            var memoryStream = MakeImage(imageStream);

            /* Use a try catch with await async */
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(EnvironmentStrings.StorageAccount);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(EnvironmentStrings.BlobStorage);

            cloudBlobContainer.CreateIfNotExists();
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(path);
            cloudBlockBlob.UploadFromStream(memoryStream);
        }

        public string GetAdviceToDrinkBeer() {
            double temp = weatherInformation.AverageCelcius;
            string returnString = "";
            if (temp >= 15 && temp <= 50) {
                returnString = "Lovely weather, go ahead, drink beer";
            }else if(temp < 15 && temp >= -10) {
                returnString = "I shouldn't get a beer if I were you";
            } else if(temp < -10) {
                returnString = "Get a cup of tea for god sake";
            } else {
                returnString = "it's damn warm, get loads of beer, now!";
            }

            returnString += " Temperature: " + weatherInformation.AverageCelcius + " Celcius";
            return returnString;
        }
    }
}
