using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BeerAppServerSide {
    public class MapImage {

        public MapImage() { }

        public void DrawImage(Stream imageStream) {
            Image image = Image.FromStream(imageStream);
            Bitmap bitmap = (Bitmap)image;
            Console.WriteLine(bitmap);
        }
    }
}
