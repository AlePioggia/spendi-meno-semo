import {
  ChangeDetectionStrategy,
  Component,
  computed,
  Input,
  OnInit,
  Output,
  EventEmitter,
  signal,
  effect
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

import { TableColumnConfig, TableViewConfig } from './table-column.interface';

@Component({
  selector: 'app-table-view',
  imports: [CommonModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './table-view.component.html',
  styleUrl: './table-view.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TableViewComponent<T = any> implements OnInit {
  @Input() data: T[] = [];
  @Input() config!: TableViewConfig<T>;
  
  @Output() rowAction = new EventEmitter<{ action: string; row: T }>();
  @Output() rowDoubleClick = new EventEmitter<T>();

  private filters = signal<Record<string, any>>({});
  sortField = signal<string | null>(null);
  sortDir = signal<'asc' | 'desc'>('asc');

  constructor() {
  }

  ngOnInit() {
    // Inizializza i filtri basati sulla configurazione
    this.initializeFilters();
  }

  private initializeFilters() {
    const cols = this.config?.columns ?? [];
    const newFilters: Record<string, any> = {};
    cols.forEach(col => {
      newFilters[String(col.key)] = col.type === 'number' ? { min: '', max: '' } : '';
    });
    this.filters.set(newFilters);
  }

  /** Dati filtrati e ordinati */
  filteredData = computed(() => {
    const filterMap = this.filters();
    const cols = this.config?.columns ?? [];
    
    let filtered = this.data.filter(row => {
      return cols.every(col => {
        const key = String(col.key);
        const value = (row as any)[key];
        const filter = filterMap[key];

        if (col.type === 'string') {
          const normalizedFilter = String(filter ?? '').toLowerCase().trim();
          if (!normalizedFilter) return true;
          // Usa il valore formattato se disponibile, altrimenti usa il valore raw
          const displayValue = this.formatValue(col, value);
          return displayValue.toLowerCase().includes(normalizedFilter);
        }

        if (col.type === 'number') {
          const minStr = (filter?.min ?? '').toString().trim();
          const maxStr = (filter?.max ?? '').toString().trim();
          const numValue = Number(value);

          if (minStr && Number.isFinite(Number(minStr))) {
            if (numValue < Number(minStr)) return false;
          }
          if (maxStr && Number.isFinite(Number(maxStr))) {
            if (numValue > Number(maxStr)) return false;
          }
          return true;
        }

        if (col.type === 'enum') {
          if (filter === 'All' || !filter) return true;
          return String(value) === filter;
        }

        if (col.type === 'date') {
          const normalizedFilter = String(filter ?? '').toLowerCase().trim();
          if (!normalizedFilter) return true;
          // Converti la data a formato YYYY-MM-DD per il confronto (come il date picker)
          const dateStr = this.getDateISOString(value);
          return dateStr.includes(normalizedFilter);
        }

        return true;
      });
    });

    if (this.sortField()) {
      const field = this.sortField() as string;
      const dir = this.sortDir() === 'asc' ? 1 : -1;
      
      filtered.sort((a, b) => {
        const valA = (a as any)[field];
        const valB = (b as any)[field];

        if (typeof valA === 'number' && typeof valB === 'number') {
          return (valA - valB) * dir;
        }

        return String(valA ?? '').localeCompare(String(valB ?? ''), 'it-IT', { sensitivity: 'base' }) * dir;
      });
    }

    return filtered;
  });

  summary = computed(() => {
    if (!this.config?.summaryField) return null;

    const key = String(this.config.summaryField);
    const sum = this.filteredData().reduce((acc, row) => {
      const value = (row as any)[key];
      return acc + (Number(value) || 0);
    }, 0);

    return sum;
  });

  get columns(): TableColumnConfig[] {
    return this.config?.columns ?? [];
  }

  setFilter(columnKey: string, value: any) {
    const current = { ...this.filters() };
    const col = this.columns.find(c => String(c.key) === columnKey);
    
    if (col?.type === 'number') {
      current[columnKey] = { ...(current[columnKey] || { min: '', max: '' }), ...value };
    } else {
      current[columnKey] = value;
    }
    this.filters.set(current);
  }

  clearFilter(columnKey: string) {
    const current = { ...this.filters() };
    const col = this.columns.find(c => String(c.key) === columnKey);
    
    if (col?.type === 'number') {
      current[columnKey] = { min: '', max: '' };
    } else {
      current[columnKey] = '';
    }
    this.filters.set(current);
  }

  clearAllFilters() {
    const newFilters: Record<string, any> = {};
    this.columns.forEach(col => {
      const key = String(col.key);
      newFilters[key] = col.type === 'number' ? { min: '', max: '' } : '';
    });
    this.filters.set(newFilters);
  }

  toggleSort(field: string) {
    if (this.sortField() === field) {
      this.sortDir.set(this.sortDir() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortField.set(field);
      this.sortDir.set('asc');
    }
  }

  getSortIcon(field: string): string {
    if (this.sortField() !== field) return 'unfold_more';
    return this.sortDir() === 'asc' ? 'arrow_upward' : 'arrow_downward';
  }

  getFilterValue(columnKey: string): any {
    return this.filters()[columnKey];
  }

  hasFiltersApplied(): boolean {
    const filterMap = this.filters();
    return Object.entries(filterMap).some(([, value]) => {
      if (typeof value === 'string') return value.trim() !== '';
      if (typeof value === 'object' && (value?.min || value?.max)) return true;
      return value && value !== 'All';
    });
  }

  isFilterActive(columnKey: string): boolean {
    const filter = this.getFilterValue(columnKey);
    if (typeof filter === 'string') return filter.trim() !== '';
    if (typeof filter === 'object' && (filter?.min || filter?.max)) return true;
    return filter && filter !== 'All';
  }

  hasRowActions(): boolean {
    return this.config.showActions ?? false;
  }

  /** Espone String per usage nel template */
  asString(value: any): string {
    return String(value);
  }

  /** Ottiene il label di un valore enum */
  getEnumLabel(column: TableColumnConfig<T>, value: any): string {
    if (!column.enumMap) return String(value ?? '');
    return column.enumMap[value] ?? String(value ?? '');
  }

  /** Lista voci enum come array di oggetti */
  getEnumOptions(enumMap?: Record<string, string>): Array<{ key: string; label: string }> {
    if (!enumMap) return [];
    return Object.entries(enumMap).map(([key, label]) => ({ key, label }));
  }

  /** Estrae e formatta il valore di una cella */
  formatRowValue(row: T, col: TableColumnConfig<T>): string {
    const value = (row as any)[col.key];
    return this.formatValue(col, value);
  }

  /** Ritorna le classi da applicare alla cella (incluse classi dinamiche) */
  getCellClasses(row: T, col: TableColumnConfig<T>): Record<string, boolean> {
    const classes: Record<string, boolean> = {};
    
    // Aggiungi classe statica se definita
    if (col.cellClassName) {
      classes[col.cellClassName] = true;
    }

    // Aggiungi classi dinamiche basate sulla colonna e sui dati
    const value = (row as any)[col.key];
    const rowData = row as any;

    // Per colonne di tipo amount, aggiungi classe income/expense
    if (col.type === 'number' && col.key === 'amount') {
      const expenseType = rowData['expenseType'];
      if (expenseType === 'Income') {
        classes['amount-income'] = true;
      } else if (expenseType === 'Expense') {
        classes['amount-expense'] = true;
      }
    }

    return classes;
  }

  formatValue(column: TableColumnConfig<T>, value: any): string {
    if (column.formatter) return column.formatter(value);
    if (column.type === 'date') return this.formatDate(value);
    return String(value ?? '');
  }

  private formatDate(value: any): string {
    if (!value) return '';
    const date = new Date(value);
    return new Intl.DateTimeFormat('it-IT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).format(date);
  }

  private getDateISOString(value: any): string {
    if (!value) return '';
    const date = new Date(value);
    return date.toISOString().split('T')[0]; // YYYY-MM-DD
  }

  onRowAction(action: string, row: T) {
    this.rowAction.emit({ action, row });
  }

  onRowDoubleClick(row: T) {
    this.rowDoubleClick.emit(row);
  }
}
