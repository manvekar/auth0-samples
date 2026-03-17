---
name: repository-readme-maintainer-agent
description: Ensures root README.md reflects current repository structure and important changes
command: /check-readme, /update-readme
autoTrigger: [preCommit, sessionStart]
---

# Repository README Maintainer Agent

## Purpose
Maintain accurate, up-to-date root `README.md` that describes the repository's purpose, structure, and projects. Prompt user to update when important changes occur.

## Important Changes Detection

The agent monitors for these changes in commits:

1. **New subproject added** under `official/` or `src/`
   - New directory with README, .csproj, or significant files
   - Git status shows untracked new folder

2. **Project README modified** 
   - `official/*/README.md` changed
   - Content update indicates new features, architecture changes

3. **Version bump** in any project
   - Changes to `.version` file
   - Changes to `Directory.Build.props` or `.csproj` where `<Version>` modified
   - Changes to `CHANGELOG.md` or `RELEASE_NOTES.md`

4. **Agent policy changes**
   - `AGENTS.md` modified (security/workflow rules changed)
   - New skills added to `.claude/agents/` or `.kilocode/skills/`

5. **Major dependency updates** (optional heuristic)
   - PackageReference version changes in .csproj beyond patch level

## Behavior

### On Session Start
- Check if root `README.md` exists
- If missing: Create from template (if configured)
- Read current structure and verify against filesystem
- Report: "Repository contains X projects. All documented in README."

### On Pre-Commit (when relevant files changed)
1. Check if staged changes include any "important changes" (above)
2. If yes:
   ```
   ⚠️ Important changes detected that may affect the root README.md:
   - New project: official/new-project/
   - Updated: official/aspnetcore-api/README.md
   - Version bump: 1.0.0 → 1.1.0

   Should the root README.md be updated to reflect these changes? [y/N]
   ```
3. If user responds Yes:
   - Analyze changes
   - Suggest specific updates (e.g., "Add new project section with description from its README", "Update version number for official/aspnetcore-api")
   - Offer to generate draft updates: "Generate updated README.md content?"
   - If user agrees, draft updates and write to `README.md`
   - Stage the updated `README.md` automatically (after user confirms content)

### Manual Commands
- `/check-readme`: Compare current README.md vs filesystem; report discrepancies
- `/update-readme`: Force update process; useful after manual additions

## Update Strategy

When updating `README.md`, the agent should:

1. **Preserve existing structure** - Don't rewrite entire file, amend/add sections
2. **Update version numbers** - If a project's version changed, reflect it
3. **Add new projects** - Add new section(s) under "Repository Structure"
4. **Refresh descriptions** - If project README updated, copy first paragraph/summary
5. **Keep formatting** - Maintain consistent markdown style
6. **Add dates sparingly** - Only if user requests "recent updates" section (typically avoid)

## File Format
Root `README.md` uses this template (if creating new):
```markdown
# [Repo Name]

[Overview]

## Repository Structure

### [Project Name]
[Description] - Version: X.Y.Z

[Add more as needed]
```

## Integration with Other Skills
- Coexists with `task-manager-agent`; can update `PROJECT_CONTEXT.md` to note README updates performed
- Respects `pre-push-security-gate`; README updates still require DoD verification
- Runs before pushes to ensure documentation stays current

## Error Handling
- If `README.md` malformed: Report error, don't modify
- If conflict with existing content: Use merge markers, alert user
- If uncertain about changes: Ask clarifying questions before updating
- Always allow user to skip/decline updates
