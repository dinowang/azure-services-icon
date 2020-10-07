using System.Collections.Generic;

namespace CdsIconDownloader.Models
{
    public class Services : List<Service> { }

    public class Service
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Category { get; set; }

        public string Filename { get; set; }

        public string Name { get; set; }

        public string Revised { get; set; }

        public bool PlaceHolder { get; set; }
    }
}