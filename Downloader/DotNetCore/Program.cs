using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            
            client.BaseAddress = new Uri("https://docs.microsoft.com/en-us/azure/");
            
            var index = client.GetStreamAsync("").Result;
            
            var doc = new HtmlDocument();
            doc.Load(index, Encoding.UTF8);

            var products = doc
                            .DocumentNode
                            .CssSelect("#products ul#all div.group h3")
                            .Select(x => new 
                            { 
                                Group = x.InnerHtml, 
                                Products = x.NextSibling
                                            .NextSibling
                                            .CssSelect("li")
                                            .Select(i => new
                                            {
                                                Group = x.InnerHtml, 
                                                Name = WebUtility.HtmlEncode(i.InnerText.Trim(new[] { ' ', '\n', '\r' }).Replace("  ", " ")), 
                                                Image = i.CssSelect("img").FirstOrDefault().Attributes["src"].Value 
                                            })
                            })
                            .SelectMany(x => x.Products)
                            .OrderBy(x => x.Group);

            var basePath = @"../../Artifacts/";

            foreach (var product in products)
            {
                var groupPath = Path.Combine(basePath, product.Group).Replace(" + ", ", ");
                var svgFile = Path.Combine(groupPath, $"{product.Name}.svg");

                if (!Directory.Exists(groupPath))
                {
                    Directory.CreateDirectory(groupPath);
                }

                using (var svgStream = client.GetStreamAsync(product.Image).Result)
                using (var output = new FileStream(svgFile, FileMode.Create, FileAccess.ReadWrite))
                {
                    svgStream.CopyTo(output);
                }
            }
        }
    }
}
