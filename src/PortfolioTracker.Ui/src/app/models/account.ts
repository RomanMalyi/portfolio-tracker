import { AccountType } from '../enums/accountType';

export interface IAccount {
  id: string;
  userId: string;
  name: string;
  accountType: AccountType;
}
