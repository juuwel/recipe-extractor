using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace RecipeService.Infrastructure.Utils;

/// <summary>
/// Utility class for webhook authentication
/// </summary>
public class WebhookAuthUtils
{
    private readonly string _secretKey;
    private readonly string _salt;

    public WebhookAuthUtils(IConfiguration configuration)
    {
        _secretKey = configuration["Webhook:SecretKey"] 
            ?? throw new InvalidOperationException("Webhook:SecretKey configuration is missing");
        _salt = configuration["Webhook:Salt"] 
            ?? throw new InvalidOperationException("Webhook:Salt configuration is missing");
    }

    /// <summary>
    /// Verify salted HMAC webhook token
    /// </summary>
    public bool VerifyWebhookToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        // Create salted message
        var saltedMessage = $"notion-webhook:{_salt}";

        // Generate expected token
        var expectedToken = GenerateHmacSha512(saltedMessage, _secretKey);

        // Use constant-time comparison to prevent timing attacks
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(token),
            Encoding.UTF8.GetBytes(expectedToken)
        );
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
