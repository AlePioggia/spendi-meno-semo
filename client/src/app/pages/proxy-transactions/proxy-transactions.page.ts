import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSelectModule } from '@angular/material/select';

import { TransactionService } from '../../services/transaction.service';
import { CategoryService } from '../../services/category.service';
import { TransactionType, TransactionResponseDto } from '../../interfaces/transaction.interface';
import { CategoryResponseDto } from '../../interfaces/category.interface';
import {
  TransactionCreateDialog,
  TransactionCreateDialogData,
  TransactionDialogResult
} from '../transactions/transaction-create.dialog';

@Component({
  standalone: true,
  selector: 'app-proxy-transactions',
  templateUrl: './proxy-transactions.page.html',
  styleUrl: './proxy-transactions.page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatSelectModule
  ]
})
export class ProxyTransactionsPage {
  private readonly transactionsService = inject(TransactionService);
  private readonly categoryService = inject(CategoryService);
  private readonly dialog = inject(MatDialog);
  
  private readonly italianNumericDateFormatter = new Intl.DateTimeFormat('it-IT', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  });

  private readonly today = new Date();
  private proxyTransactions = signal<TransactionResponseDto[]>([]);
  loading = signal(false);
  private isProxyDialog = false;
  
  tableFilterDate = signal('');
  tableFilterCategory = signal('');
  tableFilterDescription = signal('');
  tableFilterType = signal<'All' | TransactionType>('All');
  tableFilterAmountMin = signal('');
  tableFilterAmountMax = signal('');

  tableSortField = signal<'date' | 'category' | 'description' | 'type' | 'amount'>('date');
  tableSortDir = signal<'asc' | 'desc'>('asc');
  categories = signal<CategoryResponseDto[]>([]);
  
  constructor() {
    this.load();
  }

  private load() {
    this.loading.set(true);

    this.categoryService.getCategories().subscribe({
      next: (cats) => this.categories.set(cats),
      error: () => undefined
    });

    this.loadTransactions();
  }

  private loadTransactions() {
    this.transactionsService.getTransactions().subscribe({
      next: (transactions) => {
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        const proxyTxs = transactions.filter(t => {
          if (!t.isProxyTransaction) return false;
          
          const txDate = this.toLocalDate(t.date);
          return txDate >= today;
        });
        this.proxyTransactions.set(proxyTxs);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading transactions:', error);
        this.loading.set(false);
      }
    });
  }

  filteredTransactions = computed(() => {
    const dateFilter = this.normalize(this.tableFilterDate());
    const categoryFilter = this.normalize(this.tableFilterCategory());
    const descriptionFilter = this.normalize(this.tableFilterDescription());
    const typeFilter = this.tableFilterType();

    const minStr = this.tableFilterAmountMin().trim();
    const maxStr = this.tableFilterAmountMax().trim();

    const min = minStr.length ? Number(minStr) : NaN;
    const max = maxStr.length ? Number(maxStr) : NaN;
    const hasMin = minStr.length > 0 && Number.isFinite(min);
    const hasMax = maxStr.length > 0 && Number.isFinite(max);

    return this.proxyTransactions().filter(tx => {
      if (typeFilter !== 'All' && tx.expenseType !== typeFilter) return false;

      if (dateFilter) {
        const dateText = this.normalize(this.formatDayNumeric(tx.date));
        const dateKey = this.normalize(this.toDayKey(tx.date));
        if (!dateText.includes(dateFilter) && !dateKey.includes(dateFilter)) return false;
      }

      if (categoryFilter) {
        const cat = this.normalize(this.categoryName(tx.categoryId));
        if (!cat.includes(categoryFilter)) return false;
      }

      if (descriptionFilter) {
        const desc = this.normalize(tx.description ?? '');
        if (!desc.includes(descriptionFilter)) return false;
      }

      const amount = Number(tx.amount);
      if (hasMin && amount < min) return false;
      if (hasMax && amount > max) return false;

      return true;
    });
  });

  tableRows = computed(() => {
    const list = [...this.filteredTransactions()];
    const field = this.tableSortField();
    const dir = this.tableSortDir();
    const mul = dir === 'asc' ? 1 : -1;

    const compareString = (a: string, b: string) => a.localeCompare(b, 'it-IT', { sensitivity: 'base' });

    list.sort((a, b) => {
      if (field === 'date') {
        const byDate = this.toLocalDate(a.date).getTime() - this.toLocalDate(b.date).getTime();
        if (byDate !== 0) return byDate * mul;
        const byCreated = this.toLocalDate(a.createdAt).getTime() - this.toLocalDate(b.createdAt).getTime();
        return byCreated * mul;
      }

      if (field === 'amount') {
        const byAmount = (Number(a.amount) - Number(b.amount)) * mul;
        if (byAmount !== 0) return byAmount;
        return (this.toLocalDate(a.date).getTime() - this.toLocalDate(b.date).getTime()) * mul;
      }

      if (field === 'category') {
        return compareString(this.categoryName(a.categoryId), this.categoryName(b.categoryId)) * mul;
      }

      if (field === 'description') {
        return compareString(a.description ?? '', b.description ?? '') * mul;
      }

      // type
      return compareString(a.expenseType, b.expenseType) * mul;
    });

    return list;
  });

  tableFilteredBalance = computed(() =>
    this.filteredTransactions().reduce((sum, t) => {
      const amount = Number(t.amount);
      return t.expenseType === 'Income' ? sum + amount : sum - amount;
    }, 0)
  );

  tableSortIcon(field: 'date' | 'category' | 'description' | 'type' | 'amount'): string {
    if (this.tableSortField() !== field) return 'unfold_more';
    return this.tableSortDir() === 'asc' ? 'arrow_upward' : 'arrow_downward';
  }

  toggleTableSort(field: 'date' | 'category' | 'description' | 'type' | 'amount') {
    if (this.tableSortField() !== field) {
      this.tableSortField.set(field);
      this.tableSortDir.set('asc');
      return;
    }
    this.tableSortDir.set(this.tableSortDir() === 'asc' ? 'desc' : 'asc');
  }

  clearTableFilterDate() {
    this.tableFilterDate.set('');
  }

  clearTableFilterCategory() {
    this.tableFilterCategory.set('');
  }

  clearTableFilterDescription() {
    this.tableFilterDescription.set('');
  }

  clearTableFilterType() {
    this.tableFilterType.set('All');
  }

  clearTableFilterAmountMin() {
    this.tableFilterAmountMin.set('');
  }

  clearTableFilterAmountMax() {
    this.tableFilterAmountMax.set('');
  }

  clearTableFilters() {
    this.tableFilterDate.set('');
    this.tableFilterCategory.set('');
    this.tableFilterDescription.set('');
    this.tableFilterType.set('All');
    this.tableFilterAmountMin.set('');
    this.tableFilterAmountMax.set('');
  }

  formatDayNumeric(value: unknown): string {
    return this.italianNumericDateFormatter.format(this.toLocalDate(value));
  }

  categoryName(categoryId: number): string {
    const match = this.categories().find(c => c.id === categoryId);
    return match?.name ?? `#${categoryId}`;
  }

  openEditDialog(tx: TransactionResponseDto) {
    const initialDate = this.toDayKey(tx.date);
    this.openDialog({
      categories: this.categories(),
      initialDate,
      mode: 'edit',
      transaction: tx
    });
  }

  deleteTransaction(id: number) {
    this.loading.set(true);
    this.transactionsService.deleteTransaction(id).subscribe({
      next: () => {
        this.loadTransactions();
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  openCreateDialogForProxyTransaction() {
    const initialDate = this.toDayKey(this.today);
    this.isProxyDialog = true;
    this.openDialog({
      categories: this.categories(),
      initialDate,
      mode: 'create',
      isProxyTransaction: true
    });
  }

  private openDialog(data: TransactionCreateDialogData & { isProxyTransaction?: boolean }) {
    const dialogRef = this.dialog.open(TransactionCreateDialog, {
      width: '440px',
      data
    });

    dialogRef.afterClosed().subscribe((result: TransactionDialogResult | undefined) => {
      if (!result) return;

      this.loading.set(true);

      if (result.mode === 'create') {
        this.transactionsService.createTransaction(result.request).subscribe({
          next: () => {
            this.isProxyDialog = false;
            this.loadTransactions();
            this.loading.set(false);
          },
          error: () => {
            this.isProxyDialog = false;
            this.loading.set(false);
          }
        });
        return;
      }

      this.transactionsService.updateTransaction(result.id, result.request).subscribe({
        next: () => {
          this.isProxyDialog = false;
          this.loadTransactions();
          this.loading.set(false);
        },
        error: () => {
          this.isProxyDialog = false;
          this.loading.set(false);
        }
      });
    });
  }

  private toDayKey(value: unknown): string {
    const d = this.toLocalDate(value);
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${y}-${m}-${day}`;
  }

  private toLocalDate(value: unknown): Date {
    if (value instanceof Date) return value;

    if (typeof value === 'string') {
      const match = value.match(/^(\d{4})-(\d{2})-(\d{2})/);
      if (match) {
        const year = Number(match[1]);
        const month = Number(match[2]) - 1;
        const day = Number(match[3]);
        return new Date(year, month, day);
      }
      return new Date(value);
    }

    return new Date();
  }

  private normalize(value: unknown): string {
    return String(value ?? '').trim().toLowerCase();
  }
}
