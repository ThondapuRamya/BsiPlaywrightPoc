using System.Security.Cryptography.X509Certificates;

namespace BsiPlaywrightPoc.Model.Api
{
    public class NamedApiClientObject
    {
        public string? ClientName { get; set; }
        public string? ClientBaseUrl { get; set; }
        public bool AllowAutoRedirect { get; set; }
        public X509Certificate? TransportCertificate { get; set; }
        public IList<KeyValuePair<string, string>>? ApiHeaders { get; set; }
    }
}
