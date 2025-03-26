using Bogus;
using BsiPlaywrightPoc.Model.User;
using TestStack.Dossier;

namespace BsiPlaywrightPoc.Helpers
{
    public static class RandomData
    {
        private static readonly Faker Faker = new Faker();

        public static UserCredentials GenerateRandomUserCredentials()
        {
            var anonymous = new AnonymousValueFixture();
            return new UserCredentials
            {
                Firstname = anonymous.Person.NameFirst(),
                Lastname = anonymous.Person.NameLast(),
                Email = $"{Guid.NewGuid()}@mailsac.com".ToLower(),
                Password = $"Pa5sw0rd{anonymous.Person.Password()}",
                Purchase = "digital copy",
                PayBy = "credit card",
                PurchaseQuantity = "1"
            };
        }

        public static CreditCardDetails GenerateCreditCardDetails()
        {
            return new CreditCardDetails
            {
                Name = "", // set later in the test
                Number = "4242424242424242",
                Expiry = DateTime.Now.AddYears(3).ToString("MM/yy"),
                CVV = "333"
            };
        }

        public static CreditCardDetails GetInvoiceCreditDetails()
        {
            return new CreditCardDetails
            {
                Name = "invoice member",
                Number = default!,
                Expiry = default!,
                CVV = default!
            };
        }

        public static UserCredentials GetInvoiceUserDetails()
        {
            return new UserCredentials()
            {
                Firstname = "invoice",
                Lastname = "member",
                Email = "accord.testmember@bsigroup.com",
                Password = "Tester123!",
                Purchase = default!,
                PayBy = default!,
                PurchaseQuantity = default!
            };
        }

