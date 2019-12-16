using System.Collections.Generic;

namespace ServiceMapCrawler.Models
{
    public class Services : Dictionary<string, Service> { }

    public class Service
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Category { get; set; }

        public bool IsAzureProduct { get; set; }

        public IEnumerable<string> ServicesIO { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public Dictionary<string, Availability> Availability { get; set; }
    }

    public class Availability
    {
        public bool Available { get; set; }

        public bool InPreview { get; set; }

        public bool InGA { get; set; }

        public string Expectation { get; set; }
    }
}