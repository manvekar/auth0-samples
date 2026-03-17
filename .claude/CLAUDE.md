# Claude Code - Auth0 SDK Project

## Auto-Loaded Agents
The following agents are automatically active:
- **secret-scanner-agent** - Scans for secrets before commit/push
- **git-identity-verifier** - Ensures commits use manvekar@gmail.com
- **pre-push-security-gate** - Comprehensive security check
- **task-manager-agent** - Manages PROJECT_CONTEXT.md and task lifecycle

## Startup Routine
1. Check `PROJECT_CONTEXT.md` for incomplete tasks
2. If incomplete tasks exist: Display summary, ask which to continue
3. Load parent `../AGENTS.md` rules (cascading)
4. Verify git identity (`/verify-identity`)
5. Run security scan on current changes

## Task Workflow
When starting new work:
1. `/new-task` - Create task entry in PROJECT_CONTEXT.md
2. Break down into atomic subtasks if needed
3. Complete subtasks one by one, verifying DoD each time
4. Push after each task completion
5. Update PROJECT_CONTEXT.md with status

## Commands
- `/security-scan` - Full scan
- `/verify-identity` - Git check
- `/pre-push-check` - Pre-push verification
- `/show-tasks` - List tasks from PROJECT_CONTEXT.md
- `/new-task <title>` - Add new task
- `/complete-task <id>` - Mark task done (requires DoD proof)
- `/continue-task <id>` - Resume incomplete task

## Rules
- NEVER commit without running security scan
- NEVER push without verifying git identity
- ALWAYS push after task completion
- ALWAYS update PROJECT_CONTEXT.md
- PRIORITIZE incomplete tasks over new work
