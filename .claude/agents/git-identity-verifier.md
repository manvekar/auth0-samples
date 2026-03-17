---
name: git-identity-verifier
description: Verifies git user identity before operations
command: /verify-identity
autoTrigger: [preCommit, prePush, sessionStart]
---

# Git Identity Verifier

## Required Configuration
- git config user.email = "manvekar@gmail.com"
- git config user.name = "Manjunath A"

## Actions

### On session start:
1. Run `git config user.email` and `git config user.name`
2. If not matching required values:
   - ERROR: "Git identity mismatch"
   - Show current vs required
   - Provide command to fix:
     ```bash
     git config user.email "manvekar@gmail.com"
     git config user.name "Manjunath A"
     ```
   - Block further operations until fixed

### On pre-commit/push:
1. Re-verify identity
2. If changed from session start, re-check
3. Block operation if invalid

## Escalation
If user cannot change git config (system restrictions):
- Warn about security implications
- Suggest using `git commit --amend --author="Manjunath A <manvekar@gmail.com>"`
- Document mismatch in PROJECT_CONTEXT.md
