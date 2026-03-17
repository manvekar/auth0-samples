---
name: pre-push-security-gate
description: Multi-stage security verification before push
command: /pre-push-check
autoTrigger: [prePush]
---

# Pre-Push Security Gate

## Purpose
Ensure no secrets, wrong identity, or incomplete work gets pushed.

## Sequence (ALL must pass)

1. **Git Identity Check**
   - Run verify-git-identity
   - Fail if not manvekar@gmail.com

2. **Secret Scanning - Fast**
   - `gitleaks protect --staged` (blocking)
   - Fail if any secret found

3. **Secret Scanning - Comprehensive**
   - `detect-secrets scan` (deep scan)
   - Fail if high-confidence secrets

4. **Credential Verification** (optional if TruffleHog available)
   - `trufflehog git --fail --results=verified`
   - Actively verifies if secrets still work
   - Fail if verified active credentials

5. **Task Completion Check** (if PROJECT_CONTEXT.md exists)
   - Ensure current task status is "Completed"
   - Verify DoD checklist all checked
   - If task incomplete: Block push, show missing DoD items

6. **Uncommitted Work Check**
   - `git status --porcelain`
   - Fail if uncommitted changes remain (should be committed as part of task)

## On Success
- Display: "✅ All security checks passed. Push allowed."
- Proceed with push

## On Failure
- Display detailed failure reason
- Show how to fix
- Block push entirely
- Log incident to .security/suspicious-pushes.log
