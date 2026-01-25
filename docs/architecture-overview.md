# Architettura di massima

## Componenti

- **Client**: Angular + Angular Material, servito via Nginx in container `client`.
- **Expenses service**: .NET Web API in container `expenses-service`.
- **Keycloak**: Identity Provider / OIDC, in container `keycloak` con DB PostgreSQL dedicato.
- **SQL Server**: database del microservizio Expenses.

## Diagramma (high level)

```mermaid
flowchart LR
  U[Utente / Browser]

  U -->|HTTP GET http://localhost:4200| N[Nginx static (Angular build)<br/>container: client<br/>host:4200 -> container:80]

  U -->|REST API calls| E[Expenses API<br/>container: expenses-service<br/>host:5001 -> container:8080]

  U -->|OIDC login| K[Keycloak<br/>container: keycloak<br/>host:8080 -> container:8080]

  E -->|JDBC (docker network)| MSSQL[(SQL Server<br/>container: sqlserver<br/>host:1433 -> container:1433)]

  K -->|JDBC (docker network)| PG[(PostgreSQL<br/>container: keycloak-postgres<br/>host:5432 -> container:5432)]
```

Nota importante: il frontend gira nel **browser**, quindi le chiamate API/OIDC partono dal browser verso le porte esposte sull’host (non “da container a container”).

## Porte e networking (Docker)

Riferimento: `docker-compose.yml`

- `client`: `4200:80`
- `expenses-service`: `5001:8080`
- `keycloak`: `8080:8080`
- `sqlserver`: `1433:1433`
- `postgres`: `5432:5432`

Tutti i container sono nella rete `app-network`.

## Dev locale (senza Docker per l’API)

Al momento i servizi Angular puntano a `http://localhost:5116` (es. `CategoryService`, `TransactionService`), quindi in modalità dev tipica:

- `expenses-service` gira in locale su `http://localhost:5116`
- Keycloak rimane su `http://localhost:8080`

In modalità full-docker invece l’API è su `http://localhost:5001`.

## Flusso auth (semplificato)

1. Il client avvia l’inizializzazione Keycloak e redirige l’utente al login.
2. Keycloak autentica e rilascia token OIDC.
3. Il client allega il bearer token alle chiamate verso `expenses-service`.

Nota: nel codice attuale del backend alcuni punti sono ancora “temporanei” (userId/tenantId hardcoded in alcuni endpoint). Vedi [Backend: API contracts](./backend-api.md).
