import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { AccountType } from 'src/app/enums/accountType';
import { IAccount } from 'src/app/models/account';

@Component({
  selector: 'app-add-account-dialog',
  templateUrl: './add-account-dialog.component.html',
  styleUrls: ['./add-account-dialog.component.scss'],
})
export class AddAccountDialogComponent implements OnInit {
  public account: IAccount = {
    id: '',
    name: '',
    accountType: AccountType.Other,
  };
  public categories: string[] = [];
  constructor(public dialogRef: MatDialogRef<AddAccountDialogComponent>) {}

  ngOnInit(): void {
    for (var enumMember in AccountType) {
      if (isNaN(+enumMember)) this.categories.push(enumMember);
    }
  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}
