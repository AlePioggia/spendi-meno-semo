export type TransactionType = 'Expense' | 'Income';

export interface CreateTransactionRequestDto {
  description?: string;
  amount: number;
  currency: string;
  transactionType: TransactionType;
  categoryId: number;
  date: string;
  isProxyTransaction?: boolean;
}

export interface UpdateTransactionRequestDto {
  id: number;
  description: string;
  amount: number;
  transactionType: TransactionType;
  currency: string;
  date: string;
  categoryId: number;
  isProxyTransaction?: boolean;
}

export interface TransactionResponseDto {
  id: number;
  description: string;
  amount: number;
  currency: string;
  expenseType: TransactionType;
  categoryId: number;
  date: Date;
  createdAt: Date;
  isProxyTransaction: boolean;
}
