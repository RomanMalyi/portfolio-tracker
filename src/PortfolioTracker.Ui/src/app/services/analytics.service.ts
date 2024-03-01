import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AnalyticsResponse } from '../models/analyticsResponse';
import { MarketValue } from '../models/marketValue';

@Injectable({
  providedIn: 'root',
})
export class AnalyticsService {
  public analyticsInfo: AnalyticsResponse = {
    mostPopularCurrency: '',
    highestLevelOfRisk: '',
    numberOfAsstTypes: 0,
    numberOfAccounts: 0,
    currentTotalAmount: 0,
    snapshots: [],
    portfolioChange: 0,
  };
  public marketInfo: MarketValue[] = [];
  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };

  constructor(private http: HttpClient) {}

  public getAnalyticsInfo(): Observable<AnalyticsResponse> {
    const url = `https://localhost:44333/api/Analytics`;
    return this.http.get<AnalyticsResponse>(url, this.httpOptions);
  }

  getMarketValue(): Observable<MarketValue[]> {
    const url = `https://localhost:7259/api/Market/tickers?tickers=AAPL&tickers=MSFT&tickers=AMZN&tickers=TSLA&tickers=GOOGL&tickers=NVDA&tickers=BRK.B&tickers=META&tickers=UNH&tickers=VOO&tickers=XLE&tickers=BABA`;
    return this.http.get<MarketValue[]>(url, this.httpOptions);
  }
}
