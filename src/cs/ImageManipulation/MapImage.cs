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

        public void UploadImageToBlob(Stream imageStream) {
            /* File name of the map */ 
            string path = "AmsterdamMapWeather.png";

            /* Make the image and put text in the image to tell the user if they want to drink beer */
            var memoryStream = MakeImage(imageStream);

            /* Make the azure services needed to upload the image to the blob container */
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(EnvironmentStrings.StorageAccount);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(EnvironmentStrings.BlobStorage);

            /* Make the blob container if it doesnt exist */
            cloudBlobContainer.CreateIfNotExists();
            
            /* Upload the image with the given fileName (path) to the blob as a MemoryStream */
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(path);
            cloudBlockBlob.UploadFromStream(memoryStream);
        }

        public MemoryStream MakeImage(Stream imageStream) {
            /* Set the beer advise string given by some celcius value conditions */  
            string beerAdviceString = GetAdviceToDrinkBeer();

            /* Try to load the Image using the Stream using the ImageSharp library 
                and add the beerAdviseString text at the upperleft corner of the image*/
            try {
                Image image = Image.Load(imageStream);
                return ImageHelper.AddTextToImage(imageStream, (beerAdviceString, (10, 20)));
            }catch(Exception) {
                throw new FileLoadException();
            }
        }

        /* Construct a beerAdvise string given by some totally fictional conditions */
        public string GetAdviceToDrinkBeer() {
            double temp = weatherInformation.AverageCelcius;
            string returnString;

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