        public static UserAddressDetails GetAddressDetails(string country)
        {
            var addressDetails = country.ToLower() switch
            {
                "united states" => new UserAddressDetails
                {
                    Address = "42 Abide St",
                    City = "Los Angles",
                    Country = country,
                    State = "California",
                    Zipcode = "90015",
                    Phone = "555-555-5555",
                    VatType = "Others"
                },
                "australia" => new UserAddressDetails
                {
                    Address = "51 Cullen St",
                    Suburb = "Nimbin NSW",
                    City = Faker.Address.City(),
                    Country = country,
                    State = "New South Wales",
                    Postcode = "2480",
                    Phone = "+353 44 934 7828",
                    VatType = "Others"
                },
                "canada" => new UserAddressDetails
                {
                    Address = "27 Aspen Rd",
                    City = "Happy Valley-Goose Bay",
                    Country = country,
                    Province = "Newfoundland and Labrador",
                    Postcode = "A1B 4J6",
                    Phone = "+353 44 934 7828",
                    VatType = "Others"
                },
                "united kingdom" => new UserAddressDetails
                {
                    Address = "42 Abide Pl",
                    City = "Bath",
                    Country = country,
                    County = "",
                    Postcode = "BA1 5DJ",
                    Phone = "+42 444 444 4444",
                    Vat = "GB000472631",
                    VatType = "UK"
                },
                "new zealand" => new UserAddressDetails
                {
                    Address = "237 Jackson Street, Petone",
                    City = "Lower Hutt",
                    Country = country,
                    Region = "Wellington",
                    Postcode = "5012",
                    Phone = "+353 44 934 7828",
                    VatType = "Others"
                },
                "italy" => new UserAddressDetails
                {
                    Address = "Via della Botticella, 27",
                    City = "Rome",
                    Country = country,
                    Province = "Rome",
                    Postcode = "00153",
                    Phone = "+390639727369",
                    Vat = "IT01935550812",
                    CodiceFiscale = "FNIDAX96L16C893A",
                    VatType = "EU"
            },
                "germany" => new UserAddressDetails
                {
                    Address = "Schleißheimerstraße 42",
                    City = "München",
                    Country = country,
                    Postcode = "80333",
                    Phone = "+971 42 420 4242",
                    Vat = "DE220709071",
                    VatType = "EU"
            },
                "ireland" => new UserAddressDetails
                {
                    Address = "42 Abide Blvd",
                    City = "Dublin",
                    Country = country,
                    Postcode = "W23 F854",
                    County = "Dublin",
                    Phone = "+353 44 934 7828",
                    Vat =  "IE3206488LH",
                    VatType = "EU"
            },
                _ => new UserAddressDetails
                {
                    Address = Faker.Address.StreetAddress(),
                    City = Faker.Address.City(),
                    Country = country,
                    Postcode = "",
                    VatType = "Others"
                }
            };

            //Random random;
            //Faker.Locale = GetLocaleForCountry(country);

            //var addressDetails = new UserAddressDetails
            //{
            //    Address = Faker.Address.StreetAddress(),
            //    City = Faker.Address.City(),
            //    Country = country
            //};

            //switch (country.ToLower())
            //{
            //    case "united states":
            //        addressDetails.State = Faker.Address.State();
            //        addressDetails.Zipcode = Faker.Address.ZipCode();
            //        break;

            //    case "australia":
            //        random = new Random();
            //        addressDetails.State = ValidAustralianStates[random.Next(ValidAustralianStates.Count)];
            //        var postcodeRange = AustralianStateToPostcodeRanges[addressDetails.State];
            //        addressDetails.Postcode = random.Next(postcodeRange.Min, postcodeRange.Max + 1).ToString();
            //        break;

            //    case "canada":
            //        random = new Random();
            //        addressDetails.Province = ValidCanadaProvinces[random.Next(ValidCanadaProvinces.Count)];
            //        addressDetails.Postcode = GetRandomCanadianPostalCode(addressDetails.Province);
            //        break;

            //    case "united kingdom":
            //        var anonymous = new AnonymousValueFixture();
            //        addressDetails.City = anonymous.AddressUk.City();
            //        addressDetails.County = anonymous.AddressUk.County();
            //        addressDetails.Postcode = anonymous.AddressUk.PostCode();
            //        break;

            //    case "new zealand":
            //        random = new Random();
            //        addressDetails.Region = ValidNewZealandRegions[random.Next(ValidNewZealandRegions.Count)];
            //        var postalcodeRange = NewZealandRegionToPostalcodeRanges[addressDetails.Region];
            //        addressDetails.Postcode = random.Next(postalcodeRange.Min, postalcodeRange.Max + 1).ToString();
            //        break;

            //    case "italy":
            //    case "germany":
            //        addressDetails.Postcode = Faker.Address.ZipCode();
            //        break;

            //    default:
            //        // Fallback for countries without state/province
            //        addressDetails.Postcode = Faker.Address.ZipCode();
            //        break;

            return addressDetails;
        }

        private static string GetLocaleForCountry(string country)
        {
            return country.ToLower() switch
            {
                "united states" => "en_US",
                "united kingdom" => "en_GB",
                "australia" => "en_AU",
                "canada" => "en_CA",
                "new zealand" => "en_NZ",
                "italy" => "it_IT",
                "germany" => "de_DE",
                "ireland" => "en_IE",  // Ireland locale
                "zimbabwe" => "en_US",  // Use a general English locale for Zimbabwe
                // Add more locales as needed
                _ => "en_US" // Fallback locale
            };
        }

        private static readonly List<string> ValidAustralianStates =
        [
            "Western Australia",
            "Australian Capital Territory",
            "New South Wales",
            "Victoria",
            "Northern Territory",
            "Queensland",
            "South Australia",
            "Tasmania"
        ];

        private static readonly Dictionary<string, (int Min, int Max)> AustralianStateToPostcodeRanges = new Dictionary<string, (int Min, int Max)>
        {
            { "New South Wales", (1000, 2999) },
            { "Victoria", (3000, 3999) },
            { "Queensland", (4000, 4999) },
            { "South Australia", (5000, 5999) },
            { "Western Australia", (6000, 6999) },
            { "Tasmania", (7000, 7999) },
            { "Northern Territory", (800, 999) },
            { "Australian Capital Territory", (200, 299) }
        };

        private static readonly List<string> ValidNewZealandRegions =
        [
            "Auckland",
            "Bay of Plenty",
            "Canterbury",
            "Chatham Islands",
            "Gisborne",
            "Hawke’s Bay",
            "Manawatū-Whanganui",
            "Marlborough",
            "Nelson",
            "Northland",
            "Otago",
            "Southland",
            "Taranaki",
            "Tasman",
            "Waikato",
            "Wellington",
            "West Coast",
        ];

