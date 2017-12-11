import { Component, Inject, OnInit } from '@angular/core';
import { DataSource } from '@angular/cdk/collections';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import { LoanService, LoanPayment } from '../services/loan.service';

import { ViewChild } from '@angular/core';
import { GoogleChartComponent } from 'ng2-google-charts';
import { HostListener } from '@angular/core';

@Component({
    selector: 'loanchart',
    templateUrl: './loancharts.component.html'
})
export class LoanChartsComponent implements OnInit {
    public loanpayments: LoanPayment[];
    
    dataSource = new ExampleDataSource(this.loanpayments);

    constructor(private loanService: LoanService) {
        this.loanpayments = new Array();
    }
    // type GoogleChartComponent and import for it can be ommited
    @ViewChild('your_chart') chart: GoogleChartComponent;

    // shows spinner while data is loading
    showSpinner: boolean;

    public chartData = {
        chartType: 'AreaChart', // your type
        dataTable: [
            ['Date', 'Principal', 'Interest'],
            ['2017-10-31', 79.59, 8.33],
            //['2017-11-30', 80.25, 7.67],
            //['2017-12-31', 80.92, 7.00],
            //['2018-01-31', 81.59, 6.33],
            //['2018-02-28', 82.27, 5.65],
            //['2018-03-31', 82.96, 4.96],
            //['2018-04-30', 83.65, 4.27],
            //['2018-05-31', 84.35, 3.57],
            //['2018-06-30', 85.05, 2.87],
            //['2018-07-31', 85.76, 2.16],
            //['2018-08-31', 86.47, 1.45],
            //['2018-09-30', 87.14, 0.73],
        ],
        options: {
            'title': 'Tasks',
            'isStacked': 'true',
        },
    };

    private processYourData() {
        var data = [
            ['Date', 'Principal', 'Interest'],
            ['2017-10-31', 179.59, 8.33],
            ['2017-11-30', 80.25, 7.67],
            ['2017-12-31', 80.92, 7.00],
            ['2018-01-31', 81.59, 6.33],
            ['2018-02-28', 82.27, 5.65],
            ['2018-03-31', 82.96, 4.96],
            ['2018-04-30', 83.65, 4.27],
            ['2018-05-31', 84.35, 3.57],
            ['2018-06-30', 85.05, 2.87],
            ['2018-07-31', 85.76, 2.16],
            ['2018-08-31', 86.47, 1.45],
            ['2018-09-30', 87.14, 0.73],
        ];
        data.length = 0;
        data.push(['Date', 'Principal', 'Interest']);
        data.push(['2017-10-31', 179.59, 8.33]);
        data.push(['2017-10-31', 99.59, 28.33]);
        console.log(this.loanpayments);
        for (let entry of this.loanpayments) {
            data.push([entry.periodFormatted, entry.principal, entry.interest]);
        }
        //for (let entry of data) {
        //    console.log(typeof entry[0])
        //    console.log(typeof entry[1])
        //    console.log(typeof entry[2])
        //}
        var data2 = [
            ['Date', 'Principal', 'Interest'],
            ['2017-10-31', 179.59, 8.33],
            ['2017-11-30', 80.25, 7.67],
            ['2017-12-31', 80.92, 7.00],
            ['2018-01-31', 81.59, 6.33],
            ['2018-02-28', 82.27, 5.65],
            ['2018-03-31', 82.96, 4.96],
            ['2018-04-30', 83.65, 4.27],
            ['2018-05-31', 84.35, 3.57],
            ['2018-06-30', 85.05, 2.87],
            ['2018-07-31', 85.76, 2.16],
            ['2018-08-31', 86.47, 1.45],
            ['2018-09-30', 87.14, 0.73],
        ];
        console.log(data);
        console.log(data2);
        console.log(typeof data);
        console.log(typeof data2);
        this.chartData.dataTable = data;
    }

    ngOnInit() {
        this.showSpinner = true;
        this.loanService.currentLoanPayments.subscribe(lp => {
            this.showSpinner = false;
            this.loanpayments = lp;
            this.dataSource = new ExampleDataSource(lp)
            this.processYourData();
        });
    }

}

class ExampleDataSource extends DataSource<any> {
    /** Connect function called by the table to retrieve one stream containing the data to render. */
    constructor(private loanpayments: LoanPayment[]) {
        super();
    }

    connect(): Observable<LoanPayment[]> {
        return Observable.of(this.loanpayments);
    }

    disconnect() { }
}