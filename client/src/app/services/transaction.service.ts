import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateTransactionRequestDto,
  TransactionResponseDto,
  UpdateTransactionRequestDto
} from '../interfaces/transaction.interface';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/api/transaction`;

  getTransactions(): Observable<TransactionResponseDto[]> {
    return this.http.get<TransactionResponseDto[]>(this.apiUrl);
  }

  getTransactionById(id: number): Observable<TransactionResponseDto> {
    return this.http.get<TransactionResponseDto>(`${this.apiUrl}/${id}`);
  }

  createTransaction(request: CreateTransactionRequestDto): Observable<void> {
    return this.http.post<void>(this.apiUrl, request);
  }

  updateTransaction(id: number, request: UpdateTransactionRequestDto): Observable<number> {
    return this.http.put<number>(`${this.apiUrl}/${id}`, request);
  }

  deleteTransaction(id: number): Observable<number> {
    return this.http.delete<number>(`${this.apiUrl}/${id}`);
  }
}
