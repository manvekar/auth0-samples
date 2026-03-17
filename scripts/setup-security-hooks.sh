#!/bin/bash
set -e

echo "🔧 Setting up security hooks..."

# Install pre-commit hooks
echo "Installing pre-commit hook..."
pre-commit install

# Verify
if [ -f .git/hooks/pre-commit ]; then
    echo "✅ pre-commit installed"
else
    echo "⚠️  pre-commit not found. Ensure pre-commit is in PATH."
fi

# Create baseline for detect-secrets
if [ ! -f .secrets.baseline ]; then
    echo "Creating initial detect-secrets baseline..."
    detect-secrets scan --baseline .secrets.baseline || true
    echo "✅ Baseline created (review and commit .secrets.baseline)"
fi

# Create PROJECT_CONTEXT.md template
if [ ! -f PROJECT_CONTEXT.md ]; then
    echo "Creating PROJECT_CONTEXT.md template..."
    cp templates/PROJECT_CONTEXT.md.template PROJECT_CONTEXT.md
    echo "✅ PROJECT_CONTEXT.md created from template"
fi

echo ""
echo "🎉 Setup complete!"
echo "Next steps:"
echo "1. Review and commit .secrets.baseline"
echo "2. Configure git identity:"
echo "   git config user.email \"manvekar@gmail.com\""
echo "   git config user.name \"Manjunath A\""
echo "3. Start coding - agents will auto-load security skills"
