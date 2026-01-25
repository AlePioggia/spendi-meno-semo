import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

import { InputFieldComponent } from '../../shared/input-field/input-field/input-field.component';
import { CategoryResponseDto } from '../../interfaces/category.interface';
import { CreateTransactionRequestDto, TransactionType } from '../../interfaces/transaction.interface';

export interface TransactionCreateDialogData {
  categories: CategoryResponseDto[];
  initialDate: string; // YYYY-MM-DD
}

@Component({
  standalone: true,
  selector: 'app-transaction-create-dialog',
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    InputFieldComponent
  ],
  template: `
    <h2 mat-dialog-title>Nuova transazione</h2>

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

      <app-input-field
        label="Data"
        type="date"
        [value]="date"
        (valueChange)="date.set($event)"
      />

      <mat-form-field appearance="fill" class="full-width">
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
        [disabled]="!canCreate()"
        (click)="confirm()"
      >
        Aggiungi
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

  description = signal('');
  amount = signal('');
  date = signal(this.data.initialDate);
  transactionType = signal<TransactionType>('Expense');
  categoryId = signal<number>(this.data.categories[0]?.id ?? 0);

  canCreate = computed(() => {
    const parsedAmount = Number(this.amount());
    return (
      this.date().trim().length === 10 &&
      Number.isFinite(parsedAmount) &&
      parsedAmount > 0 &&
      this.categoryId() > 0
    );
  });

  confirm() {
    const request: CreateTransactionRequestDto = {
      description: this.description().trim() || undefined,
      amount: Number(this.amount()),
      currency: 'EUR',
      transactionType: this.transactionType(),
      categoryId: this.categoryId(),
      date: this.date()
    };

    this.dialogRef.close(request);
  }
}