        private static readonly Dictionary<string, (int Min, int Max)> NewZealandRegionToPostalcodeRanges = new Dictionary<string, (int Min, int Max)>
        {
            { "Auckland", (1000, 2699) },
            { "Bay of Plenty", (3000, 3999) },
            { "Canterbury", (7000, 7999) },
            { "Chatham Islands", (8942, 8942) },
            { "Gisborne", (4010, 4099) },
            { "Hawke’s Bay", (4100, 4299) },
            { "Manawatū-Whanganui", (4300, 4999) },
            { "Marlborough", (7200, 7299) },
            { "Nelson", (7000, 7099) },
            { "Northland", (1000, 0999) },
            { "Otago", (9000, 9599) },
            { "Southland", (9600, 9799) },
            { "Taranaki", (4300, 4399) },
            { "Tasman", (7000, 7099) },
            { "Waikato", (3000, 3999) },
            { "Wellington", (5000, 6999) },
            { "West Coast", (7800, 7899) },
        };

        private static readonly List<string> ValidCanadaProvinces =
        [
            "Alberta",
            "British Columbia",
            "Manitoba",
            "New Brunswick",
            "Newfoundland and Labrador",
            "Northwest Territories",
            "Nova Scotia",
            "Nunavut",
            "Ontario",
            "Prince Edward Island",
            "Quebec",
            "Saskatchewan",
            "Yukon",
        ];

        private static readonly Dictionary<string, (string Min, string Max)> CanadaProvinceToPostalcodeRanges = new Dictionary<string, (string Min, string Max)>
        {
            { "Alberta", ("T0A", "T9Z") },
            { "British Columbia", ("V0A", "V9Z") },
            { "Manitoba", ("R0A", "R9A") },
            { "New Brunswick", ("E1A", "E9G") },
            { "Newfoundland and Labrador", ("A0A", "A9Z") },
            { "Northwest Territories", ("X0A", "X1A") },
            { "Nova Scotia", ("B0A", "B9Z") },
            { "Nunavut", ("X0A", "X0C") },
            { "Ontario", ("K0A", "P9Z") },
            { "Prince Edward Island", ("C0A", "C1A") },
            { "Quebec", ("G0A", "J9Z") },
            { "Saskatchewan", ("S0A", "S9Z") },
            { "Yukon", ("Y0A", "Y1A") }
        };

        private static string GetRandomCanadianPostalCode(string province)
        {
            if (!CanadaProvinceToPostalcodeRanges.TryGetValue(province, out var range))
                throw new ArgumentException("Invalid province provided.");

            var random = new Random();
            var min = range.Min;
            var max = range.Max;

            // Handle the first letter
            var minLetter = min[0];
            var maxLetter = max[0];
            var firstLetter = (char)random.Next(minLetter, maxLetter + 1);

            // Handle the numeric part, second character in the string
            var minDigit = int.Parse(min[1].ToString());
            var maxDigit = firstLetter == minLetter ? int.Parse(max[1].ToString()) : 9;
            var digitPart = random.Next(minDigit, maxDigit + 1);

            // Handle the third character, which is a letter
            var minThirdLetter = 'A';
            var maxThirdLetter = 'Z';
            if (firstLetter == minLetter && digitPart == minDigit)
            {
                minThirdLetter = min[2];
            }
            if (firstLetter == maxLetter && digitPart == maxDigit)
            {
                maxThirdLetter = max[2];
            }

            var thirdLetter = (char)random.Next(minThirdLetter, maxThirdLetter + 1);

            // Combine to form a valid partial postal code
            var partialCode = $"{firstLetter}{digitPart}{thirdLetter}";

            // Generate a valid second part of the postal code
            var lastPart = random.Next(0, 999).ToString("D3"); // Random 3-digit number

            // Construct the final postal code
            return $"{partialCode} {lastPart[0]}{(char)random.Next('A', 'Z' + 1)}{lastPart[1]}";
        }
    }
}
