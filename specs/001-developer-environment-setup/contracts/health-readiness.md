# Approved API Contract: Process Health

**Status**: APPROVED by the technical owner on 2026-10-19.
**OpenAPI**: [health.openapi.yaml](health.openapi.yaml), version 1.0.0, OpenAPI 3.1.0.

## `GET /health/live`

Returns HTTP `200` while the API process is alive. This liveness check MUST NOT depend on
PostgreSQL; it distinguishes a running process from one that has stopped responding.

## `GET /health/ready`

Returns HTTP `200` only when PostgreSQL is reachable. Returns HTTP `503 Service Unavailable` when
PostgreSQL is unreachable. It MUST NOT expose connection strings, credentials, SQL text, or other
secret values in the response.

## Startup Configuration

The API MUST fail fast when a required environment variable is missing. Startup errors MUST name
the missing variable without printing its value. Secrets MUST NOT appear in logs or either health
response.

Both routes are read-only and MUST NOT access or disclose child/product data. They are unversioned
operational paths; the versioned OpenAPI document and its `info.version` provide the contract
version. Any incompatible contract change requires a new OpenAPI document version and review.