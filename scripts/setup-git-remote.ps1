#!/usr/bin/env powershell
$remoteUrl = "https://github.com/manvekar/auth0-samples.git"

Write-Host "🌐 Setting up git remote..." -ForegroundColor Cyan

# Check if origin exists
$existing = git remote get-url origin 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Warning "Remote 'origin' already exists: $existing"
    $response = Read-Host "Overwrite? (y/N)"
    if ($response -eq 'y') {
        git remote remove origin
        git remote add origin $remoteUrl
        Write-Host "✅ Remote origin updated to $remoteUrl" -ForegroundColor Green
    }
} else {
    git remote add origin $remoteUrl
    Write-Host "✅ Remote origin added: $remoteUrl" -ForegroundColor Green
}

# Set default branch to main
$currentBranch = git branch --show-current
if ($currentBranch -ne "main") {
    git branch -M main
    Write-Host "✅ Current branch renamed to main" -ForegroundColor Green
}

Write-Host "`n📋 Git configuration summary:"
git remote -v
git config user.email
git config user.name

Write-Host "`n🎉 Git setup complete!" -ForegroundColor Cyan
