using Newtonsoft.Json;

namespace BsiPlaywrightPoc.Model.ResponseObjects
{
    public class FeatureFlagResponseObject
    {
        [JsonProperty(PropertyName = "newProductPage")]
        public bool NewProductPage { get; set; }

        [JsonProperty(PropertyName = "changeUserType")]
        public bool ChangeUserType { get; set; }

        [JsonProperty(PropertyName = "seatAtSubSetLevel")]
        public bool SeatAtSubSetLevel { get; set; }

        [JsonProperty(PropertyName = "multipleInstitutionsPerOrganisation")]
        public bool MultipleInstitutionsPerOrganisation { get; set; }

        [JsonProperty(PropertyName = "desktopBurgerMenu")]
        public bool DesktopBurgerMenu { get; set; }

        [JsonProperty(PropertyName = "marketingPreferences")]
        public bool MarketingPreferences { get; set; }

        [JsonProperty(PropertyName = "institutionalLogin")]
        public bool InstitutionalLogin { get; set; }

        [JsonProperty(PropertyName = "notificationsList")]
        public bool NotificationsList { get; set; }

        [JsonProperty(PropertyName = "viewOnline")]
        public bool ViewOnline { get; set; }

        [JsonProperty(PropertyName = "disableDrmDownload")]
        public bool DisableDrmDownload { get; set; }

        [JsonProperty(PropertyName = "newSearchFilters")]
        public bool NewSearchFilters { get; set; }

        [JsonProperty(PropertyName = "reviewMode")]
        public bool ReviewMode { get; set; }

        [JsonProperty(PropertyName = "xmlReader")]
        public bool XmlReader { get; set; }

        [JsonProperty(PropertyName = "addEntireSeriesToBasket")]
        public bool AddEntireSeriesToBasket { get; set; }

        [JsonProperty(PropertyName = "licenseManagement")]
        public bool LicenseManagement { get; set; }

        [JsonProperty(PropertyName = "newLogos")]
        public bool NewLogos { get; set; }

        [JsonProperty(PropertyName = "replaceBsolLanguage")]
        public bool ReplaceBsolLanguage { get; set; }

        [JsonProperty(PropertyName = "blindLogin")]
        public bool BlindLogin { get; set; }

        [JsonProperty(PropertyName = "useStandardsApiForOverviewAndPreview")]
        public bool UseStandardsApiForOverviewAndPreview { get; set; }
    }
}
