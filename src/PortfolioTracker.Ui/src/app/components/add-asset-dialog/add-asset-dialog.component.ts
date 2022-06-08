import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, Validators } from '@angular/forms';
import { AssetType } from 'src/app/enums/assetType';
import { IAsset } from 'src/app/models/asset';
import { Currency } from 'src/app/enums/currency';
import { RiskLevel } from 'src/app/enums/riskLevel';

@Component({
  selector: 'app-add-asset-dialog',
  templateUrl: './add-asset-dialog.component.html',
  styleUrls: ['./add-asset-dialog.component.scss'],
})
export class AddAssetDialogComponent implements OnInit {
  generalFormGroup = this._formBuilder.group({
    nameCtrl: ['', Validators.required],
    typeCtrl: ['', Validators.required],
  });
  financeFormGroup = this._formBuilder.group({
    currencyCtrl: ['', Validators.required],
    unitsCtrl: ['', Validators.required],
    exchangeTickerCtrl: [null],
    openPriceCtrl: [null],
  });
  additionalFormGroup = this._formBuilder.group({
    riskLevelCtrl: ['', Validators.required],
    interestRateCtrl: [null],
  });

  public createdAsset: IAsset = {
    id: '',
    accountId: '',
    name: 'string;',
    assetType: AssetType.Other,
    currency: Currency.EUR,
    units: 0,
    riskLevel: RiskLevel.High,
    interestRate: null,
    exchangeTicker: null,
    openPrice: null,
  };

  public assetTypes: string[] = [];
  public currencies: string[] = [];
  public riskLevels: string[] = [];

  constructor(
    public dialogRef: MatDialogRef<AddAssetDialogComponent>,
    private _formBuilder: FormBuilder
  ) {}

  ngOnInit(): void {
    for (var enumMember in AssetType) {
      if (isNaN(+enumMember)) this.assetTypes.push(enumMember);
    }
    for (var enumMember in Currency) {
      if (isNaN(+enumMember)) this.currencies.push(enumMember);
    }
    for (var enumMember in RiskLevel) {
      if (isNaN(+enumMember)) this.riskLevels.push(enumMember);
    }
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  public addAsset(): void {
    this.createdAsset.name = this.generalFormGroup.controls['nameCtrl'].value;
    this.createdAsset.assetType =
      this.generalFormGroup.controls['typeCtrl'].value;
    this.createdAsset.currency =
      this.financeFormGroup.controls['currencyCtrl'].value;
    this.createdAsset.units = this.financeFormGroup.controls['unitsCtrl'].value;
    this.createdAsset.exchangeTicker =
      this.financeFormGroup.controls['exchangeTickerCtrl'].value;
    this.createdAsset.openPrice =
      this.financeFormGroup.controls['openPriceCtrl'].value;
    this.createdAsset.riskLevel =
      this.additionalFormGroup.controls['riskLevelCtrl'].value;
    this.createdAsset.interestRate =
      this.additionalFormGroup.controls['interestRateCtrl'].value;
  }

  public showTickerFields(): boolean {
    return (
      this.createdAsset.assetType.toString() === AssetType[AssetType.Stocks] ||
      this.createdAsset.assetType.toString() === AssetType[AssetType.Bonds] ||
      this.createdAsset.assetType.toString() === AssetType[AssetType.Funds] ||
      this.createdAsset.assetType.toString() ===
        AssetType[AssetType.Commodities] ||
      this.createdAsset.assetType.toString() ===
        AssetType[AssetType.Cryptocurrency]
    );
  }
}
