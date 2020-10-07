namespace CdsIconV2Downloader.Models
{
    public class Icon
    {
        public string libraryName { get; set; }
        public object description { get; set; }
        public string key { get; set; }
        public int size { get; set; }
        public string[] fonts { get; set; }
        public string friendlyName { get; set; }
        public string glyph { get; set; }
        public object keywords { get; set; }
        public object metaphors { get; set; }
        public object unicodeValue { get; set; }
        public string svgXml { get; set; }
        public string svgKey { get; set; }
        public object svgPath { get; set; }
        public IconFamily iconFamily { get; set; }
    }

}