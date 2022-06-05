import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { IAccount } from 'src/app/models/account';
import { AccountService } from 'src/app/services/account.service';
import { AddAccountDialogComponent } from '../add-account-dialog/add-account-dialog.component';

export interface PeriodicElement {
  name: string;
  position: number;
  weight: number;
  symbol: string;
}

@Component({
  selector: 'app-portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.scss'],
})
export class PortfolioComponent implements OnInit {
  public accounts: IAccount[] = [];
  displayedColumns: string[] = ['name', 'accountType'];

  constructor(
    private accountService: AccountService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.accountService.getAccounts().subscribe({
      next: (data) => {
        this.accounts = data;
      },
      error: (e) => {
        console.log(e);
      },
    });
  }

  public openDialog(): void {
    let createdAccount: IAccount;
    const dialogRef = this.dialog.open(AddAccountDialogComponent, {
      width: '300px',
      data: {},
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result !== undefined) {
        this.accounts = [];
        createdAccount = result;
        this.accounts.push({
          name: createdAccount.name,
          accountType: createdAccount.accountType,
          id: '0',
        });
      }
    });
  }
}
