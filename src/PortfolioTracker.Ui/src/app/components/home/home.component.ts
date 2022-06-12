import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AnalyticsService } from 'src/app/services/analytics.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  options: any;
  constructor(
    public analyticsService: AnalyticsService,
    private datePipe: DatePipe
  ) {}

  ngOnInit(): void {
    this.analyticsService.getMarketValue().subscribe({
      next: (response) => {
        this.analyticsService.marketInfo = response;
      },
      error: (e) => {
        console.log(e);
      },
    });

    this.analyticsService.getAnalyticsInfo().subscribe({
      next: (response) => {
        this.analyticsService.analyticsInfo = response;
        this.generateChart();
      },
      error: (e) => {
        console.log(e);
      },
    });
  }

  private generateChart() {
    const snapshots = this.analyticsService.analyticsInfo.snapshots;
    const xAxisData = [];
    const portfolioValue = [];
    for (let i = 0; i < snapshots.length; i++) {
      xAxisData.push(
        this.datePipe.transform(snapshots[i].snapshotDate, 'MM-dd')
      );
      portfolioValue.push(snapshots[i].totalAmount);
    }

    this.options = {
      title: {
        left: 'center',
        text: 'Portfolio value',
      },
      color: {
        type: 'linear',
        x: 0,
        y: 0,
        x2: 0,
        y2: 1,
        colorStops: [
          {
            offset: 0,
            color: 'lightgreen', // color at 0%
          },
          {
            offset: 1,
            color: 'white', // color at 100%
          },
        ],
        global: false, // default is false
      },
      tooltip: {},
      xAxis: {
        data: xAxisData,
        silent: false,
        splitLine: {
          show: false,
        },
      },
      yAxis: {},
      series: [
        {
          name: 'Portfolio Value',
          type: 'bar',
          data: portfolioValue,
          animationDelay: (idx: number) => idx * 10 + 100,
        },
      ],
      animationEasing: 'elasticOut',
      animationDelayUpdate: (idx: number) => idx * 5,
    };
  }
}
