import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { AccountType } from 'src/app/enums/accountType';
import { IAccount } from 'src/app/models/account';
import { IPaginationOptions } from 'src/app/models/paginationOptions';
import { AccountService } from 'src/app/services/account.service';
import { AddAccountDialogComponent } from '../add-account-dialog/add-account-dialog.component';

@Component({
  selector: 'app-portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.scss'],
})
export class PortfolioComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  public paginationOptions: IPaginationOptions = {
    displayedColumns: ['position', 'name', 'accountType'],
    skip: 0,
    pageSize: 10,
    totalCount: 0,
  };
  public accounts: IAccount[] = [];

  constructor(
    private accountService: AccountService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadAccounts();
  }

  public openDialog(): void {
    const dialogRef = this.dialog.open(AddAccountDialogComponent, {
      width: '300px',
      data: {},
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result !== undefined) {
        this.accountService.createAccount(result).subscribe({
          next: (response) => {
            this.loadAccounts();
          },
          error: (e) => {
            console.log(e);
          },
        });
      }
    });
  }

  public getPagingData(event: any) {
    this.paginationOptions.pageSize = event.pageSize;
    this.paginationOptions.skip = event.pageIndex * event.pageSize;

    this.loadAccounts();
  }

  private loadAccounts() {
    this.accountService
      .getAccounts(this.paginationOptions.skip, this.paginationOptions.pageSize)
      .subscribe({
        next: (response) => {
          this.accounts = response.data;
          this.paginationOptions.totalCount = response.totalCount;
        },
        error: (e) => {
          console.log(e);
        },
      });
  }

  public EnumToString(value: AccountType): string {
    return AccountType[value];
  }
}
