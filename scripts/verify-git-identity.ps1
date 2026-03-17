#!/usr/bin/env powershell
$requiredEmail = "manvekar@gmail.com"
$requiredName = "Manjunath A"

$currentEmail = git config user.email
$currentName = git config user.name

if ($currentEmail -ne $requiredEmail) {
    Write-Error "Git identity verification FAILED"
    Write-Error "  Current email: $currentEmail"
    Write-Error "  Required email: $requiredEmail"
    Write-Error "  Fix with: git config user.email `"$requiredEmail`""
    exit 1
}

if ($currentName -ne $requiredName) {
    Write-Warning "Git name mismatch (optional but recommended)"
    Write-Warning "  Current name: $currentName"
    Write-Warning "  Required name: $requiredName"
    Write-Warning "  Fix with: git config user.name `"$requiredName`""
    # Don't fail on name, only email is critical
}

Write-Host "✅ Git identity verified: $currentEmail <$currentName>" -ForegroundColor Green
exit 0
