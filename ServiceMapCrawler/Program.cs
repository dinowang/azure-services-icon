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

            var regions = model
                            .Where(x => x.Value.Availability != null)
                            .SelectMany(x => x.Value.Availability?.Select(y => y.Key))
                            .ToHashSet();

            var serviceSheet = Path.Combine(basePath, "service-sheet.html");

            using (var fileStream = new FileStream(serviceSheet, FileMode.Create, FileAccess.ReadWrite))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.WriteLine("<html><head><style>table { border-collapse: collapse; } th, td { border: solid 1px black; padding: 2px 3px; word-break: keep-all; }</style></head><body>");
                writer.WriteLine("<table>");
                writer.WriteLine("<thead>");
                writer.WriteLine("<tr>");
                writer.WriteLine("<th></th>");
                foreach (var regionName in regions)
                {
                    writer.WriteLine($"<th>{regionName}</th>");
                }
                writer.WriteLine("</tr>");
                writer.WriteLine("</thead>");

                writer.WriteLine("<tbody>");
                foreach (var serviceName in model.Keys)
                {
                    writer.WriteLine("<tr>");
                    writer.WriteLine($"<th>{serviceName}</th>");
                    foreach (var regionName in regions)
                    {
                        writer.WriteLine("<td>");
                        var service = model[serviceName];
                        if (service.Availability != null && service.Availability.ContainsKey(regionName))
                        {
                            var availability = service.Availability[regionName];

                            if (!availability.Available)
                                writer.WriteLine("&nbsp;");
                            else if (availability.InGA)
                                writer.WriteLine("✔️");
                            else if (availability.InPreview)
                                writer.WriteLine("🔘");

                            if (!string.IsNullOrEmpty(availability.Expectation))
                                writer.WriteLine("<br>" + availability.Expectation);
                        }
                        writer.WriteLine("</td>");
                    }
                    writer.WriteLine("</tr>");
                }
                writer.WriteLine("</tbody>");
                writer.WriteLine("</table>");
                writer.WriteLine("</body></html>");
            }
        }
    }

}
