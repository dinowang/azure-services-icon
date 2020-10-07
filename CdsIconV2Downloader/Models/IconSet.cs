namespace CdsIconV2Downloader.Models
{
    public class IconSet
    {
        public string libraryKey { get; set; }
        public MasterDownloadMetadata masterDownloadMetadata { get; set; }
        public CollectionDownloadMetadata[] collectionDownloadMetadata { get; set; }
        public CollectionMetadata[] collectionMetadata { get; set; }
        public string primaryFont { get; set; }
        public string name { get; set; }
        public string iconType { get; set; }
        public string version { get; set; }
        public string modifiedOn { get; set; }
        public int collectionCount { get; set; }
        public int iconCount { get; set; }
        public bool showWarning { get; set; }
        public string warningText { get; set; }
        public string requestIconUrl { get; set; }
    }

}