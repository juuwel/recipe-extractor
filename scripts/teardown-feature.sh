#!/usr/bin/env bash
# Teardown script that finds a Helm release's namespace using jq, uninstalls the release,
# deletes PVCs and removes the namespace. Protects the production release.
# Usage: ./teardown-feature.sh <release-name>
# Example: ./teardown-feature.sh recipe-2-0-0-26-set-up-deployment

set -euo pipefail

if [ "$#" -ne 1 ]; then
  echo "Usage: $0 <release-name>" >&2
  exit 2
fi

RELEASE="$1"

# Protect production release: recipe-extractor (assumed in default namespace)
if [ "$RELEASE" = "recipe-extractor" ]; then
  echo "Refusing to uninstall production release '$RELEASE'" >&2
  exit 3
fi

# Ensure jq is available to parse helm JSON output
if ! command -v jq >/dev/null 2>&1; then
  echo "This script requires 'jq' to parse helm output. Please install jq and retry." >&2
  exit 4
fi

# Use helm JSON output and jq to find the namespace for the given release name
NS=$(helm list -A -o json | jq -r --arg name "$RELEASE" '.[] | select(.name == $name) | .namespace' | head -n1)

if [ -z "$NS" ] || [ "$NS" = "null" ]; then
  echo "Release '$RELEASE' not found in any namespace (checked helm list -A -o json)" >&2
  exit 5
fi

echo "Found release '$RELEASE' in namespace '$NS'"

echo "Uninstalling Helm release '$RELEASE' from namespace '$NS'..."
helm uninstall "$RELEASE" -n "$NS" || echo "helm uninstall failed or release already removed (ignored)"

echo "Deleting PVCs in namespace '$NS' (if any)..."
kubectl delete pvc --all -n "$NS" --ignore-not-found || true

echo "Deleting namespace '$NS'..."
kubectl delete namespace "$NS" --ignore-not-found || true

echo "Teardown complete."

echo "Note: PersistentVolumes (PVs) may remain depending on the StorageClass reclaimPolicy (e.g. 'Retain')."
