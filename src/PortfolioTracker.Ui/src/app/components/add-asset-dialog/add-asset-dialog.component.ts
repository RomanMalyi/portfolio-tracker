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
    investedCtrl: ['', Validators.required],
  });
  additionalFormGroup = this._formBuilder.group({
    riskLevelCtrl: ['', Validators.required],
    interestRateCtrl: ['', Validators.required],
  });

  public createdAsset: IAsset = {
    id: '',
    accountId: '',
    userId: '',
    name: 'string;',
    assetType: AssetType.Other,
    currency: Currency.EUR,
    invested: 0,
    riskLevel: RiskLevel.High,
    interestRate: 0,
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
    this.createdAsset.invested =
      this.financeFormGroup.controls['investedCtrl'].value;
    this.createdAsset.riskLevel =
      this.additionalFormGroup.controls['riskLevelCtrl'].value;
    this.createdAsset.interestRate =
      this.additionalFormGroup.controls['interestRateCtrl'].value;
  }
}
