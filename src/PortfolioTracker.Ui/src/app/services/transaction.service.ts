import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PageResult } from '../models/pageResult';
import { ITransaction } from '../models/transaction';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };

  constructor(private http: HttpClient) {}

  getTransactions(
    assetId: string,
    skip: number,
    take: number
  ): Observable<PageResult<ITransaction>> {
    const url = `${environment.apiUrl}/assets/${assetId}/transactions?skip=${skip}&take=${take}`;
    return this.http.get<PageResult<ITransaction>>(url, this.httpOptions);
  }

  createTransaction(
    assetId: string,
    transaction: ITransaction
  ): Observable<ITransaction> {
    return this.http.post<ITransaction>(
      `${environment.apiUrl}/assets/${assetId}/transactions`,
      transaction,
      this.httpOptions
    );
  }

  updateTransaction(
    assetId: string,
    transaction: ITransaction
  ): Observable<ITransaction> {
    return this.http.put<ITransaction>(
      `${environment.apiUrl}/assets/${assetId}/transactions`,
      transaction,
      this.httpOptions
    );
  }

  deleteTransaction(
    id: string,
    transactionId: string
  ): Observable<ITransaction> {
    const url = `${environment.apiUrl}/assets/${transactionId}/transactions/${id}`;
    return this.http.delete<ITransaction>(url, this.httpOptions);
  }
}
