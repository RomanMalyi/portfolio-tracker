import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from 'src/environments/environment';
import { IAsset } from '../models/asset';
import { PageResult } from '../models/pageResult';

@Injectable({
  providedIn: 'root',
})
export class AssetService {
  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };

  constructor(private http: HttpClient) {}

  getAssets(
    accountId: string,
    skip: number,
    take: number
  ): Observable<PageResult<IAsset>> {
    const url = `${environment.apiUrl}/accounts/${accountId}/assets?skip=${skip}&take=${take}`;
    return this.http.get<PageResult<IAsset>>(url, this.httpOptions);
  }

  createAsset(accountId: string, asset: IAsset): Observable<IAsset> {
    return this.http.post<IAsset>(
      `${environment.apiUrl}/accounts/${accountId}/assets`,
      asset,
      this.httpOptions
    );
  }

  updateAsset(accountId: string, asset: IAsset): Observable<IAsset> {
    return this.http.put<IAsset>(
      `${environment.apiUrl}/accounts/${accountId}/assets`,
      asset,
      this.httpOptions
    );
  }

  deleteAsset(id: string, accountId: string): Observable<IAsset> {
    const url = `${environment.apiUrl}/accounts/${accountId}/assets/${id}`;
    return this.http.delete<IAsset>(url, this.httpOptions);
  }
}
