# Expenses Microservizio

## 🔹 Goal

---

## 🔹 Domain Model

---

## 🔹 Database (ER Diagram)

```mermaid
erDiagram
    TRANSACTIONS {
        uuid id PK
        uuid tenant_id
        uuid user_id
        uuid category_id
        text description
        varchar expenseType
        decimal amount
        varchar currency
        date date
        timestamp created_at
    }

    CATEGORIES {
        uuid id PK
        varchar name
    }

    %% Relazioni
    TRANSACTIONS ||--o{ CATEGORIES : has
