import { Currency } from "../enums/currency";
import { RiskLevel } from "../enums/riskLevel";

export interface IAccount {
    id: string;
    name: string;
    balance: number;
    accountType: AccountType;
    currency: Currency;
    riskLevel: RiskLevel;
    interestRate: number;
}

export enum AccountType {
    CreditCard = 1,
    Cash = 2,
    CrypoWallet = 3,
    Deposit = 4,
    Loan = 5,
    Broker = 6
  }