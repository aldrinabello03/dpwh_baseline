import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgApexchartsModule } from 'ng-apexcharts';

@Component({
  selector: 'app-chart-widget',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule],
  template: `
    <div class="chart-widget">
      <div class="chart-widget__header" *ngIf="title">
        <h3>{{ title }}</h3>
      </div>
      <apx-chart
        [series]="series"
        [chart]="chartOptions"
        [labels]="labels"
        [colors]="colors"
        [title]="titleOptions"
        [dataLabels]="dataLabels"
        [legend]="legend"
        [xaxis]="xaxis"
        [yaxis]="yaxis"
        [responsive]="responsive">
      </apx-chart>
    </div>
  `,
  styles: [`
    .chart-widget { background: white; border-radius: 12px; padding: 1rem; box-shadow: 0 2px 8px rgba(0,0,0,0.08); }
    .chart-widget__header h3 { margin: 0 0 1rem; font-size: 1rem; font-weight: 600; color: #1B2A4A; }
  `]
})
export class ChartWidgetComponent implements OnChanges {
  @Input() title = '';
  @Input() type: 'bar' | 'pie' | 'line' | 'donut' | 'area' | 'radialBar' = 'bar';
  @Input() series: any[] = [];
  @Input() labels: string[] = [];
  @Input() colors: string[] = ['#1B2A4A', '#E85D26', '#2ECC71', '#3498DB', '#F39C12'];
  @Input() height = 300;

  chartOptions: any = {};
  titleOptions: any = {};
  dataLabels: any = { enabled: true };
  legend: any = { position: 'bottom' };
  xaxis: any = {};
  yaxis: any = {};
  responsive: any[] = [{ breakpoint: 480, options: { chart: { width: 200 } } }];

  ngOnChanges(changes: SimpleChanges): void {
    this.chartOptions = { type: this.type, height: this.height, toolbar: { show: false } };
    this.titleOptions = {};
  }
}
