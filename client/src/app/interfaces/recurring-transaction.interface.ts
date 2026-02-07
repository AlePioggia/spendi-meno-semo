import { TransactionType } from './transaction.interface';

export type RecurringOperationFrequency = 'Daily' | 'Weekly' | 'Monthly' | 'Yearly';

export interface TransactionTemplateUpsertRequestDto {
  description: string;
  amount: number;
  currency: string;
  transactionType: TransactionType;
  date: Date;
}

export interface RecurringOperationUpsertRequestDto {
  id?: number;
  description: string;
  frequency: RecurringOperationFrequency;
  startDate: Date;
  endDate: Date;
  categoryId: number;
  template: TransactionTemplateUpsertRequestDto;
}

export interface TransactionTemplateResponseDto {
  description: string;
  amount: number;
  currency: string;
  transactionType: TransactionType;
  date: Date;
}

export interface GetRecurringTransactionResponseDto {
  id: number;
  description: string;
  frequency: string;
  startDate: Date;
  endDate: Date;
  categoryId: number;
  createdAt: Date;
  template: TransactionTemplateResponseDto;
}
