namespace Nafath.Net.Models;

public class AuthResponse
{
    public string TransactionId { get; set; }
    public string RedirectUrl { get; set; }
    public string Error { get; set; }
}
