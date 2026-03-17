# Repository-Wide Agent Instructions

## Context
Auth0 ASP.NET Core API SDK with DPoP support. .NET 8.0+ library providing JWT Bearer authentication with RFC 9449 proof-of-possession.

## Git Identity Enforcement
- Required git user.email: `manvekar@gmail.com`
- Required git user.name: `Manjunath A`
- Agents must verify identity before any commit/push operation
- Block operations if identity mismatch

## Security Requirements
- **Pre-commit**: All agents MUST run Gitleaks + Detect-secrets before committing
- **Pre-push**: Verify git identity + full secret scan (Gitleaks + TruffleHog verification)
- **No bypass**: Never commit `.env*`, `*.secret`, `*.pem`, `appsettings.Development.json`, or any credential files
- Sandbox mode: Restrict file access to repository root only

## Task Management Protocol
- All agents MUST create/update `PROJECT_CONTEXT.md` for any multi-step work
- Break down features into atomic tasks with clear DoD
- Incomplete tasks take priority over new work on session restart
- **Definition of Done (DoD)** for EVERY task:
  1. Implementation complete
  2. All unit tests passing (`dotnet test`)
  3. Integration tests passing (if applicable)
  4. No secret scanning violations
  5. Code committed locally
  6. **Pushed to remote repository**
  7. PR created (if working on shared branch)
- **Code must be pushed immediately** after task completion, never accumulate uncommitted work

## Cascading Rule
Child AGENTS.md files in subdirectories MUST include:
```
[Parent Rules Apply] - This project inherits all rules from ../AGENTS.md
```
Child rules can ADD restrictions but NEVER REMOVE or WEAKEN parent rules.

## Agent Skills Required
The following security skills MUST be auto-loaded in all sessions:
- `secret-scanner-agent`
- `git-identity-verifier`
- `pre-push-security-gate`
- `task-manager-agent`

## Quick Commands
- `/security-scan` - Run full secret scan
- `/verify-identity` - Check git configuration
- `/pre-push-check` - Comprehensive pre-push verification
- `/show-tasks` - Display current task breakdown
- `/complete-task <id>` - Mark task done (enforces DoD)

## Forbidden Actions
- Direct read of `.env*`, `secrets/`, `*.pem`, `*.key`
- Disabling security hooks
- Committing with wrong git identity
- Pushing without completing task DoD
- Ignoring incomplete tasks when starting new session
