# Recipe Parser Helm Chart

This Helm chart deploys the Recipe Parser application, which consists of two microservices:

1. **Extraction Service** (Python/FastAPI) - Parses recipe websites and extracts structured data
2. **Recipe Service** (.NET) - Main API service that orchestrates recipe extraction and storage

## Architecture

```
Internet
    │
    ▼
[Traefik Ingress]
    │
    ├─► /api/*, /webhook → Recipe Service (.NET:8080)
    │                           │
    │                           ▼
    │                    Extraction Service (Python:8000)
    │                           │
    │                           ▼
    └───────────────────► [SQLite Database (PVC)]
```

## Prerequisites

- Kubernetes cluster
- Helm 3.x
- Traefik ingress controller (or modify `ingress.yaml` for your ingress)
- Doppler operator (for secrets management)
- GitHub Container Registry access (for pulling images)

## Configuration

### Required Secrets (via Doppler)

The following secrets must be configured in your Doppler project:

- `NOTION_API_KEY` - Notion API key for database access
- `NOTION_DATABASE_ID` - Notion database ID where recipes are stored
- `NOTION_VERSION` - Notion API version (e.g., "2025-09-03")
- `DATABASE_CONNECTION_STRING` - SQLite connection string (e.g., "Data Source=/app/data/recipes.db")
- `WEBHOOK_SECRET_KEY` - Secret key for webhook authentication
- `WEBHOOK_SALT` - Salt for webhook signature generation

### Values Configuration

Key configuration options in `values.yaml`:

```yaml
# Shared configuration
doppler:
  namespace: doppler-operator-system
  project: recipe-parser
  config: prod # or dev, staging, etc.

ingressRoute:
  host: recipe.buchtik.top # Your domain

# Extraction Service
extractionService:
  enabled: true
  image:
    repository: ghcr.io/juuwel/extraction-service
    tag: latest
  replicaCount: 1

# Recipe Service
recipeService:
  enabled: true
  image:
    repository: ghcr.io/juuwel/recipe-service
    tag: latest
  replicaCount: 1
```

## Installation

### 1. Create namespace

```bash
kubectl create namespace recipe-parser
```

### 2. Create image pull secret

```bash
kubectl create secret docker-registry github-secret \
  --docker-server=ghcr.io \
  --docker-username=<github-username> \
  --docker-password=<github-pat> \
  --namespace=recipe-parser
```

### 3. Install/Upgrade the chart

```bash
# From the repository root
helm upgrade --install recipe-extractor ./deploy/recipe-extractor \
  --namespace recipe-parser \
  --create-namespace
```

## Common Operations

### View deployment status

```bash
kubectl get pods -n recipe-parser
kubectl get svc -n recipe-parser
```

### View logs

```bash
# Recipe Service
kubectl logs -f deployment/recipe-extractor-recipe -n recipe-parser

# Extraction Service
kubectl logs -f deployment/recipe-extractor-extraction -n recipe-parser
```

### Update configuration

Edit `values.yaml` and upgrade:

```bash
helm upgrade recipe-extractor ./deploy/recipe-extractor -n recipe-parser
```

### Deploy specific versions

```bash
helm upgrade recipe-extractor ./deploy/recipe-extractor \
  --namespace recipe-parser \
  --set recipeService.image.tag=v1.2.3 \
  --set extractionService.image.tag=v2.0.1
```

### Disable a service temporarily

```bash
helm upgrade recipe-extractor ./deploy/recipe-extractor \
  --namespace recipe-parser \
  --set extractionService.enabled=false
```

### Scale services

```bash
# Scale recipe service to 3 replicas
helm upgrade recipe-extractor ./deploy/recipe-extractor \
  --namespace recipe-parser \
  --set recipeService.replicaCount=3
```

## Troubleshooting

### Pods not starting

Check pod events:

```bash
kubectl describe pod <pod-name> -n recipe-parser
```

### Image pull errors

Verify the image pull secret:

```bash
kubectl get secret github-secret -n recipe-parser
```

### Secrets not syncing from Doppler

Check Doppler operator:

```bash
kubectl get dopplersecret -n doppler-operator-system
kubectl logs -n doppler-operator-system deployment/doppler-kubernetes-operator
```

### Service communication issues

Test internal connectivity:

```bash
# Exec into recipe service pod
kubectl exec -it deployment/recipe-extractor-recipe -n recipe-parser -- /bin/bash

# Try to reach extraction service
curl http://recipe-extractor-extraction/
```

## Uninstall

```bash
helm uninstall recipe-extractor -n recipe-parser

# Optionally delete the namespace
kubectl delete namespace recipe-parser
```

## Development

### Testing locally with values override

Create a `values-dev.yaml`:

```yaml
ingressRoute:
  host: localhost

recipeService:
  image:
    tag: dev-latest
    pullPolicy: Always

extractionService:
  image:
    tag: dev-latest
    pullPolicy: Always
```

## Monitoring

Add monitoring annotations for Prometheus:

```yaml
recipeService:
  service:
    annotations:
      prometheus.io/scrape: "true"
      prometheus.io/port: "8080"
      prometheus.io/path: "/metrics"
```

## Future Improvements

- [ ] Add health check endpoints to both services
- [ ] Add NetworkPolicy for service isolation
- [ ] Set up service mesh (Istio/Linkerd)
- [ ] Add OpenTelemetry tracing
