import { AccountType } from '../enums/accountType';

export interface IAccount {
  id: string;
  name: string;
  accountType: AccountType;
}
