using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Extensions;
using CdsIconV2Downloader.Models;

namespace CdsIconV2Downloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();

            var result = await client.GetStringAsync("https://iconcloud.design/api/iconlibraryfont/masterfontinfos");
            var iconSets = JsonSerializer.Deserialize<IconSet[]>(result).Where(x => x.name == "Azure Icons");

            var outputBaseDir = Path.Combine("..", "Icons");

            if (Directory.Exists(outputBaseDir))
            {
                Directory.Delete(outputBaseDir, true);
            }
            Directory.CreateDirectory(outputBaseDir);

            foreach (var iconSet in iconSets)
            {
                foreach (var collection in iconSet.collectionMetadata.Where(x => x.iconCount > 0))
                {
                    var qb = new QueryBuilder();
                    qb.Add("masterFontName", iconSet.name);
                    qb.Add("fontName", collection.name);

                    // Console.WriteLine(qb.ToQueryString());

                    result = await client.GetStringAsync($"https://iconcloud.design/api/iconlibraryfont/font" + qb.ToQueryString());

                    var library = JsonSerializer.Deserialize<IconLibary>(result);

                    foreach (var icon in library.icons.Where(x => !string.IsNullOrEmpty(x.svgXml)))
                    {
                        var outputFile = Path.Combine(outputBaseDir, $@"{iconSet.name} - {collection.name} - {icon.friendlyName}.svg");

                        File.WriteAllText(outputFile, icon.svgXml);
                    }
                }
            }

        }
    }
}
