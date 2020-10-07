namespace CdsIconV2Downloader.Models
{
    public class IconLibary
    {
        public object baseFileName { get; set; }
        public object createdOn { get; set; }
        public int defaultSize { get; set; }
        public object[] fontFiles { get; set; }
        public Icon[] icons { get; set; }
        public int glyphsCount { get; set; }
        public string iconType { get; set; }
        public string latestVersion { get; set; }
        public string modifiedOn { get; set; }
        public string name { get; set; }
        public int[] supportedSizes { get; set; }
    }

}