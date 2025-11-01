using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RecipeService.Infrastructure.Utils;

/// <summary>
/// Utility class for webhook authentication
/// </summary>
public class WebhookAuthUtils(IConfiguration configuration, ILogger<WebhookAuthUtils> logger)
{
    private readonly string _secretKey = configuration["Webhook:SecretKey"]
            ?? throw new InvalidOperationException("Webhook:SecretKey configuration is missing");
    private readonly string _salt = configuration["Webhook:Salt"]
            ?? throw new InvalidOperationException("Webhook:Salt configuration is missing");

    /// <summary>
    /// Verify salted HMAC webhook token
    /// </summary>
    public bool VerifyWebhookToken(string? token, IPAddress? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogWarning("Invalid webhook token received from IP: {IpAddress}",
                ipAddress);
            return false;
        }

        // Create salted message
        var saltedMessage = $"notion-webhook:{_salt}";

        // Generate expected token
        var expectedToken = GenerateHmacSha512(saltedMessage, _secretKey);
        // Use constant-time comparison to prevent timing attacks
        bool isValid = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(token),
            Encoding.UTF8.GetBytes(expectedToken)
        );

        if (!isValid)
        {
            logger.LogWarning("Invalid webhook token received from IP: {IpAddress}",
                ipAddress);
        }

        return isValid;
    }

    private static string GenerateHmacSha512(string message, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(messageBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
