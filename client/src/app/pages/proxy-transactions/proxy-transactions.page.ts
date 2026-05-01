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

import { TableViewComponent } from '../../shared/table-view/table-view.component';
import { TableViewConfig } from '../../shared/table-view/table-column.interface';
import { TransactionService } from '../../services/transaction.service';
import { CategoryService } from '../../services/category.service';
import { TransactionType, TransactionResponseDto, UpdateTransactionRequestDto } from '../../interfaces/transaction.interface';
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
    TableViewComponent
  ]
})
export class ProxyTransactionsPage {
  private readonly transactionsService = inject(TransactionService);
  private readonly categoryService = inject(CategoryService);
  private readonly dialog = inject(MatDialog);

  private readonly today = new Date();
  proxyTransactions = signal<TransactionResponseDto[]>([]);
  loading = signal(false);
  categories = signal<CategoryResponseDto[]>([]);

  tableConfig: TableViewConfig<TransactionResponseDto> = {
    columns: [
      {
        key: 'date',
        label: 'Data',
        type: 'date',
        className: 'col-date',
        formatter: (value) =>
          new Intl.DateTimeFormat('it-IT', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
          }).format(this.toLocalDate(value))
      },
      {
        key: 'categoryId',
        label: 'Categoria',
        type: 'string',
        className: 'col-cat',
        cellClassName: 'cat',
        formatter: (value) => this.categoryName(value)
      },
      {
        key: 'description',
        label: 'Descrizione',
        type: 'string',
        cellClassName: 'desc-cell'
      },
      {
        key: 'expenseType',
        label: 'Tipo',
        type: 'enum',
        className: 'col-type',
        enumMap: {
          'Expense': 'Spesa',
          'Income': 'Entrata'
        }
      },
      {
        key: 'amount',
        label: 'Importo',
        type: 'number',
        className: 'col-amt',
        cellClassName: 'amount-cell',
        formatter: (value, row?: TransactionResponseDto) => {
          const amount = Number(value);
          const sign = row?.expenseType === 'Income' ? '+' : '-';
          return `${sign}${Math.abs(amount).toFixed(2)} €`;
        }
      }
    ],
    summaryField: 'amount',
    summaryCalculator: (rows: TransactionResponseDto[]) => {
      return rows.reduce((sum, t) => {
        const amount = Number(t.amount);
        return t.expenseType === 'Income' ? sum + amount : sum - amount;
      }, 0);
    },
    sortable: true,
    filterable: true,
    showActions: true,
    customBtns: [
      { action: 'abilitate', label: 'Abilita', tooltip: 'Rendi la transazione effettiva' , icon: 'double_arrow' }
    ]
  };

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

  onTableRowAction(event: { action: string; row: TransactionResponseDto }) {
    const { action, row } = event;

    if (action === 'edit') {
      this.openEditDialog(row);
    } else if (action === 'delete') {
      this.deleteTransaction(row.id);
    } else if (action === 'abilitate') {
      this.abilitateTransaction(row.id, row);
    }
  }

  abilitateTransaction(id: number, row: TransactionResponseDto) {
    let r: UpdateTransactionRequestDto = {
        id: row.id,
        amount: row.amount,
        categoryId: row.categoryId,
        date: row.date.toString() ?? '',
        description: row.description,
        transactionType: row.expenseType,
        currency: 'EUR',
        isProxyTransaction: false
      }
      this.transactionsService.updateTransaction(row.id, r).subscribe({
        next: () => {
          this.loadTransactions();
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      });
  }

  openEditDialog(tx: TransactionResponseDto) {
    const initialDate = this.toDayKey(tx.date);
    this.openDialog({
      categories: this.categories(),
      initialDate,
      mode: 'edit',
      transaction: tx,
      isProxyTransaction: true
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

  openCreateDialog() {
    const initialDate = this.toDayKey(this.today);
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
            this.loadTransactions();
            this.loading.set(false);
          },
          error: () => {
            this.loading.set(false);
          }
        });
        return;
      }

      this.transactionsService.updateTransaction(result.id, result.request).subscribe({
        next: () => {
          this.loadTransactions();
          this.loading.set(false);
        },
        error: () => {
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

  categoryName(categoryId: number): string {
    const match = this.categories().find(c => c.id === categoryId);
    return match?.name ?? `#${categoryId}`;
  }
}
