#!/usr/bin/env bash

set -e

# Parse argument
BUMP_TYPE="$1"
if [[ "$BUMP_TYPE" != "--major" && "$BUMP_TYPE" != "--minor" && "$BUMP_TYPE" != "--patch" ]]; then
  echo "Usage: $0 [--major|--minor|--patch]"
  exit 1
fi

# Extract current version from pyproject.toml
CUR_VERSION=$(grep -E '^\s*version\s*=' pyproject.toml | head -1 | sed -E 's/^\s*version\s*=\s*\"([^\"]+)\"/\1/')
if [ -z "$CUR_VERSION" ]; then
  echo "Could not extract version from pyproject.toml"
  exit 1
fi

IFS='.' read -r MAJOR MINOR PATCH <<< "$CUR_VERSION"

# Bump version
case "$BUMP_TYPE" in
  --major)
    MAJOR=$((MAJOR + 1))
    MINOR=0
    PATCH=0
    ;;
  --minor)
    MINOR=$((MINOR + 1))
    PATCH=0
    ;;
  --patch)
    PATCH=$((PATCH + 1))
    ;;
esac

NEW_VERSION="${MAJOR}.${MINOR}.${PATCH}"

# Update pyproject.toml
sed -i.bak -E "s/^(\s*version\s*=\s*\")[^\"]+(\".*)/\\1${NEW_VERSION}\\2/" pyproject.toml

# Update Chart.yaml (appVersion quoted, version unquoted)
yq e -i ".appVersion = \"$NEW_VERSION\"" deploy/recipe-extractor/Chart.yaml
yq e -i ".version = \"$NEW_VERSION\"" deploy/recipe-extractor/Chart.yaml

# Remove quotes from version if present
sed -i 's/^version: \"\(.*\)\"$/version: \1/' deploy/recipe-extractor/Chart.yaml

# Clean up backup file
rm pyproject.toml.bak

echo "Bumped version to $NEW_VERSION"
