export type ColumnType = 'string' | 'number' | 'date' | 'enum';

export interface TableColumnConfig<T = any> {
  /** Chiave del campo nel dato */
  key: keyof T;
  /** Etichetta visibile nell'intestazione */
  label: string;
  /** Tipo di dato per determinare il filtro */
  type: ColumnType;
  /** CSS class per il th */
  className?: string;
  /** Formatter custom per visualizzare il valore */
  formatter?: (value: any, row: T) => string;
  /** Per tipo 'enum': mappe tra valore e label */
  enumMap?: Record<string, string>;
  /** Classe CSS per l'elemento td */
  cellClassName?: string;
}

export interface TableViewConfig<T = any> {
  /** Configurazione delle colonne */
  columns: TableColumnConfig<T>[];
  /** Campo chiave unico per le righe (default: 'id') */
  keyField?: keyof T;
  /** Mostra soglia totale/bilancio per campi numerici */
  summaryField?: keyof T;
  /** Funzione custom per calcolare il totale del summary field */
  summaryCalculator?: (rows: T[]) => number;
  /** Icona per il pulsante azioni (default: 'more_vert') */
  actionsIcon?: string;
  /** Abilita ordinamento colonne (default: true) */
  sortable?: boolean;
  /** Abilita filtri (default: true) */
  filterable?: boolean;
  /** Mostra colonna azioni (default: false) */
  showActions?: boolean;
}
