import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';

import { CategoryResponseDto } from '../../interfaces/category.interface';
import {
  GetRecurringTransactionResponseDto,
  RecurringOperationFrequency,
  RecurringOperationUpsertRequestDto
} from '../../interfaces/recurring-transaction.interface';
import { TransactionType } from '../../interfaces/transaction.interface';
import { InputFieldComponent } from '../../shared/input-field/input-field/input-field.component';

export type RecurringTransactionDialogMode = 'create' | 'edit';

export type RecurringTransactionDialogResult =
  | { mode: 'create'; request: RecurringOperationUpsertRequestDto }
  | { mode: 'edit'; id: number; request: RecurringOperationUpsertRequestDto };

export interface RecurringTransactionUpsertDialogData {
  categories: CategoryResponseDto[];
  mode?: RecurringTransactionDialogMode;
  recurring?: GetRecurringTransactionResponseDto;
}

@Component({
  standalone: true,
  selector: 'app-recurring-transaction-upsert-dialog',
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
    <h2 mat-dialog-title>
      <mat-icon class="title-icon">repeat</mat-icon>
      {{ title() }}
    </h2>

    <mat-dialog-content>
      <div class="section-title">
        <mat-icon>event_repeat</mat-icon>
        <span>Ricorrenza</span>
      </div>

      <app-input-field
        label="Descrizione ricorrenza"
        [value]="description"
        (valueChange)="description.set($event)"
      />

      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Frequenza</mat-label>
        <mat-select [value]="frequency()" (selectionChange)="frequency.set($event.value)">
          <mat-option value="Daily">Giornaliera</mat-option>
          <mat-option value="Weekly">Settimanale</mat-option>
          <mat-option value="Monthly">Mensile</mat-option>
          <mat-option value="Yearly">Annuale</mat-option>
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Categoria</mat-label>
        <mat-select [value]="categoryId()" (selectionChange)="categoryId.set($event.value)">
          <mat-option *ngFor="let c of categories" [value]="c.id">{{ c.name }}</mat-option>
        </mat-select>
      </mat-form-field>

      <div class="dates-row">
        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Inizio</mat-label>
          <input
            matInput
            [matDatepicker]="startPicker"
            [value]="startDate()"
            (dateChange)="startDate.set($event.value ?? startDate())"
          />
          <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
          <mat-datepicker #startPicker></mat-datepicker>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Fine</mat-label>
          <input
            matInput
            [matDatepicker]="endPicker"
            [value]="endDate()"
            (dateChange)="endDate.set($event.value ?? endDate())"
          />
          <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
          <mat-datepicker #endPicker></mat-datepicker>
        </mat-form-field>
      </div>

      <div class="section-title spaced">
        <mat-icon>receipt_long</mat-icon>
        <span>Template transazione</span>
      </div>

      <app-input-field
        label="Descrizione"
        [value]="templateDescription"
        (valueChange)="templateDescription.set($event)"
      />

      <app-input-field
        label="Importo"
        type="number"
        [value]="templateAmount"
        (valueChange)="templateAmount.set($event)"
      />

      <div class="template-row">
        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Tipo</mat-label>
          <mat-select [value]="templateTransactionType()" (selectionChange)="templateTransactionType.set($event.value)">
            <mat-option value="Expense">Spesa</mat-option>
            <mat-option value="Income">Entrata</mat-option>
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Valuta</mat-label>
          <mat-select [value]="templateCurrency()" (selectionChange)="templateCurrency.set($event.value)">
            <mat-option value="EUR">EUR</mat-option>
            <mat-option value="USD">USD</mat-option>
            <mat-option value="GBP">GBP</mat-option>
            <mat-option value="JPY">JPY</mat-option>
            <mat-option value="AUD">AUD</mat-option>
            <mat-option value="CAD">CAD</mat-option>
            <mat-option value="CHF">CHF</mat-option>
            <mat-option value="CNY">CNY</mat-option>
            <mat-option value="SEK">SEK</mat-option>
            <mat-option value="NZD">NZD</mat-option>
          </mat-select>
        </mat-form-field>
      </div>

      <div class="template-date-wrap">
        <div *ngIf="frequency() === 'Daily'" class="hint subtle">
          <mat-icon>info</mat-icon>
          <span>Per ricorrenza giornaliera la data del template non è necessaria.</span>
        </div>

        <mat-form-field *ngIf="frequency() === 'Weekly'" appearance="fill" class="full-width">
          <mat-label>Giorno della settimana</mat-label>
          <mat-select [value]="templateWeekday()" (selectionChange)="templateWeekday.set($event.value)">
            <mat-option value="Monday">Lunedì</mat-option>
            <mat-option value="Tuesday">Martedì</mat-option>
            <mat-option value="Wednesday">Mercoledì</mat-option>
            <mat-option value="Thursday">Giovedì</mat-option>
            <mat-option value="Friday">Venerdì</mat-option>
            <mat-option value="Saturday">Sabato</mat-option>
            <mat-option value="Sunday">Domenica</mat-option>
          </mat-select>
        </mat-form-field>

        <app-input-field
          *ngIf="frequency() === 'Monthly'"
          label="Giorno del mese"
          type="number"
          [value]="templateDayOfMonth"
          (valueChange)="templateDayOfMonth.set($event)"
        />

        <mat-form-field *ngIf="frequency() === 'Yearly'" appearance="fill" class="full-width">
          <mat-label>Data template</mat-label>
          <input
            matInput
            [matDatepicker]="tplPicker"
            [value]="templateDate()"
            (dateChange)="templateDate.set($event.value ?? templateDate())"
          />
          <mat-datepicker-toggle matSuffix [for]="tplPicker"></mat-datepicker-toggle>
          <mat-datepicker #tplPicker></mat-datepicker>
        </mat-form-field>
      </div>

      <div class="hint" *ngIf="dateRangeInvalid()">
        <mat-icon>warning</mat-icon>
        <span>La data di fine deve essere maggiore o uguale alla data di inizio.</span>
      </div>
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
    mat-dialog-content {
      display: flex;
      flex-direction: column;
      gap: 12px;
      min-width: 360px;
    }

    .title-icon {
      margin-right: 8px;
      vertical-align: -4px;
    }

    .full-width {
      width: 100%;
    }

    .dates-row,
    .template-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 12px;
    }

    .template-date-wrap {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .section-title {
      display: flex;
      align-items: center;
      gap: 8px;
      font-weight: 800;
      color: #111827;
      padding-top: 4px;
    }

    .section-title.spaced {
      margin-top: 6px;
    }

    .section-title mat-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
      opacity: 0.85;
    }

    .hint {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 10px 12px;
      border-radius: 12px;
      background: #f8fafc;
      border: 1px dashed #d1d5db;
      color: #6b7280;
      font-size: 13px;
    }

    .hint mat-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
    }

    .hint.subtle {
      border-style: solid;
      opacity: 0.95;
    }

    @media (max-width: 520px) {
      mat-dialog-content {
        min-width: 300px;
      }

      .dates-row,
      .template-row {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class RecurringTransactionUpsertDialog {
  private dialogRef = inject(MatDialogRef<RecurringTransactionUpsertDialog>);
  private data = inject<RecurringTransactionUpsertDialogData>(MAT_DIALOG_DATA);

  categories = this.data.categories;

  private readonly mode: RecurringTransactionDialogMode = this.data.mode ?? 'create';
  private readonly existing = this.data.recurring;

  title = computed(() => (this.mode === 'edit' ? 'Modifica ricorrenza' : 'Nuova spesa ricorrente'));
  confirmLabel = computed(() => (this.mode === 'edit' ? 'Salva' : 'Crea'));

  description = signal(this.existing?.description ?? '');
  frequency = signal<RecurringOperationFrequency>(this.toFrequency(this.existing?.frequency) ?? 'Monthly');
  startDate = signal(this.toLocalDate(this.existing?.startDate ?? new Date()));
  endDate = signal(this.toLocalDate(this.existing?.endDate ?? new Date()));
  categoryId = signal<number>(this.existing?.categoryId ?? (this.data.categories[0]?.id ?? 0));

  templateDescription = signal(this.existing?.template?.description ?? '');
  templateAmount = signal(this.existing ? String(this.existing.template?.amount ?? '') : '');
  templateCurrency = signal(this.existing?.template?.currency ?? 'EUR');
  templateTransactionType = signal<TransactionType>(
    this.toTransactionType(this.existing?.template?.transactionType) ?? 'Expense'
  );
  templateDate = signal(this.toLocalDate(this.existing?.template?.date ?? new Date()));
  templateWeekday = signal<
    'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday' | 'Sunday'
  >(this.toWeekday(this.existing?.template?.date) ?? 'Monday');
  templateDayOfMonth = signal(
    this.existing ? String(this.toLocalDate(this.existing.template?.date).getDate()) : '1'
  );

  dateRangeInvalid = computed(() => {
    const start = this.startDate();
    const end = this.endDate();
    return this.isValidDate(start) && this.isValidDate(end) && end.getTime() < start.getTime();
  });

  canConfirm = computed(() => {
    const parsedAmount = Number(this.templateAmount());

    const freq = this.frequency();
    const needsYearlyDate = freq === 'Yearly';
    const needsWeeklyDay = freq === 'Weekly';
    const needsMonthlyDay = freq === 'Monthly';

    const domStr = this.templateDayOfMonth().trim();
    const dom = domStr.length ? Number(domStr) : NaN;
    const domValid = Number.isFinite(dom) && dom >= 1 && dom <= 31;

    return (
      this.description().trim().length > 0 &&
      this.categoryId() > 0 &&
      this.templateDescription().trim().length > 0 &&
      Number.isFinite(parsedAmount) &&
      parsedAmount > 0 &&
      this.templateCurrency().trim().length > 0 &&
      this.isValidDate(this.startDate()) &&
      this.isValidDate(this.endDate()) &&
      !this.dateRangeInvalid() &&
      (!needsYearlyDate || this.isValidDate(this.templateDate())) &&
      (!needsWeeklyDay || !!this.templateWeekday()) &&
      (!needsMonthlyDay || domValid)
    );
  });

  confirm() {
    const templateDate = this.buildTemplateDate();

    const request: RecurringOperationUpsertRequestDto = {
      id: this.mode === 'edit' ? this.existing?.id : undefined,
      description: this.description().trim(),
      frequency: this.frequency(),
      startDate: this.startDate(),
      endDate: this.endDate(),
      categoryId: this.categoryId(),
      template: {
        description: this.templateDescription().trim(),
        amount: Number(this.templateAmount()),
        currency: this.templateCurrency(),
        transactionType: this.templateTransactionType(),
        date: templateDate
      }
    };

    if (this.mode === 'edit' && this.existing) {
      const result: RecurringTransactionDialogResult = { mode: 'edit', id: this.existing.id, request };
      this.dialogRef.close(result);
      return;
    }

    const result: RecurringTransactionDialogResult = { mode: 'create', request };
    this.dialogRef.close(result);
  }

  private isValidDate(value: unknown): value is Date {
    return value instanceof Date && Number.isFinite(value.getTime());
  }

  private toFrequency(value: unknown): RecurringOperationFrequency | null {
    if (value === 'Daily' || value === 'Weekly' || value === 'Monthly' || value === 'Yearly') return value;
    return null;
  }

  private toWeekday(value: unknown):
    | 'Monday'
    | 'Tuesday'
    | 'Wednesday'
    | 'Thursday'
    | 'Friday'
    | 'Saturday'
    | 'Sunday'
    | null {
    const d = this.toLocalDate(value);
    if (!this.isValidDate(d)) return null;

    switch (d.getDay()) {
      case 0:
        return 'Sunday';
      case 1:
        return 'Monday';
      case 2:
        return 'Tuesday';
      case 3:
        return 'Wednesday';
      case 4:
        return 'Thursday';
      case 5:
        return 'Friday';
      case 6:
        return 'Saturday';
      default:
        return null;
    }
  }

  private weekdayToIndex(value: string): number {
    switch (value) {
      case 'Sunday':
        return 0;
      case 'Monday':
        return 1;
      case 'Tuesday':
        return 2;
      case 'Wednesday':
        return 3;
      case 'Thursday':
        return 4;
      case 'Friday':
        return 5;
      case 'Saturday':
        return 6;
      default:
        return 1;
    }
  }

  private buildTemplateDate(): Date {
    const freq = this.frequency();
    const start = this.toLocalDate(this.startDate());

    if (freq === 'Daily') {
      return new Date(start.getFullYear(), start.getMonth(), start.getDate());
    }

    if (freq === 'Weekly') {
      const desired = this.weekdayToIndex(this.templateWeekday());
      const base = new Date(start.getFullYear(), start.getMonth(), start.getDate());
      const current = base.getDay();
      const diff = (desired - current + 7) % 7;
      base.setDate(base.getDate() + diff);
      return base;
    }

    if (freq === 'Monthly') {
      const domStr = this.templateDayOfMonth().trim();
      const dom = domStr.length ? Number(domStr) : 1;
      const safeDay = Math.min(Math.max(1, dom), 31);

      return new Date(Date.UTC(2000, 0, safeDay, 12, 0, 0));
    }

    return this.toLocalDate(this.templateDate());
  }

  private toTransactionType(value: unknown): TransactionType | null {
    if (value === 'Expense' || value === 'Income') return value;
    return null;
  }

  private toLocalDate(value: unknown): Date {
    if (value instanceof Date) return value;

    if (typeof value === 'string') {
      const iso = Date.parse(value);
      if (Number.isFinite(iso)) return new Date(value);

      const match = value.match(/^\d{4}-\d{2}-\d{2}/);
      if (match) {
        const y = Number(value.slice(0, 4));
        const m = Number(value.slice(5, 7)) - 1;
        const d = Number(value.slice(8, 10));
        return new Date(y, m, d);
      }
    }

    return new Date();
  }
}
