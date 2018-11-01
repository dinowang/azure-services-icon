<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Fizzler.Systems.HtmlAgilityPack</NuGetReference>
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <Namespace>Fizzler</Namespace>
  <Namespace>HtmlAgilityPack</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>Fizzler.Systems.HtmlAgilityPack</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

void Main()
{
	var client = new HttpClient();
	
	client.BaseAddress = new Uri("https://docs.microsoft.com/en-us/azure/");
	
	var index = client.GetStreamAsync("").Result;
	
	var doc = new HtmlDocument();
	doc.Load(index, Encoding.UTF8);

	var products = doc.DocumentNode
					  .QuerySelectorAll("#products ul#all div.group h3")
					  .Select(x => new 
					  { 
					      Group = x.InnerHtml, 
						  Products = x.NextSibling
						              .NextSibling
									  .QuerySelectorAll("li")
									  .Select(i => new
									  {
									  	  Group = x.InnerHtml, 
									      Name = WebUtility.HtmlEncode(i.InnerText.Trim(new[] { ' ', '\n', '\r' }).Replace("  ", " ")), 
										  Image = i.QuerySelector("img").Attributes["src"].Value 
									  })
					  })
					  .SelectMany(x => x.Products)
					  .OrderBy(x => x.Group);
	products.Dump();

	var basePath = @"E:\azure-icons\";

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

// Define other methods and classes here