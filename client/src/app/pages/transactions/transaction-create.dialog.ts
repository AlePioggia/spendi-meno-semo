import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { InputFieldComponent } from '../../shared/input-field/input-field/input-field.component';
import { CategoryResponseDto } from '../../interfaces/category.interface';
import {
  CreateTransactionRequestDto,
  TransactionResponseDto,
  TransactionType,
  UpdateTransactionRequestDto
} from '../../interfaces/transaction.interface';

export type TransactionDialogMode = 'create' | 'edit';

export type TransactionDialogResult =
  | { mode: 'create'; request: CreateTransactionRequestDto }
  | { mode: 'edit'; id: number; request: UpdateTransactionRequestDto };

export interface TransactionCreateDialogData {
  categories: CategoryResponseDto[];
  initialDate: string;
  mode?: TransactionDialogMode;
  transaction?: TransactionResponseDto;
  isProxyTransaction?: boolean;
}

@Component({
  standalone: true,
  selector: 'app-transaction-create-dialog',
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatNativeDateModule,
    MatSelectModule,
    InputFieldComponent
  ],
  template: `
    <h2 mat-dialog-title>{{ title() }}</h2>

    <mat-dialog-content>
      <app-input-field
        label="Descrizione"
        [value]="description"
        (valueChange)="description.set($event)"
      />

      <app-input-field
        label="Importo"
        type="number"
        [value]="amount"
        (valueChange)="amount.set($event)"
      />

      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Data</mat-label>
        <input
          matInput
          [matDatepicker]="picker"
          [value]="date()"
          [min]="minDate()"
          (dateChange)="date.set($event.value ?? date())"
        />
        <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
        <mat-datepicker #picker></mat-datepicker>
      </mat-form-field>

      <mat-form-field appearance="fill" class="full-width" *ngIf="!isProxyTransaction">
        <mat-label>Tipo</mat-label>
        <mat-select [value]="transactionType()" (selectionChange)="transactionType.set($event.value)">
          <mat-option value="Expense">Spesa</mat-option>
          <mat-option value="Income">Entrata</mat-option>
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Categoria</mat-label>
        <mat-select [value]="categoryId()" (selectionChange)="categoryId.set($event.value)">
          <mat-option *ngFor="let c of categories" [value]="c.id">
            {{ c.name }}
          </mat-option>
        </mat-select>
      </mat-form-field>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Annulla</button>

      <button
        mat-raised-button
        color="primary"
        [disabled]="!canConfirm()"
        (click)="confirm()"
      >
        {{ confirmLabel() }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .full-width {
      width: 100%;
    }

    mat-dialog-content {
      display: flex;
      flex-direction: column;
      gap: 12px;
      min-width: 320px;
    }
  `]
})
export class TransactionCreateDialog {
  private dialogRef = inject(MatDialogRef<TransactionCreateDialog>);
  private data = inject<TransactionCreateDialogData>(MAT_DIALOG_DATA);

  categories = this.data.categories;
  isProxyTransaction = this.data.isProxyTransaction ?? false;

  private readonly mode: TransactionDialogMode = this.data.mode ?? 'create';
  private readonly existing = this.data.transaction;
  private readonly today = new Date();

  title = computed(() => (this.mode === 'edit' ? 'Modifica transazione' : 'Nuova transazione'));
  confirmLabel = computed(() => (this.mode === 'edit' ? 'Salva' : 'Aggiungi'));

  minDate = computed(() => {
    const d = new Date(this.today);
    d.setHours(0, 0, 0, 0);
    return d;
  });

  description = signal(this.existing?.description ?? '');
  amount = signal(this.existing ? String(this.existing.amount) : '');
  date = signal(this.toLocalDate(this.existing?.date ?? this.data.initialDate));
  transactionType = signal<TransactionType>(this.existing?.expenseType ?? (this.isProxyTransaction ? 'Expense' : 'Expense'));
  categoryId = signal<number>(this.existing?.categoryId ?? (this.data.categories[0]?.id ?? 0));

  canConfirm = computed(() => {
    const parsedAmount = Number(this.amount());
    const d = this.date();
    return (
      d instanceof Date &&
      Number.isFinite(d.getTime()) &&
      Number.isFinite(parsedAmount) &&
      parsedAmount > 0 &&
      this.categoryId() > 0
    );
  });

  confirm() {
    const dateKey = this.toDayKey(this.date());

    if (this.mode === 'edit' && this.existing) {
      const request: UpdateTransactionRequestDto = {
        id: this.existing.id,
        description: this.description().trim(),
        amount: Number(this.amount()),
        currency: 'EUR',
        transactionType: this.transactionType(),
        categoryId: this.categoryId(),
        date: dateKey,
        ...(this.isProxyTransaction && { isProxyTransaction: true })
      };
      const result: TransactionDialogResult = { mode: 'edit', id: this.existing.id, request };
      this.dialogRef.close(result);
      return;
    }

    const request: CreateTransactionRequestDto = {
      description: this.description().trim() || undefined,
      amount: Number(this.amount()),
      currency: 'EUR',
      transactionType: this.isProxyTransaction ? 'Expense' : this.transactionType(),
      categoryId: this.categoryId(),
      date: dateKey,
      ...(this.isProxyTransaction && { isProxyTransaction: true })
    };

    const result: TransactionDialogResult = { mode: 'create', request };
    this.dialogRef.close(result);
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
