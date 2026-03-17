# Auth0 DPoP Playground - Learnings & Guide



**What is DPoP?**

DPoP is an OAuth 2.0 extension that binds access tokens to a specific client using public-key cryptography. It prevents token theft by requiring a cryptographic proof with every API request, ensuring only the client that obtained the token can use it.

**Key Benefits:**
- Mitigates token replay attacks
- No need for client secrets in public clients (SPAs, mobile apps)
- Application-level token binding without TLS requirements
- RFC 9449 standard implementation

---

## Prerequisites

- **Auth0 Account** with DPoP feature enabled (Professional/Enterprise plan)
- **.NET 8.0+ SDK**
- **Auth0 Resources** configured:
  - API/Resource Server with correct Audience
  - Machine-to-Machine (M2M) Application with Client ID & Secret

---

## Auth0 Configuration

### 1. Enable Sender Constraining for the API

**Important:** DPoP is only available on **Professional** or **Enterprise** plans. The feature will be disabled on Free/Essentials plans.

1. Navigate to **Auth0 Dashboard** → **Applications** → **APIs**
2. Select your API (or create one with an identifier like `https://test-api.com/test1`)
3. Under **Settings** → **Token Sender-Constraining**:
   - **Sender Constraining Method:** `DPoP`
   - **Require Token Sender Constraining:** `Always` (or `For Public Applications`)
4. Save changes

### 2. Configure the Client Application

1. Navigate to **Auth0 Dashboard** → **Applications** → **Applications**
2. Select your M2M application
3. Under **Settings** → **Token Sender-Constraining**:
   - Toggle on **Require Sender Constraining**
4. Save changes

### 3. Alternative: Management API

If the UI is disabled, use the Management API (requires upgrade):

```bash
# Enable DPoP for the API (Resource Server)
curl -X PATCH https://YOUR_DOMAIN/api/v2/resource-servers/{API_ID} \
  -H "Authorization: Bearer {MGMT_API_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "proof_of_possession": {
      "mechanism": "dpop",
      "required": true
    }
  }'

# Enable sender constraining for the client
curl -X PATCH https://YOUR_DOMAIN/api/v2/clients/{CLIENT_ID} \
  -H "Authorization: Bearer {MGMT_API_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"require_proof_of_possession": true}'
```

---

## Playground Projects Structure

```
Auth0.AspNetCore.Authentication.Api/
├── src/
│   └── Auth0.AspNetCore.Authentication.Api/     # Main SDK library
│       └── DPoP/
│           ├── DPoPProofValidationService.cs   # Core RFC 9449 validation
│           ├── DPoPOptions.cs                   # Mode settings (Allowed/Required/Disabled)
│           └── EventHandlers/                   # JWT Bearer event integrations
│
├── Auth0.AspNetCore.Authentication.Api.Playground/
│   ├── Program.cs                              # API server with DPoP middleware
│   ├── appsettings.json                        # Auth0 configuration
│   └── Properties/
│       └── launchSettings.json                 # HTTPS profiles (http + https)
│
└── Auth0.AspNetCore.Authentication.Api.Playground.DPoPClient/
    ├── DPoPClient.cs                           # DPoP proof generation & API calls
    ├── Program.cs                              # Complete flow orchestrator
    ├── .env.example                            # Template for Auth0 credentials
    └── Properties/
        └── launchSettings.json                 # HTTPS profile
```

---

## Complete DPoP Flow

### Step 1: API Server Setup (Playground)

The API server is a minimal ASP.NET Core app with:

```csharp
builder.Services.AddAuth0ApiAuthentication(options =>
{
    options.Domain = configuration["Auth0:Domain"];
    options.JwtBearerOptions = new JwtBearerOptions
    {
        Audience = configuration["Auth0:Audience"]
    };
}).WithDPoP();  // Enables DPoP validation

app.MapGet("/restricted-endpoint", () => "Protected")
   .RequireAuthorization();
```

**Key DPoP Validation Checks:**
- DPoP proof signature verification (ES256)
- Claims validation: `htm`, `htu`, `iat`, `jti`
- Token hash (`ath`) matches access token
- `cnf.jkt` claim exists in access token
- JWK thumbprint from proof matches `cnf.jkt`

### Step 2: DPoP Client Operation

The DPoP client orchestrates:

