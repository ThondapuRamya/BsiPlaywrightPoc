namespace BsiPlaywrightPoc.Model.User
{
    public class UserCredentials
    {
        public required string? Firstname { get; set; }
        public required string? Lastname { get; set; }
        public required string? Email { get; set; }
        public required string? Password { get; set; }
        public required string Purchase { get; set; }
        public required string PayBy { get; set; }
        public required string PurchaseQuantity { get; set; }
    }
}