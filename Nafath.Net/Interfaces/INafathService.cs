using Nafath.Net.Models;
using System.Threading.Tasks;

namespace Nafath.Net.Interfaces;

public interface INafathService
{
    Task<AuthResponse> StartSessionAsync(string idNumber, string requestId);
    Task<VerificationResponse> VerifyStatusAsync(string transactionId);
}
