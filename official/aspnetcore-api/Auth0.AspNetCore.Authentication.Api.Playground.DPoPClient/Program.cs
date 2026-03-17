using Auth0.AspNetCore.Authentication.Api.DPoPClient;

/// <summary>
/// Complete DPoP Flow: Token Acquisition + API Calls
///
/// This application demonstrates the complete DPoP flow:
/// 1. Generate DPoP key pair
/// 2. Request DPoP-bound token from Auth0
/// 3. Make DPoP-bound API calls
///
/// Usage:
///     Set environment variables:
///     - AUTH0_DOMAIN
///     - AUTH0_AUDIENCE
///     - AUTH0_CLIENT_ID
///     - AUTH0_CLIENT_SECRET
///     - API_BASE_URL (optional, defaults to https://localhost:7168)
///
///     Then run:
///     dotnet run
/// </summary>
class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("  Complete DPoP Flow: Auth0 Token → API Call");
            Console.WriteLine(new string('=', 70));
            Console.WriteLine();

            // Load .env file if present
            LoadEnvFile();

            // Load configuration
            var config = LoadConfiguration();

            Console.WriteLine("📋 Configuration:");
            Console.WriteLine($"   Auth0 Domain: {config.Domain}");
            Console.WriteLine($"   Audience: {config.Audience}");
            Console.WriteLine($"   Client ID: {config.ClientId[..Math.Min(20, config.ClientId.Length)]}...");
            Console.WriteLine($"   API Base URL: {config.ApiBaseUrl}");
            Console.WriteLine();

            // Initialize DPoP client
            Console.WriteLine("🔐 Step 1: Initialize DPoP Client");
            Console.WriteLine(new string('-', 70));

            using var client = new DPoPClient(config.ApiBaseUrl);
            Console.WriteLine();

            // Get DPoP-bound token from Auth0
            Console.WriteLine("🎫 Step 2: Get DPoP-Bound Token from Auth0");
            Console.WriteLine(new string('-', 70));

            string accessToken;
            try
            {
                accessToken = await client.GetDPoPTokenAsync(config);
                Console.WriteLine($"   Access Token: {accessToken[..Math.Min(60, accessToken.Length)]}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed: {ex.Message}");
                return 1;
            }
            Console.WriteLine();

            // Test open endpoint (no authentication)
            Console.WriteLine("📡 Step 3: Test Open Endpoint");
            Console.WriteLine(new string('-', 70));
            var successOpen = await client.TestOpenEndpointAsync();
            Console.WriteLine();

            // Test restricted endpoint (with DPoP authentication)
            Console.WriteLine("🔒 Step 4: Test Restricted Endpoint with DPoP Token");
            Console.WriteLine(new string('-', 70));
            var successRestricted = await client.TestRestrictedEndpointAsync(accessToken);
            Console.WriteLine();

            // Summary
            Console.WriteLine(new string('=', 70));
            if (successOpen && successRestricted)
            {
                Console.WriteLine("✅ Complete DPoP Flow Successful!");
                Console.WriteLine();
                Console.WriteLine("Summary:");
                Console.WriteLine("  1. ✅ Generated ES256 key pair");
                Console.WriteLine("  2. ✅ Obtained DPoP-bound token from Auth0");
                Console.WriteLine("  3. ✅ Accessed open endpoint");
                Console.WriteLine("  4. ✅ Accessed restricted endpoint with DPoP authentication");
                Console.WriteLine();
                Console.WriteLine("Your API is correctly configured for DPoP authentication!");
                return 0;
            }
            else
            {
                Console.WriteLine("❌ Some Tests Failed");
                if (!successOpen)
                {
                    Console.WriteLine("  - Open endpoint failed (check server)");
                }
                if (!successRestricted)
                {
                    Console.WriteLine("  - Restricted endpoint failed (check token/DPoP)");
                }
                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return 1;
        }
        finally
        {
            Console.WriteLine(new string('=', 70));
        }
    }

    static void LoadEnvFile()
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (!File.Exists(envPath))
        {
            return;
        }

        try
        {
            foreach (var line in File.ReadAllLines(envPath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
                {
                    continue;
                }

                var parts = trimmed.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim().Trim('\'', '"');
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
            Console.WriteLine("✅ Loaded environment variables from .env file");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to load .env file: {ex.Message}");
        }
    }

    static Auth0Config LoadConfiguration()
    {
        var domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN");
        var audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE");
        var clientId = Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("AUTH0_CLIENT_SECRET");
        var apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "https://localhost:7168";

        var missing = new List<string>();
        if (string.IsNullOrEmpty(domain)) missing.Add("AUTH0_DOMAIN");
        if (string.IsNullOrEmpty(audience)) missing.Add("AUTH0_AUDIENCE");
        if (string.IsNullOrEmpty(clientId)) missing.Add("AUTH0_CLIENT_ID");
        if (string.IsNullOrEmpty(clientSecret)) missing.Add("AUTH0_CLIENT_SECRET");

        if (missing.Any())
        {
            Console.WriteLine("❌ Missing required environment variables:");
            foreach (var key in missing)
            {
                Console.WriteLine($"   - {key}");
            }
            Console.WriteLine();
            Console.WriteLine("Please set the following environment variables:");
            Console.WriteLine("  export AUTH0_DOMAIN='your-domain.auth0.com'");
            Console.WriteLine("  export AUTH0_AUDIENCE='your-api-identifier'");
            Console.WriteLine("  export AUTH0_CLIENT_ID='your-client-id'");
            Console.WriteLine("  export AUTH0_CLIENT_SECRET='your-client-secret'");
            Console.WriteLine();
            Console.WriteLine("Optionally:");
            Console.WriteLine("  export API_BASE_URL='https://localhost:7168'");
            Environment.Exit(1);
        }

        return new Auth0Config(domain!, audience!, clientId!, clientSecret!, apiBaseUrl);
    }
}
