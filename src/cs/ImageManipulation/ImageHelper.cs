
using System;
using System.IO;
using SixLabors.Fonts;

// pre-release packages!
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

// ImageHelper class from Triple at: https://github.com/wearetriple/InHolland-CCD/blob/master/DemoQ/ImageHelper.cs
namespace BeerAppServerSide {
    public static class ImageHelper {
        public static MemoryStream AddTextToImage(Stream imageStream, params (string text, (float x, float y) position) [] texts) {
            Console.WriteLine("texts: " + texts);
            imageStream.Position = 0;

            MemoryStream memoryStream = new MemoryStream();

            Image image = Image.Load(imageStream);

            image.Clone(img => {
                    foreach (var (text, (x, y)) in texts) {
                        img.DrawText(text, SystemFonts.CreateFont("Verdana", 24), Rgba32.Black, new PointF(x, y));
                    }
                }).SaveAsPng(memoryStream);

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
