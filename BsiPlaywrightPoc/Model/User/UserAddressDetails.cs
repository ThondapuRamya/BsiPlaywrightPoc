namespace BsiPlaywrightPoc.Model.User
{
    public class UserAddressDetails
    {
        public required string? Address { get; set; }
        public required string? City { get; set; }
        public string? Postcode { get; set; }
        public string? Zipcode { get; set; }
        public string? State { get; set; } // For US, Australia, etc.
        public string? Region { get; set; } // For New Zealand, etc.
        public string? Province { get; set; } // For Canada
        public string? County { get; set; } // For UK
        public required string Country { get; set; }
        public string? Suburb { get; set; } // For Australia
        public string? Phone { get; set; }
        public string? Vat { get; set; }
        public string? CodiceFiscale { get; set; }
        public string? VatType { get; set; }
    }
}