1. **Generate ES256 key pair** (once, reuse for entire flow)
2. **Request DPoP-bound token** from Auth0:
   - POST to `https://{domain}/oauth/token`
   - DPoP header with proof (no `ath` for token request)
   - Body: `grant_type=client_credentials&client_id=...&client_secret=...&audience=...`
3. **Call open endpoint** (no auth) - sanity check
4. **Call restricted endpoint**:
   - Generate new DPoP proof with `ath` (hash of access token)
   - Headers:
     - `Authorization: DPoP {access_token}`
     - `DPoP: {proof_jwt}`

---

## Execution Steps

### 1. Prepare Environment Variables

Copy the example and fill in your Auth0 credentials:

```bash
cp Auth0.AspNetCore.Authentication.Api.Playground.DPoPClient/.env.example .env
```

Edit `.env`:

```env
AUTH0_DOMAIN=your-tenant.auth0.com
AUTH0_AUDIENCE=https://your-api-identifier
AUTH0_CLIENT_ID=your-client-id
AUTH0_CLIENT_SECRET=your-client-secret
API_BASE_URL=https://localhost:7168
```

**Note:** The `.env` file is auto-loaded by `Program.cs` at startup.

### 2. Ensure HTTPS Launch Profiles Exist

Create `Files/launchSettings.json` in both playground projects:

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": { "ASPNETCORE_ENVIRONMENT": "Development" }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": { "ASPNETCORE_ENVIRONMENT": "Development" }
    }
  }
}
```

### 3. Run the API Server

Terminal 1:

```bash
cd Auth0.AspNetCore.Authentication.Api.Playground
dotnet run --launch-profile https
```

The server starts on `https://localhost:7168` (or port from `launchSettings.json`). Swagger UI is available.

### 4. Run the DPoP Client

Terminal 2:

```bash
cd Auth0.AspNetCore.Authentication.Api.Playground.DPoPClient
dotnet run --launch-profile https
```

---

## Expected Successful Output

```
======================================================================
  Complete DPoP Flow: Auth0 Token → API Call
======================================================================

✅ Loaded environment variables from .env file
📋 Configuration:
   Auth0 Domain: your-tenant.auth0.com
   Audience: https://your-api-identifier
   Client ID: abc123...
   API Base URL: https://localhost:7168

🔐 Step 1: Initialize DPoP Client
----------------------------------------------------------------------
🔑 Generating ES256 key pair for DPoP...
✅ Key pair generated successfully

🎫 Step 2: Get DPoP-Bound Token from Auth0
----------------------------------------------------------------------
🔑 Requesting DPoP-bound token from: https://your-tenant.auth0.com/oauth/token
   DPoP proof created for token request
✅ Token obtained successfully
   Access Token: eyJhbGciOiJSUzI1NiIs...

📡 Step 3: Test Open Endpoint
----------------------------------------------------------------------
📡 Making GET request to: https://localhost:7168/open-endpoint
📥 Response Status: 200 OK
✅ Open endpoint access successful!

🔒 Step 4: Test Restricted Endpoint with DPoP Token
----------------------------------------------------------------------
📡 Making GET request to: https://localhost:7168/restricted-endpoint
🎫 Access token: eyJhbGciOiJSUzI1NiIs...
📥 Response Status: 200 OK
✅ Restricted endpoint access successful!

======================================================================
✅ Complete DPoP Flow Successful!
======================================================================
```

---

## Troubleshooting

### Error 1: "Missing required environment variables"

**Cause:** `.env` file not found or variables not set.

**Fix:**
1. Ensure `.env` exists in the DPoPClient directory
2. Verify all required variables are defined (no placeholders)

---

### Error 2: "No connection could be made because the target machine actively refused it"

**Cause:** API server not running or wrong `API_BASE_URL`.

**Fix:**
1. Start the API server in a separate terminal first
2. Confirm the port matches between server (`launchSettings.json`) and client (`.env`)

---

### Error 3: "JWT Access token has no jkt confirmation claim"

**Response Header:** `WWW-Authenticate: DPoP error="invalid_token", error_description="JWT Access token has no jkt confirmation claim"`

**Cause:** Auth0 did not bind the token to a DPoP proof. This means:
- The API (Resource Server) doesn't have DPoP sender constraining enabled
- OR the client doesn't require sender constraining
- OR DPoP feature is not available on your plan

