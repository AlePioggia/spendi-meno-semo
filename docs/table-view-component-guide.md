# Table View Component - Guida al Riutilizzo

## Overview

Il componente `TableViewComponent` è un componente Angular generico per visualizzare dati tabulari con filtri e ordinamento intelligenti. I filtri si adattano automaticamente in base al tipo di colonna.

## Caratteristiche

- ✅ Filtri intelligenti in base al tipo di dato (string, number, date, enum)
- ✅ Ordinamento per colonna
- ✅ Somma/Bilancio configurabile per campi numerici
- ✅ Pulsanti di azione (edit/delete) opzionali
- ✅ Supporto TypeScript con generici

## Tipi di Dato Supportati

### String
- Filtro: campo di ricerca testo
- Match: case-insensitive substring match

### Number
- Filtro: range min/max
- Display: due input in fianco

### Date
- Filtro: input date HTML5
- Display: formattato in locale (it-IT)

### Enum
- Filtro: select dropdown
- Mappa tra valore e label leggibile

## Come Usare

### 1. Definire le Colonne

```typescript
import { TableViewConfig, TableColumnConfig } from '../../shared/table-view/table-column.interface';

// Definire il tipo di dato
type MyRowType = {
  id: number;
  name: string;
  amount: number;
  date: string;
  status: 'active' | 'inactive';
  currency?: string;
};

// Configurare le colonne
const tableConfig: TableViewConfig<MyRowType> = {
  columns: [
    {
      key: 'date',
      label: 'Data',
      type: 'date',
      className: 'col-date',
      formatter: (value) => new Date(value).toLocaleDateString('it-IT')
    },
    {
      key: 'name',
      label: 'Nome',
      type: 'string'
    },
    {
      key: 'amount',
      label: 'Importo',
      type: 'number',
      className: 'col-amt',
      formatter: (value) => `${Number(value).toFixed(2)} €`
    },
    {
      key: 'status',
      label: 'Stato',
      type: 'enum',
      enumMap: {
        'active': 'Attivo',
        'inactive': 'Inattivo'
      }
    }
  ],
  summaryField: 'amount',      // Mostra totale per questo campo
  sortable: true,              // (default: true)
  filterable: true,            // (default: true)
  showActions: true            // Mostra colonna azioni
};
```

### 2. Aggiungere il Componente alla Pagina

**TypeScript:**
```typescript
import { TableViewComponent } from '../../shared/table-view/table-view.component';

@Component({
  // ...
  imports: [
    CommonModule,
    TableViewComponent,
    MatProgressSpinnerModule,
    // altri moduli...
  ]
})
export class MyPage {
  myData = signal<MyRowType[]>([]);
  loading = signal(false);
  tableConfig = { /* come sopra */ };

  onTableRowAction(event: { action: string; row: MyRowType }) {
    const { action, row } = event;
    
    if (action === 'edit') {
      this.openEditDialog(row);
    } else if (action === 'delete') {
      this.deleteRow(row.id);
    }
  }
}
```

**HTML:**
```html
<app-table-view
  [data]="myData()"
  [config]="tableConfig"
  (rowAction)="onTableRowAction($event)"
>
</app-table-view>
```

### 3. Configurazione Avanzata

#### FormatterCustom
Per personalizzare il rendering di una colonna:

```typescript
{
  key: 'amount',
  label: 'Importo',
  type: 'number',
  formatter: (value) => {
    const num = Number(value);
    return `${num < 0 ? '-' : '+'}${Math.abs(num).toFixed(2)} €`;
  }
}
```

#### CSS Class Personalizzate
```typescript
{
  key: 'amount',
  label: 'Importo',
  type: 'number',
  className: 'col-amt',         // Su <th>
  cellClassName: 'amt-cell'     // Su <td>
}
```

## Interfacce

### TableViewConfig<T>
```typescript
interface TableViewConfig<T> {
  columns: TableColumnConfig<T>[];
  keyField?: keyof T;           // default: 'id'
  summaryField?: keyof T;       // Campo per totale
  sortable?: boolean;           // default: true
  filterable?: boolean;         // default: true
  showActions?: boolean;        // default: false
}
```

### TableColumnConfig<T>
```typescript
interface TableColumnConfig<T> {
  key: keyof T;
  label: string;
  type: 'string' | 'number' | 'date' | 'enum';
  className?: string;
  cellClassName?: string;
  formatter?: (value: any) => string;
  enumMap?: Record<string, string>;
}
```

## Output Events

### rowAction
Emesso quando viene cliccato un pulsante azione (edit/delete):

```typescript
type RowActionEvent<T> = {
  action: 'edit' | 'delete';
  row: T;
}
```

### rowDoubleClick
Emesso al doppio clic su una riga (non configurato in actionRow):

```typescript
interface TableViewComponent<T> {
  @Output() rowDoubleClick = new EventEmitter<T>();
}
```

## Esempio Completo

Vedi l'implementazione in:
- [`proxy-transactions.page.ts`](../client/src/app/pages/proxy-transactions/proxy-transactions.page.ts)
- [`proxy-transactions.page.html`](../client/src/app/pages/proxy-transactions/proxy-transactions.page.html)

## Miglioramenti Futuri

- [ ] Paginazione
- [ ] Esportazione dati (CSV, Excel)
- [ ] Selezione multipla righe
- [ ] Drag & drop colonne
- [ ] Colonne congelate (sticky)
