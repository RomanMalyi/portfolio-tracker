import { Currency } from '../enums/currency';
import { RiskLevel } from '../enums/riskLevel';

export interface IAsset {
  id: string;
  accountId: string;
  userId: string;
  name: string;
  currency: Currency;
  riskLevel: RiskLevel;
  invested: number;
  interestRate: number;
}
