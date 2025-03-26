using BsiPlaywrightPoc.Model;
using BsiPlaywrightPoc.Model.Enums;

namespace BsiPlaywrightPoc.TestData
{
    public static class StandardsData
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static Standard GetStandardBySapId(this string sapId)
        {
            return Standards.FirstOrDefault(s => s.SapId == sapId)!;
        }

        public static Standard GetStandardByName(this string name)
        {
            return Standards.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase))!;
        }

        public static string GetRandomStandardSapId(this PurchaseType purchaseType)
        {
            var random = new Random();

            // Determine the valid purchase types to filter by
            var validPurchaseTypes = purchaseType switch
            {
                PurchaseType.DigitalCopy => new[] { PurchaseType.DigitalCopyOnly, PurchaseType.DigitalAndHardCopy },
                PurchaseType.HardCopy => new[] { PurchaseType.HardCopyOnly, PurchaseType.DigitalAndHardCopy },
                PurchaseType.DigitalAndHardCopy => new[] { PurchaseType.DigitalAndHardCopy },
                PurchaseType.DigitalCopyOnly => new[] { PurchaseType.DigitalCopyOnly },
                PurchaseType.HardCopyOnly => new[] { PurchaseType.HardCopyOnly },
                _ => throw new ArgumentOutOfRangeException(nameof(purchaseType), "Invalid purchase type provided.")
            };

            // Filter the dictionary to get the keys (SAP IDs) that match the valid purchase types
            var matchingSapIds = RandomSapIds
                .Where(x => validPurchaseTypes.Contains(x.Value))
                .Select(x => x.Key)
                .ToList();

            // If no matching SAP IDs are found, return null or throw an exception
            if (!matchingSapIds.Any())
            {
                throw new InvalidOperationException("No SAP IDs found for the specified purchase type.");
            }

            // Return a random SAP ID from the matching ones
            return matchingSapIds[random.Next(matchingSapIds.Count)];
        }

        private static readonly List<Standard> Standards =
        [
            new Standard
            {
                Name = "standardWithInternationalRelations",
                Href = "/standard-test-method-for-compressibility-of-leather/standard",
                Title = "Standard Test Method for Compressibility of Leather",
                SapId = "000000000030431117",
                Id = "ASTM D2213 - 00(2020)",
                Files =
                [
                    $"{BaseDirectory}/TestData/Standards/Metadata/30431117.xml",
                    $"{BaseDirectory}/TestData/Standards/Pdfs/30431117.pdf"
                ]
            },

            new Standard
            {
                Name = "DigitalOnlyStandard",
                Href = "/specification-orthodontic-wire-and-tape-and-dental-ligature-wire/standard",
                Title = "Specification. Orthodontic wire and tape and dental ligature wire",
                SapId = "000000000000042234",
                Files =
                [
                    $"{BaseDirectory}/TestData/Standards/Metadata/00042234.xml",
                    $"{BaseDirectory}/TestData/Standards/Pdfs/00042234.pdf"
                ]
            }
        ];

        private static readonly Dictionary<string, PurchaseType> RandomSapIds = new()
        {
            { "000000000030416209", PurchaseType.DigitalAndHardCopy },
            { "000000000030103100", PurchaseType.DigitalAndHardCopy },
            { "000000000030376962", PurchaseType.DigitalAndHardCopy },
            { "000000000000295971", PurchaseType.DigitalAndHardCopy },
            //{ "000000000030368322", PurchaseType.DigitalAndHardCopy }, not working for vat tests
            { "000000000000042258", PurchaseType.DigitalAndHardCopy },
            { "000000000030304594", PurchaseType.DigitalAndHardCopy },
            { "000000000030260281", PurchaseType.DigitalAndHardCopy },
            { "000000000000072097", PurchaseType.DigitalAndHardCopy },
            { "000000000030341354", PurchaseType.HardCopyOnly },
            { "000000000030351768", PurchaseType.DigitalAndHardCopy }
        };
    }
}
