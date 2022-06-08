import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute } from '@angular/router';
import { TransactionType } from 'src/app/enums/transactionType';
import { IPaginationOptions } from 'src/app/models/paginationOptions';
import { ITransaction } from 'src/app/models/transaction';
import { TransactionService } from 'src/app/services/transaction.service';
import { AddTransactionDialogComponent } from '../add-transaction-dialog/add-transaction-dialog.component';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.scss'],
})
export class TransactionsComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  public paginationOptions: IPaginationOptions = {
    displayedColumns: [
      'position',
      'transactionType',
      'amount',
      'date',
      'description',
    ],
    skip: 0,
    pageSize: 10,
    totalCount: 0,
  };
  public transactions: ITransaction[] = [];
  private assetId: string = '';

  constructor(
    private transactionService: TransactionService,
    public dialog: MatDialog,
    private route: ActivatedRoute
  ) {
    this.route.params.subscribe((params) => {
      this.assetId = params['assetId'];
    });
  }

  ngOnInit(): void {
    this.loadTransactions();
  }
  public openDialog(): void {
    const dialogRef = this.dialog.open(AddTransactionDialogComponent, {
      width: '300px',
      data: {},
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result !== undefined) {
        this.transactionService
          .createTransaction(this.assetId, result)
          .subscribe({
            next: (response) => {
              this.loadTransactions();
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

    this.loadTransactions();
  }

  private loadTransactions() {
    this.transactionService
      .getTransactions(
        this.assetId,
        this.paginationOptions.skip,
        this.paginationOptions.pageSize
      )
      .subscribe({
        next: (response) => {
          this.transactions = response.data;
          this.paginationOptions.totalCount = response.totalCount;
        },
        error: (e) => {
          console.log(e);
        },
      });
  }

  public EnumToString(value: TransactionType): string {
    return TransactionType[value];
  }
}
