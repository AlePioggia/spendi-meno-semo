import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  signal
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

import { CategoryService } from '../../services/category.service';
import { TransactionService } from '../../services/transaction.service';
import { CategoryResponseDto } from '../../interfaces/category.interface';
import {
  CreateTransactionRequestDto,
  TransactionResponseDto
} from '../../interfaces/transaction.interface';
import {
  TransactionCreateDialog,
  TransactionCreateDialogData
} from './transaction-create.dialog';

type DayVm = {
  key: string;
  day: number;
  weekday: string;
};

@Component({
  standalone: true,
  selector: 'app-transactions-page',
  templateUrl: './transactions.page.html',
  styleUrl: './transactions.page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ]
})
export class TransactionsPage {
  private readonly transactionService = inject(TransactionService);
  private readonly categoryService = inject(CategoryService);
  private readonly dialog = inject(MatDialog);

  private readonly today = new Date();

  month = signal(new Date(this.today.getFullYear(), this.today.getMonth(), 1));
  loading = signal(false);

  tableView = signal(false);
  showOnlyDaysWithTransactions = signal(false);

  categories = signal<CategoryResponseDto[]>([]);
  allTransactions = signal<TransactionResponseDto[]>([]);

  constructor() {
    this.load();
  }

  monthLabel = computed(() => {
    const value = this.month();
    return new Intl.DateTimeFormat('it-IT', { month: 'long', year: 'numeric' }).format(value);
  });

  private monthRange = computed(() => {
    const m = this.month();
    const start = new Date(m.getFullYear(), m.getMonth(), 1);
    const end = new Date(m.getFullYear(), m.getMonth() + 1, 0);
    return { start, end };
  });

  transactionsForMonth = computed(() => {
    const { start, end } = this.monthRange();
    return this.allTransactions().filter(tx => {
      const d = this.toLocalDate(tx.date);
      return d >= start && d <= end;
    });
  });

  transactionsForMonthSorted = computed(() => {
    const list = [...this.transactionsForMonth()];
    list.sort((a, b) => {
      const byDate = this.toLocalDate(a.date).getTime() - this.toLocalDate(b.date).getTime();
      if (byDate !== 0) return byDate;
      return this.toLocalDate(a.createdAt).getTime() - this.toLocalDate(b.createdAt).getTime();
    });
    return list;
  });

  transactionsByDay = computed(() => {
    const map = new Map<string, TransactionResponseDto[]>();
    for (const tx of this.transactionsForMonth()) {
      const key = this.toDayKey(tx.date);
      const list = map.get(key) ?? [];
      list.push(tx);
      map.set(key, list);
    }

    for (const [key, list] of map.entries()) {
      list.sort((a, b) => this.toLocalDate(a.createdAt).getTime() - this.toLocalDate(b.createdAt).getTime());
      map.set(key, list);
    }

    return map;
  });

  days = computed<DayVm[]>(() => {
    const m = this.month();
    const year = m.getFullYear();
    const monthIndex = m.getMonth();

    const lastDay = new Date(year, monthIndex + 1, 0).getDate();
    const formatter = new Intl.DateTimeFormat('it-IT', { weekday: 'short' });

    const out: DayVm[] = [];
    for (let day = 1; day <= lastDay; day++) {
      const date = new Date(year, monthIndex, day);
      out.push({
        key: this.toDayKey(date),
        day,
        weekday: formatter.format(date)
      });
    }
    return out;
  });

  daysToRender = computed<DayVm[]>(() => {
    const allDays = this.days();
    if (!this.showOnlyDaysWithTransactions()) return allDays;

    const byDay = this.transactionsByDay();
    return allDays.filter(d => (byDay.get(d.key)?.length ?? 0) > 0);
  });

  monthIncome = computed(() =>
    this.transactionsForMonth()
      .filter(t => t.expenseType === 'Income')
      .reduce((sum, t) => sum + Number(t.amount), 0)
  );

  monthExpense = computed(() =>
    this.transactionsForMonth()
      .filter(t => t.expenseType === 'Expense')
      .reduce((sum, t) => sum + Number(t.amount), 0)
  );

  monthBalance = computed(() => this.monthIncome() - this.monthExpense());

  dayBalance(dayKey: string): number {
    const list = this.transactionsByDay().get(dayKey) ?? [];
    return list.reduce((sum, t) => {
      const amount = Number(t.amount);
      return t.expenseType === 'Income' ? sum + amount : sum - amount;
    }, 0);
  }

  categoryName(categoryId: number): string {
    const match = this.categories().find(c => c.id === categoryId);
    return match?.name ?? `#${categoryId}`;
  }

  prevMonth() {
    const m = this.month();
    this.month.set(new Date(m.getFullYear(), m.getMonth() - 1, 1));
  }

  nextMonth() {
    const m = this.month();
    this.month.set(new Date(m.getFullYear(), m.getMonth() + 1, 1));
  }

  openCreateDialogForMonth() {
    const initialDate = this.toDayKey(this.today);
    this.openCreateDialog(initialDate);
  }

  openCreateDialogForDay(dayKey: string) {
    this.openCreateDialog(dayKey);
  }

  private openCreateDialog(initialDate: string) {
    const data: TransactionCreateDialogData = {
      categories: this.categories(),
      initialDate
    };

    const dialogRef = this.dialog.open(TransactionCreateDialog, {
      width: '440px',
      data
    });

    dialogRef.afterClosed().subscribe((result: CreateTransactionRequestDto | undefined) => {
      if (!result) return;

      this.loading.set(true);
      this.transactionService.createTransaction(result).subscribe({
        next: () => {
          this.loadTransactions();
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    });
  }

  deleteTransaction(id: number) {
    this.loading.set(true);
    this.transactionService.deleteTransaction(id).subscribe({
      next: () => {
        this.loadTransactions();
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
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
    this.transactionService.getTransactions().subscribe({
      next: (txs) => {
        this.allTransactions.set(txs);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
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
}