**Fix:**
1. Enable DPoP in Auth0 Dashboard for both:
   - Resource Server (API) → **Token Sender-Constraining** → `DPoP`
   - Client Application → **Require Sender Constraining**
2. Obtain a **fresh token** after enabling (old tokens won't have `cnf`)
3. If UI is disabled: Upgrade subscription or use Management API after upgrade

---

### Error 4: "Please upgrade your subscription to use proof_of_possession"

**Cause:** Your Auth0 tenant doesn't have DPoP feature activated.

**Fix:**
1. Upgrade to **Professional** or **Enterprise** plan
2. For eligible startups: Apply to [Auth0 for Startups](https://auth0.com/startups)
3. Contact Auth0 support to enable DPoP feature flag on your tenant

---

### Error 5: DPoP Proof Validation Fails (different errors)

Possible mismatches:
- **`htm`/`htu` claim mismatch**: URL construction differs between client and server
- **`ath` hash mismatch**: Access token hash doesn't match
- **Signature invalid**: Wrong key or algorithm

**Debugging:**
1. Decode the access token at [jwt.io](https://jwt.io) - check for `cnf.jkt` claim
2. Verify `htu` in DPoP proof matches exact request URL (scheme, host, port, path)
3. Ensure client uses same key pair for token request and API call

---

## Code Deep Dive

### DPoPProof Generation (DPoPClient.cs)

```csharp
public string CreateDPoPProof(string httpMethod, string httpUri, string? accessToken = null)
{
    var uri = new Uri(httpUri);
    var htu = $"{uri.Scheme}://{uri.Host}{(uri.IsDefaultPort ? "" : $":{uri.Port}")}{uri.AbsolutePath}";

    var claims = new Dictionary<string, object>
    {
        { "jti", Guid.NewGuid().ToString() },
        { "htm", httpMethod.ToUpperInvariant() },
        { "htu", htu },
        { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
    };

    if (!string.IsNullOrEmpty(accessToken))
    {
        claims["ath"] = ComputeAth(accessToken); // SHA-256 hash of token
    }

    var signingCredentials = new SigningCredentials(
        new ECDsaSecurityKey(_privateKey),
        SecurityAlgorithms.EcdsaSha256
    );

    var header = new JwtHeader(signingCredentials);
    header["typ"] = "dpop+jwt";
    header["jwk"] = new Dictionary<string, string>
    {
        { "kty", _publicKeyJwk.Kty },
        { "crv", _publicKeyJwk.Crv },
        { "x", _publicKeyJwk.X },
        { "y", _publicKeyJwk.Y }
    };

    var payload = new JwtPayload();
    foreach (var claim in claims) payload[claim.Key] = claim.Value;

    return new JwtSecurityTokenHandler().WriteToken(
        new JwtSecurityToken(header, payload)
    );
}
```

**Key Points:**
- Uses ES256 (ECDSA P-256 with SHA-256)
- `jwk` header contains raw public key components (no `kid`)
- Canonical JWK thumbprint computed as SHA-256 of sorted JSON
- `ath` is base64url-encoded SHA-256 of access token

---

## Server-Side Validation Flow

The SDK's `DPoPProofValidationService` executes:

1. **Extract proof** from `DPoP` header
2. **Extract token** from `Authorization: DPoP {token}` header
3. **Validate proof JWT:**
   - Signature (using JWK from header)
   - `exp` (if present)
   - `iat` (with `Leeway` offset)
   - `htm`, `htu` match request
   - `ath` hash matches token (if token present)
4. **Extract token claims** and validate:
   - Token signature (standard JWT validation)
   - `cnf.jkt` claim exists
5. **Thumbprint comparison:**
   - Compute thumbprint of `jwk` in proof
   - Compare to `cnf.jkt` in token
6. **Result:** Proof is valid and bound to token → allow request

---

## Important Learnings

### 1. Key Pair Reuse is Critical

The same ES256 key pair **must** be used for:
- Token request (Auth0 binds token to the public key)
- API call (proof must use the corresponding private key)

The DPoPClient holds `_privateKey` and `_publicKeyJwk` as instance fields, ensuring reuse.

---

### 2. Token Binding Happens at Token Issuance

Auth0 only includes `cnf.jkt` in the access token **if**:
- The Resource Server has DPoP enabled
- The client sent a valid DPoP proof in the token request
- The client is configured to require sender constraining (optional but recommended)

If any condition is false, Auth0 returns a regular token without `cnf`.

---

### 3. DPoP is Not Free

Contrary to initial assumption, DPoP **requires** a Professional or Enterprise subscription. Attempting to configure it on Free/Essentials plans results in:
- UI controls disabled
- Management API error: `"Please upgrade your subscription to use proof_of_possession."`

---

### 4. HTU Construction Must Match Exactly

The `htu` claim in the proof must exactly match the resource server's view of the request URL:
- Scheme (`https`)
- Host (no trailing dot)
- Port (included if non-standard; omitted if default 443/80)
- Path (absolute path, no query or fragment)

The client and server typically construct `htu` the same way, but custom middleware that rewrites URLs can break validation.

---

### 5. Token Type in Authorization Header

When using a DPoP-bound token, the `Authorization` header scheme should be `DPoP` (not `Bearer`):

```
Authorization: DPoP {access_token}
```

Auth0 returns `token_type: "DPoP"` in the token response when DPoP is used. The SDK tolerates `Bearer` as well, but `DPoP` is semantically correct.

---

### 6. Nonce for Public Clients

Public clients (SPAs, mobile apps) must handle the `DPoP-Nonce` challenge:
1. Initial token request without `nonce` → 400 with `use_dpop_nonce`
2. Extract `DPoP-Nonce` header
3. Retry with `nonce` claim in proof

This playground uses client credentials (confidential client), so nonce isn't required.

---

### 7. DPoP Modes on Resource Server

The SDK supports three modes via `DPoPOptions.Mode`:
- **Allowed** (default): Accept both Bearer and DPoP tokens
- **Required**: Reject Bearer tokens entirely
- **Disabled**: Standard JWT Bearer only

For the playground, `Allowed` works for testing. Production APIs should use `Required`.

---

## Testing Checklist

Before declaring success, ensure:

- [ ] Auth0 subscription includes DPoP (Professional+)
- [ ] Resource Server (API) has DPoP enabled as method
- [ ] Resource Server policy set to `Always` or `For Public Applications`
- [ ] Client Application has "Require Sender Constraining" toggled on
- [ ] `.env` file contains correct credentials (no placeholders)
- [ ] API server running on expected port
- [ ] DPoP client uses matching `API_BASE_URL`
- [ ] Access token decoded at jwt.io shows `cnf.jkt` claim
- [ ] Both open endpoint (200) and restricted endpoint (200) succeed

---

## Additional Resources

- **RFC 9449**: [OAuth 2.0 Demonstrating Proof of Possession (DPoP)](https://datatracker.ietf.org/doc/html/rfc9449)
- **Auth0 Docs**: [Sender Constraining](https://auth0.com/docs/secure/sender-constraining)
- **Auth0 Blog**: [Implementing DPoP with Auth0](https://auth0.com/blog/implementing-dpop-with-auth0/)
- **DPoP Interactive Playground**: [dpop.info](https://dpop.info)
- **GitHub**: [auth0/Authorization-Agent](https://github.com/auth0/Authorization-Agent) - Reference implementation

---

## Common Pitfalls

| Symptom | Likely Cause | Fix |
|---------|--------------|-----|
| 401 with "no jkt" | API DPoP not enabled | Enable in Dashboard or upgrade |
| 401 with "invalid_dpop_proof" | Proof malformed | Verify `jwk`, `htm`, `htu`, `ath` |
| 400 "use_dpop_nonce" | Public client, no nonce | Implement nonce flow |
| Token missing `cnf` | Client doesn't require PoP | Enable for client application |
| UI controls disabled | Plan doesn't include DPoP | Upgrade subscription |

---

## Conclusion

DPoP significantly enhances OAuth 2.0 security by binding tokens to client keys. The Auth0 SDK provides robust RFC 9449 implementation. The main blocker is the **plan requirement** - ensure your tenant has DPoP enabled before investing time in implementation.

The playground projects serve as complete reference implementations:
- **Playground API**: Shows server-side DPoP validation with fluent configuration
- **DPoPClient**: Demonstrates client-side proof generation and token acquisition

Use these as templates for your production implementations.

---

*Last updated: 2026-03-17*  
*Tested with: Auth0.AspNetCore.Authentication.Api (latest), .NET 8.0*
