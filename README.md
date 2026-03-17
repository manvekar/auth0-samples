# Auth0 Samples

Collection of Auth0 integration samples, libraries, and playground applications.

## Repository Structure

This repository contains multiple Auth0-related projects under the `official/` directory, each with its own documentation:

### [official/aspnetcore-api](/official/aspnetcore-api/)
Enhanced version of the official **Auth0 ASP.NET Core API SDK** with built-in **DPoP (Demonstration of Proof-of-Possession)** support.

- **Library**: `Auth0.AspNetCore.Authentication.Api` (NuGet package)
- **Features**: Complete JWT Bearer authentication + RFC 9449 DPoP proof-of-possession
- **Version**: 1.0.0-beta.4
- **Framework**: .NET 8.0+
- **Status**: Production-ready for DPoP scenarios
- **Documentation**: See [official/aspnetcore-api/README.md](/official/aspnetcore-api/README.md)
- **Contributing**: Follow [AGENTS.md](/AGENTS.md) for agent-assisted development

### [Future Projects]
Additional Auth0 samples will be added under `official/` as they become available.

## Development Framework

This repository uses an **AI Agent Security Framework** to ensure code quality and security:

- **Git Identity**: All commits use `manvekar@gmail.com` (enforced)
- **Secret Scanning**: Pre-commit hooks block secrets before they enter git
- **CI/CD**: Secret scanning on every push/PR
- **Task Management**: `PROJECT_CONTEXT.md` tracks work breakdown and Definition of Done
- **DoD Enforcement**: Tasks require tests green + code push before completion

See [AGENTS.md](/AGENTS.md) for complete agent instructions and policy.

## Agent Setup

Agents auto-load security skills from `.claude/` and `.kilocode/` directories. Before coding:

```bash
# Install security tools
pip install pre-commit detect-secrets
# Or: brew install gitleaks pre-commit

# Setup hooks
./scripts/setup-security-hooks.sh  # or .ps1 on Windows
```

## Contributing

All changes must follow the agent security protocols. Before pushing:

```bash
/pre-push-check  # Verify identity + secret scan
```

## License

Each project under `official/` has its own license. Refer to individual project directories for license details.
