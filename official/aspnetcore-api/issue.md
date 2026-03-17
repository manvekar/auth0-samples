# Issue: .NET 10.0 Incompatibility with Swashbuckle.AspNetCore

## Problem
The Auth0.AspNetCore.Authentication.Api.Playground project fails to start with a TypeLoadException when running on .NET 10.0.

## Symptoms
- `dotnet run` fails immediately
- Error: `System.TypeLoadException: Method 'GetSwagger' in type 'Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator' from assembly 'Swashbuckle.AspNetCore.SwaggerGen, Version=6.6.2.0' does not have an implementation.`

## Root Cause
The project targets .NET 10.0, which is a preview/unreleased framework version. The Swashbuckle.AspNetCore 6.6.2 package (used for Swagger/OpenAPI support) is not compatible with .NET 10.0 at this time.

## Solution
Change the target framework from `net10.0` to `net8.0` (Long-Term Support stable release) in the project file.

### Steps to Fix
1. Open `Auth0.AspNetCore.Authentication.Api.Playground.csproj`
2. Change: `<TargetFramework>net10.0</TargetFramework>`
3. To: `<TargetFramework>net8.0</TargetFramework>`
4. Run `dotnet restore`
5. Run `dotnet run --launch-profile http`

## Verification
After the change, the application should start successfully on http://localhost:5059 with Swagger UI available at http://localhost:5059/swagger.

## Additional Notes
- The .NET SDK 10.0.102 is installed on the system
- All other dependencies appear compatible with .NET 8.0
- This is a temporary workaround until Swashbuckle releases a version compatible with .NET 10.0