using System;
using System.IO;
using SkiaSharp;

namespace SvgToPng
{
    class Program
    {
        static void Main(string[] args)
        {
            var srcPath = "../Icons";
            var dstPath = "../Icons.Png";

            if (!Directory.Exists(dstPath))
                Directory.CreateDirectory(dstPath);

            foreach (var inputPath in Directory.GetFiles(srcPath, "*.svg"))
            {
                Console.WriteLine(inputPath);
                using var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);

                var outputPath = Path.Combine(dstPath, Path.GetFileNameWithoutExtension(inputPath) + ".png");
                var quality = 100;

                var svg = new SkiaSharp.Extended.Svg.SKSvg();
                try
                {
                    var pict = svg.Load(inputStream);
                    var dimen = new SkiaSharp.SKSizeI(
                        (int)Math.Ceiling(pict.CullRect.Width) * 5,
                        (int)Math.Ceiling(pict.CullRect.Height) * 5
                    );
                    var matrix = SKMatrix.MakeScale(5, 5);
                    var img = SKImage.FromPicture(pict, dimen, matrix);

                    // convert to PNG
                    var skdata = img.Encode(SkiaSharp.SKEncodedImageFormat.Png, quality);
                    using (var stream = File.OpenWrite(outputPath))
                    {
                        skdata.SaveTo(stream);
                    }
                }
                catch
                {

                }

            }
        }
    }
}
