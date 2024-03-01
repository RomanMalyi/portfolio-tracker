import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AssetType } from 'src/app/enums/assetType';
import { Currency } from 'src/app/enums/currency';
import { AnalyticsService } from 'src/app/services/analytics.service';

@Component({
  selector: 'app-analytics',
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.scss'],
})
export class AnalyticsComponent implements OnInit {
  optionsCurrencies: any;
  optionsAccounts: any;
  optionsAssets: any;
  optionsCurrenciesTime: any;
  constructor(
    public analyticsService: AnalyticsService,
    private datePipe: DatePipe
  ) {}

  ngOnInit(): void {
    this.analyticsService.getAnalyticsInfo().subscribe({
      next: (response) => {
        this.analyticsService.analyticsInfo = response;
        this.generateCurrencies();
        this.generateAccounts();
        this.generateAssets();
        this.generateCurrenciesOverTime();
      },
      error: (e) => {
        console.log(e);
      },
    });
  }

  generateCurrencies() {
    var currenciesData: any[] = [];
    var lastSnapshot =
      this.analyticsService.analyticsInfo.snapshots[
        this.analyticsService.analyticsInfo.snapshots.length - 1
      ];
    var currencies = lastSnapshot.currencyAnalytics;

    for (let i = 0; i < currencies.length; i++) {
      currenciesData.push({
        value: currencies[i].portfolioPercent.toFixed(2),
        name: this.CurrencyEnumToString(currencies[i].currency),
      });
    }
    this.optionsCurrencies = {
      title: {
        left: 'center',
        text: 'Currencies',
      },
      tooltip: {
        trigger: 'item',
      },
      legend: {
        top: '5%',
        left: 'center',
      },
      series: [
        {
          name: 'Currencies',
          type: 'pie',
          radius: ['40%', '70%'],
          avoidLabelOverlap: false,
          itemStyle: {
            borderRadius: 10,
            borderColor: '#fff',
            borderWidth: 2,
          },
          label: {
            show: false,
            position: 'center',
          },
          emphasis: {
            label: {
              show: true,
              fontSize: '40',
              fontWeight: 'bold',
            },
          },
          labelLine: {
            show: false,
          },
          data: currenciesData,
        },
      ],
    };
  }

  generateAccounts() {
    var accountsData: any[] = [];
    var lastSnapshot =
      this.analyticsService.analyticsInfo.snapshots[
        this.analyticsService.analyticsInfo.snapshots.length - 1
      ];
    var accounts = lastSnapshot.accountAnalytics;

    for (let i = 0; i < accounts.length; i++) {
      accountsData.push({
        value: accounts[i].portfolioPercent.toFixed(2),
        name: accounts[i].accountName,
      });
    }
    this.optionsAccounts = {
      title: {
        left: 'center',
        text: 'Accounts',
      },
      tooltip: {
        trigger: 'item',
      },
      legend: {
        top: '5%',
        left: 'center',
      },
      series: [
        {
          name: 'Accounts',
          type: 'pie',
          radius: ['40%', '70%'],
          avoidLabelOverlap: false,
          itemStyle: {
            borderRadius: 10,
            borderColor: '#fff',
            borderWidth: 2,
          },
          label: {
            show: false,
            position: 'center',
          },
          emphasis: {
            label: {
              show: true,
              fontSize: '40',
              fontWeight: 'bold',
            },
          },
          labelLine: {
            show: false,
          },
          data: accountsData,
        },
      ],
    };
  }

  generateAssets() {
    var assetsData: any[] = [];
    var lastSnapshot =
      this.analyticsService.analyticsInfo.snapshots[
        this.analyticsService.analyticsInfo.snapshots.length - 1
      ];
    var accounts = lastSnapshot.assetTypeAnalytics;

    for (let i = 0; i < accounts.length; i++) {
      assetsData.push({
        value: accounts[i].portfolioPercent.toFixed(2),
        name: this.AssetTypeEnumToString(accounts[i].assetType),
      });
    }
    this.optionsAssets = {
      title: {
        left: 'center',
        text: 'Assets',
      },
      tooltip: {
        trigger: 'item',
      },
      legend: {
        top: '5%',
        left: 'center',
      },
      series: [
        {
          name: 'Assets',
          type: 'pie',
          radius: ['40%', '70%'],
          avoidLabelOverlap: false,
          itemStyle: {
            borderRadius: 10,
            borderColor: '#fff',
            borderWidth: 2,
          },
          label: {
            show: false,
            position: 'center',
          },
          emphasis: {
            label: {
              show: true,
              fontSize: '40',
              fontWeight: 'bold',
            },
          },
          labelLine: {
            show: false,
          },
          data: assetsData,
        },
      ],
    };
  }

  generateCurrenciesOverTime() {
    const snapshots = this.analyticsService.analyticsInfo.snapshots;
    const xAxisData = [];
    const data1 = [];
    const data2 = [];
    const data3 = [];
    debugger;
    for (let i = 0; i < snapshots.length; i++) {
      xAxisData.push(
        this.datePipe.transform(snapshots[i].snapshotDate, 'MM-dd')
      );
      data1.push(snapshots[i].currencyAnalytics[0].portfolioPercent);
      data2.push(snapshots[i].currencyAnalytics[1].portfolioPercent);
      //data3.push(snapshots[i].currencyAnalytics[2].portfolioPercent);
    }

    this.optionsCurrenciesTime = {
      title: {
        text: 'Currencies Over Time',
      },
      toolbox: {
        // y: 'bottom',
        feature: {
          magicType: {
            type: ['stack'],
          },
          dataView: {},
          saveAsImage: {
            pixelRatio: 2,
          },
        },
      },
      tooltip: {},
      xAxis: {
        data: xAxisData,
        splitLine: {
          show: false,
        },
      },
      yAxis: {},
      series: [
        {
          name: this.CurrencyEnumToString(
            snapshots[0].currencyAnalytics[0].currency
          ),
          type: 'bar',
          data: data1,
          emphasis: {
            focus: 'series',
          },
          animationDelay: function (idx: any) {
            return idx * 10;
          },
        },
        {
          name: this.CurrencyEnumToString(
            snapshots[0].currencyAnalytics[1].currency
          ),
          type: 'bar',
          data: data2,
          emphasis: {
            focus: 'series',
          },
          animationDelay: function (idx: any) {
            return idx * 10 + 100;
          },
        },
      ],
      animationEasing: 'elasticOut',
      animationDelayUpdate: function (idx: any) {
        return idx * 5;
      },
    };
  }

  public CurrencyEnumToString(value: Currency): string {
    return Currency[value];
  }

  public AssetTypeEnumToString(value: AssetType): string {
    return AssetType[value];
  }
}
