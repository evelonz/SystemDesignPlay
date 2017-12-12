import { Component, Inject, OnInit } from '@angular/core';
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
            ['2017-11-30', 80.25, 7.67],
        ],
        options: {
            'title': 'Tasks',
            'isStacked': 'true',
        },
    };

    private processYourData() {
        var data = [
            ['Date', 'Principal', 'Interest'],
            ['2017-10-31', 179.59, 8.33], // Bull shit row to get right date type.
        ];
        data.length = 1
        for (let entry of this.loanpayments) {
            data.push([entry.periodFormatted, entry.principal, entry.interest]);
        }


        if (this.loanpayments.length > 1) {
            this.showSpinner = false;
            this.chartData = {
                chartType: 'AreaChart',
                dataTable: data,
                options: {
                    'title': 'Tasks',
                    'isStacked': 'true',
                },
            };
        }
        
    }

    ngOnInit() {
        this.showSpinner = true;
        this.loanService.currentLoanPayments.subscribe(lp => {
            this.loanpayments = lp;
            this.processYourData();
        });
    }

}