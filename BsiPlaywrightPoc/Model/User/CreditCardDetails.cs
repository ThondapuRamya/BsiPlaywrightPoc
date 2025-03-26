namespace BsiPlaywrightPoc.Model.User
{
    public class CreditCardDetails
    {
        public required string Name { get; set; }
        public required string? Number { get; set; }
        public required string? Expiry { get; set; }
        public required string? CVV { get; set; }
    }
}
