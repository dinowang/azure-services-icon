using System;
using System.Collections.Generic;
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

            var headers = doc
                            .DocumentNode
                            .CssSelect("#products ul#all div.group h3");

            var groups = headers                            
                            .Select(x => new 
                            { 
                                Group = x.InnerHtml, 
                                Products = x.GetNextSibling("ul")
                                            .CssSelect("li")
                                            .Select(i => new
                                            {
                                                Group = x.InnerHtml, 
                                                Name = WebUtility.HtmlEncode(i.InnerText.Trim(new[] { ' ', '\n', '\r' }).Replace("  ", " ")), 
                                                Image = i.CssSelect("img").FirstOrDefault()?.Attributes["src"]?.Value
                                            })
                                            .Where(i => ! string.IsNullOrEmpty(i.Image))
                                            .ToList()
                            });

            var products = groups
                            .SelectMany(x => x.Products)
                            .OrderBy(x => x.Group);

            var basePath = @"../../Artifacts/";


            var docs = new Dictionary<string, StringBuilder>();

            foreach (var product in products)
            {
                var groupPath = Path.Combine(basePath, product.Group).Replace(" + ", ", ");
                var svgFile = Path.Combine(groupPath, $"{product.Name}.svg");

                if (!Directory.Exists(groupPath))
                {
                    Directory.CreateDirectory(groupPath);
                }
                
                if (!docs.Keys.Contains(product.Group))
                {
                    docs[product.Group] = new StringBuilder();
                }
                
                var url = $"https://dinowang.github.io/azure-services-icon/Artifacts/{WebUtility.UrlEncode(product.Group)}/{WebUtility.UrlEncode(product.Name)}.svg";
                docs[product.Group]
                    .AppendLine($"**{product.Name}**")
                    .AppendLine($"![{product.Name}]({url})")
                    .AppendLine("");

                using (var svgStream = client.GetStreamAsync(product.Image).Result)
                using (var output = new FileStream(svgFile, FileMode.Create, FileAccess.ReadWrite))
                {
                    svgStream.CopyTo(output);
                }
            }

            foreach (var docKey in docs.Keys)
            {
                var groupPath = Path.Combine(basePath, docKey).Replace(" + ", ", ");
                var readmeFile = Path.Combine(groupPath, "README.md");

                using (var stream = new FileStream(readmeFile, FileMode.Create, FileAccess.ReadWrite))
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.WriteLine(docs[docKey].ToString());
                }
            }
        }
    }
}
