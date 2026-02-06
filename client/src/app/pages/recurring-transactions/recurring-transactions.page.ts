import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';

import { CategoryResponseDto } from '../../interfaces/category.interface';
import { GetRecurringTransactionResponseDto } from '../../interfaces/recurring-transaction.interface';
import { CategoryService } from '../../services/category.service';
import { RecurringTransactionService } from '../../services/recurring-transaction.service';
import {
  RecurringTransactionDialogResult,
  RecurringTransactionUpsertDialog,
  RecurringTransactionUpsertDialogData
} from './recurring-transaction-upsert.dialog';

@Component({
  standalone: true,
  selector: 'app-recurring-transactions-page',
  templateUrl: './recurring-transactions.page.html',
  styleUrl: './recurring-transactions.page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ]
})
export class RecurringTransactionsPage {
  private readonly recurringService = inject(RecurringTransactionService);
  private readonly categoryService = inject(CategoryService);
  private readonly dialog = inject(MatDialog);

  loading = signal(false);
  categories = signal<CategoryResponseDto[]>([]);
  recurringTransactions = signal<GetRecurringTransactionResponseDto[]>([]);

  displayedColumns = ['description', 'frequency', 'range', 'category', 'template', 'createdAt', 'actions'];

  constructor() {
    this.load();
  }

  categoryName = computed(() => {
    const map = new Map<number, string>();
    for (const c of this.categories()) map.set(c.id, c.name);
    return (id: number) => map.get(id) ?? `#${id}`;
  });

  formatDate(value: unknown): string {
    const d = this.toLocalDate(value);
    return new Intl.DateTimeFormat('it-IT', { day: '2-digit', month: '2-digit', year: 'numeric' }).format(d);
  }

  formatDateTime(value: unknown): string {
    const d = this.toLocalDate(value);
    return new Intl.DateTimeFormat('it-IT', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(d);
  }

  load() {
    this.loading.set(true);

    this.categoryService.getCategories().subscribe({
      next: (cats) => this.categories.set(cats),
      error: () => undefined
    });

    this.loadRecurring();
  }

  private loadRecurring() {
    this.recurringService.getRecurringTransactions().subscribe({
      next: (list) => {
        this.recurringTransactions.set(list);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  openCreateDialog() {
    this.openDialog({
      categories: this.categories(),
      mode: 'create'
    });
  }

  openEditDialog(recurring: GetRecurringTransactionResponseDto) {
    this.openDialog({
      categories: this.categories(),
      mode: 'edit',
      recurring
    });
  }

  private openDialog(data: RecurringTransactionUpsertDialogData) {
    const dialogRef = this.dialog.open(RecurringTransactionUpsertDialog, {
      width: '520px',
      data
    });

    dialogRef.afterClosed().subscribe((result: RecurringTransactionDialogResult | undefined) => {
      if (!result) return;

      this.loading.set(true);

      if (result.mode === 'create') {
        this.recurringService.createRecurringTransaction(result.request).subscribe({
          next: () => this.loadRecurring(),
          error: () => this.loading.set(false)
        });
        return;
      }

      this.recurringService.updateRecurringTransaction(result.id, result.request).subscribe({
        next: () => this.loadRecurring(),
        error: () => this.loading.set(false)
      });
    });
  }

  deleteRecurring(id: number) {
    this.loading.set(true);
    this.recurringService.deleteRecurringTransaction(id).subscribe({
      next: () => this.loadRecurring(),
      error: () => this.loading.set(false)
    });
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
}
