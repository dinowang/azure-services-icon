using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ServiceMapCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://nnmer.github.io/azure-services-map/dist/");

            var servicesStream = client.GetStreamAsync("js/data/azure-services.json").Result;
            var servicesJson = string.Empty;

            using (var reader = new StreamReader(servicesStream))
            {
                servicesJson = reader.ReadToEnd();

                servicesJson = Regex.Replace(servicesJson, "\"expectation\":false", "\"expectation\":null");
            }

            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var model = JsonSerializer.Deserialize<Models.Services>(servicesJson, options);

            var basePath = Path.Combine("..", "ServiceMapCrawler-Output");

            model.ToList().ForEach(x =>
            {
                var service = x.Value;
                var path = Path.Combine(basePath, service.Id + Path.GetExtension(service.Icon));

                using (var inuptStream = client.GetStreamAsync(service.Icon).Result)
                using (var outputStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    inuptStream.CopyTo(outputStream);
                }
            });
        }
    }

}
