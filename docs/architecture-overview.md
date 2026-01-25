# Architettura di massima

## Componenti

- **Client**: Angular + Angular Material, servito via Nginx in container `client`.
- **Expenses service**: .NET Web API in container `expenses-service`.
- **Keycloak**: Identity Provider / OIDC, in container `keycloak` con DB PostgreSQL dedicato.
- **SQL Server**: database del microservizio Expenses.

## Diagramma (high level)

```mermaid
flowchart LR
  U[Utente / Browser] -->|HTTP| A[Client Angular
container: client
port: 4200]

  A -->|REST API| E[Expenses API
container: expenses-service
port: 5001 -> 8080]

  A -->|OIDC login| K[Keycloak
container: keycloak
port: 8080]

  K -->|JDBC| PG[(PostgreSQL
container: keycloak-postgres
port: 5432)]

  E -->|JDBC| MSSQL[(SQL Server
container: ${SQL_CONTAINER_NAME}
port: 1433)]
```

## Porte e networking (Docker)

Riferimento: `docker-compose.yml`

- `client`: `4200:80`
- `expenses-service`: `5001:8080`
- `keycloak`: `8080:8080`
- `sqlserver`: `1433:1433`
- `postgres`: `5432:5432`

Tutti i container sono nella rete `app-network`.

## Flusso auth (semplificato)

1. Il client avvia l’inizializzazione Keycloak e redirige l’utente al login.
2. Keycloak autentica e rilascia token OIDC.
3. Il client allega il bearer token alle chiamate verso `expenses-service`.

Nota: nel codice attuale del backend alcuni punti sono ancora “temporanei” (userId/tenantId hardcoded in alcuni endpoint). Vedi [Backend: API contracts](./backend-api.md).
