#!/usr/bin/env bash
# Deploy a feature Helm chart + images for recipe-extractor.
# Usage: ./scripts/deploy-feature.sh <image-tag> <chart-version>
# Example: ./scripts/deploy-feature.sh 26-set-up-deployment 2.0.0-26-set-up-deployment.4ed6df6d
set -euo pipefail

IMAGE_TAG="$1" # e.g. "26-set-up-deployment"
CHART_VERSION="$2"  # e.g. "2.0.0-26-set-up-deployment.4ed6df6d"

if [ -z "$IMAGE_TAG" ] || [ -z "$CHART_VERSION" ]; then
  echo "Usage: $0 <image-tag> <chart-version>"
  exit 1
fi

# sanitize a string to be a DNS-1035 label: lowercase, alnum and '-', replace other chars with '-',
# collapse runs of '-', trim leading/trailing '-', and truncate to safe length.
sanitize() {
  local input="$1"
  local s
  s="$(printf '%s' "$input" | tr '[:upper:]' '[:lower:]' | tr '.' '-' | sed 's/[^a-z0-9-]/-/g' | sed 's/-\+/-/g' | sed 's/^-//; s/-$//')"
  s="${s:0:50}"   # keep well under 63 chars
  s="$(printf '%s' "$s" | sed 's/^-//; s/-$//')"
  printf '%s' "$s"
}

SANITIZED_CHART="$(sanitize "$CHART_VERSION")"
SANITIZED_IMAGE="$(sanitize "$IMAGE_TAG")"

# Release names for Helm can include a prefix; ensure it starts with a letter.
RELEASE_NAME="recipe-${SANITIZED_CHART}"

# Use a sanitized namespace (prefer image tag to group related resources), ensure it doesn't start with digit.
NAMESPACE="$SANITIZED_IMAGE"
if [[ "$NAMESPACE" =~ ^[0-9] ]]; then
  NAMESPACE="v$NAMESPACE"
fi
# fallback if empty for any reason
if [ -z "$NAMESPACE" ]; then
  NAMESPACE="recipe-${SANITIZED_CHART}"
fi

INGRESS_HOST="feature-${SANITIZED_IMAGE}.recipe.buchtik.top"

echo "Deploying chart version: $CHART_VERSION"
echo "Sanitized chart: $SANITIZED_CHART"
echo "Release: $RELEASE_NAME"
echo "Namespace: $NAMESPACE"
echo "Ingress host: $INGRESS_HOST"

# Install/Upgrade. Adjust --set keys to match your chart values (common keys shown).
helm upgrade --install "$RELEASE_NAME" "oci://ghcr.io/juuwel/charts/recipe-extractor" \
  --version "$CHART_VERSION" \
  --namespace "$NAMESPACE" --create-namespace \
  --wait \
  --timeout 5m \
  --set ingressRoute.host="$INGRESS_HOST" \
  --set extractionService.image.tag="$IMAGE_TAG" \
  --set recipeService.image.tag="$IMAGE_TAG"

echo "Deployed: release=$RELEASE_NAME namespace=$NAMESPACE"
echo "Preview URL: https://$INGRESS_HOST (ensure DNS/ingress rules exist)"
