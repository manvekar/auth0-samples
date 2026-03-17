#!/usr/bin/env powershell
Write-Host "🔧 Setting up security hooks..." -ForegroundColor Cyan

# Install pre-commit hooks
Write-Host "Installing pre-commit hook..." -ForegroundColor Yellow
pre-commit install

# Verify installation
if (Test-Path .git/hooks/pre-commit) {
    Write-Host "✅ pre-commit installed" -ForegroundColor Green
} else {
    Write-Warning "pre-commit not found. Ensure pre-commit is in PATH."
}

# Create .secrets.baseline for detect-secrets
if (-not (Test-Path .secrets.baseline)) {
    Write-Host "Creating initial detect-secrets baseline..." -ForegroundColor Yellow
    detect-secrets scan --baseline .secrets.baseline
    Write-Host "✅ Baseline created (review and commit .secrets.baseline)" -ForegroundColor Green
}

Write-Host "`n🔧 Setting up PROJECT_CONTEXT.md template..." -ForegroundColor Cyan
if (-not (Test-Path PROJECT_CONTEXT.md)) {
    Copy-Item templates/PROJECT_CONTEXT.md.template PROJECT_CONTEXT.md
    Write-Host "✅ PROJECT_CONTEXT.md created from template" -ForegroundColor Green
}

Write-Host "`n🎉 Setup complete!" -ForegroundColor Green
Write-Host "Next steps:"
Write-Host "1. Review and commit .secrets.baseline"
Write-Host "2. Configure your git identity if not done:"
Write-Host "   git config user.email `"manvekar@gmail.com`""
Write-Host "   git config user.name `"Manjunath A`""
Write-Host "3. Start coding - agents will auto-load security skills" -ForegroundColor Cyan
