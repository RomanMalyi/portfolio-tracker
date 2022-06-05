import { Currency } from "../enums/currency";
import { RiskLevel } from "../enums/riskLevel";

export interface IAsset {
    id: string;
    name: string;
    balance: number;
    currency: Currency;
    riskLevel: RiskLevel;
    interestRate: number;
}