import { TransactionType } from '../enums/transactionType';

export interface ITransaction {
  id: string;
  assetId: string;
  transactionType: TransactionType;
  transactionDate: Date;
  amount: number;
  fromAssetId: string | null;
  toAssetId: string | null;
  exchangeRate: number | null;
  description: string;
}
