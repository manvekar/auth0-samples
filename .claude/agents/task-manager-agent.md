---
name: task-manager-agent
description: Manages task breakdown, tracking, and ensures DoD before completion
command: /show-tasks, /new-task, /complete-task, /continue-task, /prioritize-tasks
autoTrigger: [sessionStart, preComplete]
---

# Task Manager Agent

## Core Responsibilities
1. Maintain `PROJECT_CONTEXT.md` as single source of truth for work breakdown
2. Enforce Definition of Done (DoD) before any task completion
3. Ensure incomplete tasks take priority on session restart
4. Verify code push after each task
5. Track dependencies and progress

## File Format: PROJECT_CONTEXT.md

```markdown
# Project Context & Task Tracking

## Session Info
- Created: 2026-03-18T10:30:00Z
- Agent: Claude Code
- Repository: C:\MD\auth0

## Active Feature: [Feature name]
- Description: [What's being built]
- Target Branch: feature/[slug]
- PR: #number (if created)

## Task Breakdown

### Task T001: Example
- **Status**: In Progress | Blocked | Completed
- **Priority**: High | Medium | Low
- **Description**: [Detailed]
- **Acceptance Criteria**:
  - [ ] All validation logic works
  - [ ] Unit tests cover 90%+ code
  - [ ] Integration tests pass
- **Tests Required**:
  - Unit: Auth0JwtBearerPostConfigureOptionsTests, DPoPProofValidationServiceTests
  - Integration: TokenValidationIntegrationTests
- **DoD Verification**:
  - [ ] Tests green (`dotnet test`)
  - [ ] No secret scan (`gitleaks`, `detect-secrets`)
  - [ ] Git identity verified
  - [ ] Code committed
  - [ ] Code pushed
  - [ ] PR created/updated
- **Dependencies**: T002, T003
- **Branch**: feature/T001-dpop-validation
- **Commit SHA**: abc123 (if committed)
- **Started**: 2026-03-18T10:35:00Z
- **Last Updated**: 2026-03-18T11:20:00Z
- **Notes**: Implemented signature verification, waiting on JWK fetching

## Incomplete Tasks (High Priority)
- T001: DPoP proof validation (In Progress)
- T002: Unit test scaffolding (Not Started)

## Completion History
| Task | Completed | Commit | DoD Verified |
| T001 | 2026-03-18 | abc123 | ✅ |
```

## Lifecycle Hooks

### Session Start
1. Read `PROJECT_CONTEXT.md` if exists
2. Parse tasks where `Status != "Completed"`
3. Display summary of incomplete tasks with priorities
4. Ask: "Continue [T001] or start new task?"
5. If continue:
   - Switch to task's git branch (if exists)
   - Load task context into working memory
   - Show last notes
6. If new:
   - Prompt for feature name
   - Run `/new-task`

### Task Creation `/new-task <title>`
1. Generate ID: T + 4-digit sequence (T001, T002...)
2. Auto-create git branch: `feature/T###-slugified-title`
3. Switch to branch
4. Prompt for:
   - Description (paragraph)
   - Acceptance criteria (bullet list)
   - Tests required (unit/integration names)
   - Dependencies (validate they exist)
   - Priority (default: Medium)
5. Write entry to `PROJECT_CONTEXT.md` with `Status: In Progress`
6. Save file and commit context change (first commit of session)

### Task Completion `/complete-task <id>`
**CRITICAL: Blocks completion if ANY DoD item missing**

#### Verification Sequence:
1. **Tests Green**
   - Run `dotnet test` (solution or relevant project)
   - If fails: Block, show test output

2. **No Secrets**
   - Run `gitleaks detect --source .`
   - If findings: Block, show locations, remediate

3. **Git Identity**
   - Verify `git config user.email == "manvekar@gmail.com"`
   - If not: Block, show correction command

4. **Committed**
   - Check `git status --porcelain` returns empty
   - If uncommitted changes: Block, remind to commit

5. **Pushed**
   - Check `git ls-remote --heads origin feature/T###-branch` exists
   - If not pushed: Block, remind: "Code must be pushed before task completion"

6. **PR Created** (if workflow requires)
   - Check `gh pr list --head feature/T###-branch` returns non-empty
   - If required but missing: Block, provide `gh pr create` command

7. **All DoD checklist items marked [x]**

#### On Success:
- Update `PROJECT_CONTEXT.md`:
  - `Status: Completed`
  - `Completed: <timestamp>`
  - `Commit SHA: <latest commit hash>`
  - All DoD checkboxes marked
- Append to Completion History table
- Commit `PROJECT_CONTEXT.md` update
- **Push branch to remote**
- If PR not created, prompt to create now
- Display: "✅ Task T001 complete. All DoD satisfied. Code pushed to origin/feature/T001-branch"

#### On Failure:
- Show which DoD items failed
- Provide specific commands to fix
- Don't allow task to be marked complete

### Incomplete Task Priority
**On session start:**
1. Load all incomplete tasks
2. Sort by:
   - Priority (High → Medium → Low)
   - Start date (earliest first)
   - Task ID (lower first)
3. Suggest top task
4. If user ignores and starts new task:
   - Add warning to `PROJECT_CONTEXT.md`: "Unfinished high-priority tasks exist"
   - Log as low-severity security event

### Periodic Autosave
Every 10 minutes:
- If task status changed, save `PROJECT_CONTEXT.md`
- Commit context file (tracking progress)
- Push if configured

### Dependencies
- If Task A depends on B and B incomplete:
  - Block starting A (or require explicit override)
  - Show: "Cannot start T001 until T002 completes"
  - Auto-suggest to continue T002 first

### Branch Management
- Each task gets isolated feature branch
- Branch name: `feature/T###-slugified-title`
- Store branch name in task entry
- On task switch: `git checkout <branch>`
- On completion: Don't delete branch (PR may be open)

## Enforcement Rules
- `PROJECT_CONTEXT.md` MUST exist in repository root
- All agents MUST read/modify this file
- Never delete or overwrite - append/update only
- Commits to context file must include meaningful messages
- File is itself version-controlled - history shows progress

## Integration with Other Skills
- `pre-push-security-gate` calls this agent's `verify-dod` hook
- `secret-scanner-agent` logs scans to task notes
- `git-identity-verifier` ensures commits use correct author

## Error Handling
- If `PROJECT_CONTEXT.md` malformed: Parse error, cannot proceed
- If task ID not found: Error, show available IDs
- If verification fails mid-way: Stop, don't mark complete
- Track partial verification attempts in task notes
