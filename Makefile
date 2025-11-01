.PHONY: sanitize-branch-name

# Sanitize branch name for Kubernetes/Docker/DNS compatibility
# - Convert to lowercase (k8s requirement)
# - Replace non-alphanumeric chars with hyphens (DNS/k8s requirement)
# - Remove leading/trailing hyphens (DNS requirement)
sanitize-branch-name:
    @if [ -z "$(BRANCH)" ]; then \
        echo "Error: BRANCH is required" >&2; \
        exit 1; \
    fi
    @echo "$(BRANCH)" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9-]/-/g' | sed 's/^-//;s/-$//'
