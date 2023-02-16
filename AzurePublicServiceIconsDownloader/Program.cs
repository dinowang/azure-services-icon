using System.IO.Compression;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

var outputBaseDir = Path.Combine("..", "AzurePublicServiceIcons");
var outputZipFile = Path.Combine(".", "AzurePublicServiceIcons.zip");

if (Directory.Exists(outputBaseDir))
{
    Directory.Delete(outputBaseDir, true);
}
Directory.CreateDirectory(outputBaseDir);
if (!File.Exists(outputZipFile))
{
    var client = new HttpClient();

    var html = await client.GetStringAsync("https://learn.microsoft.com/zh-tw/azure/architecture/icons/");

    var doc = new HtmlDocument();
    doc.LoadHtml(html);
    var link = doc.QuerySelector("a[data-linktype='external']");
    var url = link.GetAttributeValue("href", string.Empty);

    // https://arch-center.azureedge.net/icons/Azure_Public_Service_Icons_V10.zip
    using var result = await client.GetStreamAsync(url);

    using var outputStream = new FileStream(outputZipFile, FileMode.Create, FileAccess.Write);
    await result.CopyToAsync(outputStream);
}

using var zipStream = new FileStream(outputZipFile, FileMode.Open, FileAccess.Read);
var zipFile = new ZipArchive(zipStream);

var allSvg = Directory
                .GetFiles(outputBaseDir, "*.svg")
                .Select(x => new
                {
                    FileName = Path.GetFileName(x),
                    Status = -1
                })
                .ToDictionary(x => x.FileName, x => x.Status);


foreach (var entry in zipFile.Entries)
{
    var (outputName, status) = HandleEntry(allSvg, outputBaseDir, entry);
    allSvg[outputName] = status;
}

File.Delete(outputZipFile);

foreach (var entry in allSvg)
{
    var filePath = Path.Combine(outputBaseDir, entry.Key);
    var status = entry.Value;

    if (status == -1)
    {
        File.Delete(filePath);
    }
}

(string, int) HandleEntry(Dictionary<string, int> allSvg, string basePath, ZipArchiveEntry entry)
{
    if (entry.FullName.EndsWith("/"))
        return (string.Empty, 0);

    var entryExtName = Path.GetExtension(entry.FullName);
    if (!entryExtName.Equals(".svg", StringComparison.OrdinalIgnoreCase))
        return (string.Empty, 0);

    var entryFileName = Path.GetFileName(entry.FullName);
    entryFileName = Regex.Replace(entryFileName, @"^\d+\-icon-service-", string.Empty);
    entryFileName = Regex.Replace(entryFileName, "-", " ");
    var entryPath = Path.GetFileName(Path.GetDirectoryName(entry.FullName));
    var outputName = Path.Combine(basePath, $"{entryPath} - {entryFileName}");

    var fileInfo = new FileInfo(outputName);
    var status = 1;
    if (fileInfo.Exists && fileInfo.CreationTimeUtc == entry.LastWriteTime.UtcDateTime)
    {
        status = 0;
    }

    entry.ExtractToFile(outputName, true);
    File.SetCreationTimeUtc(outputName, entry.LastWriteTime.UtcDateTime);

    return (entryFileName, status);
}