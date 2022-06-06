import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute } from '@angular/router';
import { Currency } from 'src/app/enums/currency';
import { RiskLevel } from 'src/app/enums/riskLevel';
import { IAsset } from 'src/app/models/asset';
import { IPaginationOptions } from 'src/app/models/paginationOptions';
import { AssetService } from 'src/app/services/asset.service';
import { AddAssetDialogComponent } from '../add-asset-dialog/add-asset-dialog.component';

@Component({
  selector: 'app-assets',
  templateUrl: './assets.component.html',
  styleUrls: ['./assets.component.scss'],
})
export class AssetsComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  public paginationOptions: IPaginationOptions = {
    displayedColumns: [
      'position',
      'name',
      'currency',
      'riskLevel',
      'invested',
      'interestRate',
    ],
    skip: 0,
    pageSize: 10,
    totalCount: 0,
  };
  public assets: IAsset[] = [];
  private accountId: string = '';
  constructor(
    public dialog: MatDialog,
    private assetService: AssetService,
    private route: ActivatedRoute
  ) {
    this.route.params.subscribe((params) => {
      this.accountId = params['accountId'];
    });
  }

  ngOnInit(): void {
    this.loadAssets();
  }

  public openDialog(): void {
    const dialogRef = this.dialog.open(AddAssetDialogComponent, {
      width: '300px',
      data: {},
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result !== undefined) {
        this.assetService.createAsset(this.accountId, result).subscribe({
          next: (response) => {
            this.loadAssets();
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

    this.loadAssets();
  }

  private loadAssets() {
    this.assetService
      .getAssets(
        this.accountId,
        this.paginationOptions.skip,
        this.paginationOptions.pageSize
      )
      .subscribe({
        next: (response) => {
          this.assets = response.data;
          this.paginationOptions.totalCount = response.totalCount;
        },
        error: (e) => {
          console.log(e);
        },
      });
  }

  public RiskLevelEnumToString(value: RiskLevel): string {
    return RiskLevel[value];
  }

  public CurrencyEnumToString(value: Currency): string {
    return Currency[value];
  }
}
