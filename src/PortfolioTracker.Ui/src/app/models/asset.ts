import { AssetType } from '../enums/assetType';
import { Currency } from '../enums/currency';
import { RiskLevel } from '../enums/riskLevel';

export interface IAsset {
  id: string;
  accountId: string;
  name: string;
  assetType: AssetType;
  exchangeTicker: string | null;
  openPrice: number | null;
  interestRate: number | null;
  currency: Currency;
  units: number;
  riskLevel: RiskLevel;
}
