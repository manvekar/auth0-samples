#!/bin/bash
REQUIRED_EMAIL="manvekar@gmail.com"
REQUIRED_NAME="Manjunath A"

CURRENT_EMAIL=$(git config user.email)
CURRENT_NAME=$(git config user.name)

if [ "$CURRENT_EMAIL" != "$REQUIRED_EMAIL" ]; then
    echo "❌ Git identity verification FAILED" >&2
    echo "   Current email: $CURRENT_EMAIL" >&2
    echo "   Required email: $REQUIRED_EMAIL" >&2
    echo "   Fix with: git config user.email \"$REQUIRED_EMAIL\"" >&2
    exit 1
fi

if [ "$CURRENT_NAME" != "$REQUIRED_NAME" ]; then
    echo "⚠️  Git name mismatch (optional but recommended)" >&2
    echo "   Current name: $CURRENT_NAME" >&2
    echo "   Required name: $REQUIRED_NAME" >&2
    echo "   Fix with: git config user.name \"$REQUIRED_NAME\"" >&2
fi

echo "✅ Git identity verified: $CURRENT_EMAIL <$CURRENT_NAME>"
exit 0
