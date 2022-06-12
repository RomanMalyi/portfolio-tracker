export interface CurrencyAnalytic {
  currency: number;
  portfolioAmount: number;
  portfolioPercent: number;
}

export interface AccountAnalytic {
  accountId: string;
  accountName: string;
  accountType: number;
  portfolioAmount: number;
  portfolioPercent: number;
}

export interface AssetTypeAnalytic {
  assetType: number;
  portfolioAmount: number;
  portfolioPercent: number;
}

export interface RiskLevelAnalytic {
  riskLevel: number;
  portfolioAmount: number;
  portfolioPercent: number;
}

export interface Snapshot {
  id: string;
  userId: string;
  generationTime: Date;
  totalAmount: number;
  currencyAnalytics: CurrencyAnalytic[];
  accountAnalytics: AccountAnalytic[];
  assetTypeAnalytics: AssetTypeAnalytic[];
  riskLevelAnalytics: RiskLevelAnalytic[];
  transactionTypeAnalytics?: any;
}

export interface AnalyticsResponse {
  mostPopularCurrency: string;
  highestLevelOfRisk: string;
  numberOfAsstTypes: number;
  numberOfAccounts: number;
  currentTotalAmount: number;
  portfolioChange: number;
  snapshots: Snapshot[];
}
