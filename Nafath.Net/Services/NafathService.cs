using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nafath.Net.Interfaces;
using Nafath.Net.Models;

namespace Nafath.Net.Services;

public class NafathService : INafathService
{
    private readonly HttpClient _httpClient;
    private readonly NafathOptions _options;
    private readonly ILogger<NafathService> _logger;

    public NafathService(HttpClient httpClient, IOptions<NafathOptions> options, ILogger<NafathService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AuthResponse> StartSessionAsync(string idNumber, string requestId)
    {
        try
        {
            var payload = new
            {
                idNumber,
                requestId
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending Nafath StartSession request: {@Payload}", payload);

            var response = await _httpClient.PostAsync($"{_options.BaseUrl}/api/v1/auth/initiate", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Received response from Nafath: {Response}", responseContent);

            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred during Nafath StartSession request.");
            return new AuthResponse
            {
                TransactionId = null,
                RedirectUrl = null,
                Error = "HTTP request failed. Please try again later."
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Nafath StartSession request timed out.");
            return new AuthResponse
            {
                TransactionId = null,
                RedirectUrl = null,
                Error = "Request timed out."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in StartSessionAsync.");
            return new AuthResponse
            {
                TransactionId = null,
                RedirectUrl = null,
                Error = "An unexpected error occurred."
            };
        }
    }

    public async Task<VerificationResponse> VerifyStatusAsync(string transactionId)
    {
        try
        {
            var url = $"{_options.BaseUrl}/api/v1/auth/verify?transactionId={transactionId}";

            _logger.LogInformation("Sending Nafath VerifyStatus request: {Url}", url);

            var response = await _httpClient.GetAsync(url);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Received response from Nafath: {Response}", responseContent);

            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<VerificationResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred during Nafath VerifyStatus request.");
            return new VerificationResponse
            {
                Success = false,
                Status = "error",
                Message = "HTTP request failed."
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Nafath VerifyStatus request timed out.");
            return new VerificationResponse
            {
                Success = false,
                Status = "timeout",
                Message = "Request timed out."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in VerifyStatusAsync.");
            return new VerificationResponse
            {
                Success = false,
                Status = "error",
                Message = "An unexpected error occurred."
            };
        }
    }
}
