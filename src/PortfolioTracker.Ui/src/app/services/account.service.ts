import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IAccount } from '../models/account';
import { PageResult } from '../models/pageResult';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private baseUrl: string = `${environment.apiUrl}/accounts`;

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };

  constructor(private http: HttpClient) {}

  getAccounts(skip: number, take: number): Observable<PageResult<IAccount>> {
    const url = `${this.baseUrl}?skip=${skip}&take=${take}`;
    return this.http.get<PageResult<IAccount>>(url, this.httpOptions);
  }

  createAccount(account: IAccount): Observable<IAccount> {
    return this.http.post<IAccount>(this.baseUrl, account, this.httpOptions);
  }

  updateAccount(account: IAccount): Observable<IAccount> {
    return this.http.put<IAccount>(this.baseUrl, account, this.httpOptions);
  }

  deleteAccount(id: string): Observable<IAccount> {
    const url = `${this.baseUrl}/${id}`;
    return this.http.delete<IAccount>(url, this.httpOptions);
  }
}
