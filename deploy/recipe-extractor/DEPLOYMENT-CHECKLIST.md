# Deployment Checklist

Use this checklist when deploying the Recipe Parser application for the first time or to a new environment.

## Pre-Deployment

- [ ] Kubernetes cluster is running and accessible
- [ ] `kubectl` is configured and connected to the correct cluster
- [ ] Helm 3.x is installed
- [ ] Traefik ingress controller is installed in the cluster
- [ ] Doppler operator is installed in the cluster
- [ ] DNS is configured to point to your ingress IP

## Secret Configuration

- [ ] Doppler project created (`recipe-parser`)
- [ ] Doppler config created (e.g., `prod`, `dev`)
- [ ] All required secrets configured in Doppler:
  - [ ] `NOTION_API_KEY`
  - [ ] `NOTION_DATABASE_ID`
  - [ ] `NOTION_VERSION`
  - [ ] `DATABASE_CONNECTION_STRING`
  - [ ] `WEBHOOK_SECRET_KEY`
  - [ ] `WEBHOOK_SALT`
- [ ] Doppler token secret created in Kubernetes

## Image Registry

- [ ] GitHub Container Registry (GHCR) access configured
- [ ] Both Docker images built and pushed:
  - [ ] `ghcr.io/juuwel/extraction-service:<tag>`
  - [ ] `ghcr.io/juuwel/recipe-service:<tag>`
- [ ] Image pull secret created in Kubernetes namespace

## Values Configuration

- [ ] `values.yaml` updated with correct values:
  - [ ] Domain/host name
  - [ ] Image tags (if not using `latest`)
  - [ ] Resource limits adjusted if needed
  - [ ] Doppler project/config names
  - [ ] Replica counts

## Deployment

- [ ] Namespace created: `kubectl create namespace recipe-parser`
- [ ] Image pull secret created in namespace
- [ ] Doppler token secret exists in `doppler-operator-system` namespace
- [ ] Helm chart deployed: `helm upgrade --install recipe-extractor ./deploy/recipe-extractor -n recipe-parser`
- [ ] Wait for pods to be ready: `kubectl wait --for=condition=ready pod -l component=recipe-service -n recipe-parser --timeout=300s`

## Verification

- [ ] All pods are running:
  ```bash
  kubectl get pods -n recipe-parser
  ```
- [ ] Services are created:
  ```bash
  kubectl get svc -n recipe-parser
  ```
- [ ] Secrets synced from Doppler:
  ```bash
  kubectl get secret recipe-extractor-secret -n recipe-parser
  ```
- [ ] Ingress route is configured:
  ```bash
  kubectl get ingressroute -n recipe-parser
  ```
- [ ] PVC is bound:
  ```bash
  kubectl get pvc -n recipe-parser
  ```

## Testing

- [ ] Access application via browser: `https://<your-domain>/api`
- [ ] Test webhook endpoint: `https://<your-domain>/webhook`
- [ ] Check logs for errors:
  ```bash
  kubectl logs -f deployment/recipe-extractor-recipe -n recipe-parser
  kubectl logs -f deployment/recipe-extractor-extraction -n recipe-parser
  ```
- [ ] Verify internal service communication:
  - Recipe service can reach extraction service
  - Check extraction service logs for incoming requests

## Post-Deployment

- [ ] Monitor resource usage:
  ```bash
  kubectl top pods -n recipe-parser
  ```
- [ ] Set up monitoring/alerting (if applicable)
- [ ] Document the deployment in your team wiki
- [ ] Schedule regular backups of the SQLite database (PVC)
- [ ] Create a rollback plan

## Rollback (if needed)

If something goes wrong:

```bash
# Check history
helm history recipe-extractor -n recipe-parser

# Rollback to previous version
helm rollback recipe-extractor -n recipe-parser

# Or rollback to specific revision
helm rollback recipe-extractor <revision> -n recipe-parser
```

## Notes

- Default resource limits: 500m CPU, 512Mi memory per service
- Default replica count: 1 per service
- Database storage: 1Gi (can be adjusted in `database.yaml`)
- Both services use ClusterIP (not exposed directly outside the cluster)
