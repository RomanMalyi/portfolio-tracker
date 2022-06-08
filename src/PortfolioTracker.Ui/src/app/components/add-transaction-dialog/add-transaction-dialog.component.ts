import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { TransactionType } from 'src/app/enums/transactionType';
import { ITransaction } from 'src/app/models/transaction';

@Component({
  selector: 'app-add-transaction-dialog',
  templateUrl: './add-transaction-dialog.component.html',
  styleUrls: ['./add-transaction-dialog.component.scss'],
})
export class AddTransactionDialogComponent implements OnInit {
  public transaction: ITransaction = {
    id: '',
    assetId: '',
    transactionType: TransactionType.Expense,
    transactionDate: new Date(),
    amount: 0,
    description: '',
    fromAssetId: null,
    toAssetId: null,
    exchangeRate: null,
  };
  public transactionTypes: string[] = [];
  constructor(public dialogRef: MatDialogRef<AddTransactionDialogComponent>) {}

  ngOnInit(): void {
    for (var enumMember in TransactionType) {
      if (isNaN(+enumMember)) this.transactionTypes.push(enumMember);
    }
  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}
