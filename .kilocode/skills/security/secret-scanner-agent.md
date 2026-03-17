---
name: secret-scanner-agent
description: Scans code for secrets before operations
command: /security-scan
autoTrigger: [preCommit, prePush]
---

# Secret Scanner Agent

## Purpose
Prevent accidental exposure of credentials, API keys, tokens, and other secrets.

## Actions

### On pre-commit:
1. Run `gitleaks protect --staged`
2. If secrets found: Block commit, show locations, suggest remediation
3. Run `detect-secrets scan` as secondary check
4. Report results to user

### On pre-push:
1. Run `gitleaks detect --source .` (full repo scan)
2. If using Docker: `docker run trufflesecurity/trufflehog:latest git --fail --results=verified`
3. Block push if any secrets detected
4. Provide remediation steps: rotate credentials, remove from history with BFG, invalidate tokens

### On manual `/security-scan`:
1. Scan entire repository (not just staged)
2. Generate SARIF report if requested
3. List all findings with severity levels
4. Suggest exclusions for false positives (add to `.gitleaks.toml`)

## Allowed Tools
- gitleaks (protect, detect)
- detect-secrets
- trufflehog (optional)
- git (to check staged changes)

## Never Allowed
- Disabling scans
- Ignoring findings
- Committing after scan fails
