# Frontend: struttura e pattern

## Stack

- Angular (standalone components)
- Angular Material (MDC)
- Routing Angular
- Keycloak (login via redirect OIDC)

## Struttura (client/src/app)

- `app.routes.ts`
  - Definisce rotte applicative (`/transactions`, `/categories`, …)

- `shared/layout/`
  - `main-layout.component.ts` contiene sidenav + header + footer e ospita il `router-outlet`
  - `header.component.ts` e `footer.component.ts` sono componenti presentazionali

- `pages/`
  - `transactions/` planner mensile + dialog di creazione
  - `categories/` lista categorie + dialog di creazione

- `services/`
  - `transaction.service.ts` e `category.service.ts` comunicano con le API REST
  - `keycloak.service.ts` centralizza init e configurazione Keycloak

- `interfaces/`
  - DTO “frontend” per allinearsi ai contratti del backend

## Pattern UI

- Layout centrato (card, max-width) per una UI consistente tra pagine.
- Dialog (MatDialog) usati per creazione rapida.
- Angular Material con tema coerente (palette blu).

## Nota su stili MDC

Angular Material (MDC) usa una struttura DOM e classi diverse rispetto alle versioni precedenti.
Quando si personalizza il sidenav/menu è necessario targettare le classi MDC corrette (es. `mat-mdc-*`, `mdc-*`).

## Auth

- L’app avvia Keycloak all’avvio e redirige al login quando richiesto.
- L’interceptor aggiunge il bearer token alle chiamate API.

(Le parti sopra dipendono anche dalla configurazione backend e dalla corretta selezione del theme in Keycloak.)
