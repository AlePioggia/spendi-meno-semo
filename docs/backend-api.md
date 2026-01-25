# Backend: API contracts (Expenses service)

Base URL (dev): `http://localhost:5116`

Docker: `expenses-service` espone `5001:8080` (quindi base tipica: `http://localhost:5001`).

## Category

### GET /api/category
Ritorna la lista delle categorie.

Response: `200 OK` → `GetCategoryResponseDto[]`

Nota: nel controller la query usa attualmente user/tenant fissi.

### GET /api/category/{id}
Ritorna una categoria.

Response:
- `200 OK` → `GetCategoryResponseDto`
- `404 NotFound`

### POST /api/category
Crea una categoria.

Request: `CreateCategoryRequestDto`

Response:
- `200 OK`
- `400 BadRequest` in caso di validazione

### DELETE /api/category/{id}
Soft delete.

Response:
- `200 OK`

## Transaction

### GET /api/transaction
Ritorna tutte le transazioni.

Response: `200 OK` → `GetTransactionResponseDto[]`

Nota: nel controller la query usa attualmente user/tenant fissi.

### GET /api/transaction/{id}
Ritorna una transazione.

Response:
- `200 OK` → `GetTransactionResponseDto`
- `404 NotFound`

### POST /api/transaction
Crea una transazione.

Request: `CreateTransactionRequestDto`

Campi:
- `description?: string`
- `amount: decimal`
- `currency: string` (es. `EUR`)
- `transactionType: string` (`Expense` | `Income`)
- `categoryId: long`
- `date: DateTime`

Response:
- `200 OK`
- `400 BadRequest` se `currency` / `transactionType` non sono validi

### PUT /api/transaction/{id}
Aggiorna una transazione.

Request: `UpdateTransactionRequestDto`

Response:
- `200 OK` (ritorna l’id)

### DELETE /api/transaction/{id}
Soft delete.

Response:
- `200 OK`

## Auth e autorizzazione (stato attuale)

- In `Program.cs` è configurato JWT Bearer con authority Keycloak realm `myapp`.
- `CategoryController` è marcato `[Authorize]`.
- `TransactionsController` al momento non ha `[Authorize]`.
- In più punti user/tenant sono temporaneamente hardcoded (`1, 1`).

Quando verrà completata la parte auth end-to-end, l’obiettivo è:

- `[Authorize]` su tutti i controller rilevanti
- estrazione delle claim dal token (`sub`, `tenantId`, …)
- filtri e controlli a livello Application (commands/queries) per garantire isolamento per tenant/user
