import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  GetRecurringTransactionResponseDto,
  RecurringOperationUpsertRequestDto
} from '../interfaces/recurring-transaction.interface';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RecurringTransactionService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/api/recurringTransaction`;

  getRecurringTransactions(): Observable<GetRecurringTransactionResponseDto[]> {
    return this.http.get<GetRecurringTransactionResponseDto[]>(this.apiUrl);
  }

  getRecurringTransactionById(id: number): Observable<GetRecurringTransactionResponseDto> {
    return this.http.get<GetRecurringTransactionResponseDto>(`${this.apiUrl}/${id}`);
  }

  createRecurringTransaction(request: RecurringOperationUpsertRequestDto): Observable<void> {
    return this.http.post<void>(this.apiUrl, request);
  }

  updateRecurringTransaction(id: number, request: RecurringOperationUpsertRequestDto): Observable<number> {
    return this.http.put<number>(`${this.apiUrl}/${id}`, request);
  }

  deleteRecurringTransaction(id: number): Observable<number> {
    return this.http.delete<number>(`${this.apiUrl}/${id}`);
  }
}
