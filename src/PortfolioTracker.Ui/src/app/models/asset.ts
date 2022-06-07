import { AssetType } from '../enums/assetType';
import { Currency } from '../enums/currency';
import { RiskLevel } from '../enums/riskLevel';

export interface IAsset {
  id: string;
  accountId: string;
  userId: string;
  name: string;
  assetType: AssetType;
  currency: Currency;
  invested: number;
  riskLevel: RiskLevel;
  interestRate: number;
}
