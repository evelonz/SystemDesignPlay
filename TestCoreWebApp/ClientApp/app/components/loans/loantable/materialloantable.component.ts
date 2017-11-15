import { Component, Inject, OnInit } from '@angular/core';
import { DataSource } from '@angular/cdk/collections';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import { LoanService, LoanPayment } from '../services/loan.service';

@Component({
    selector: 'materialloantable',
    templateUrl: './materialloantable.component.html'
})
export class MaterialLoanDataComponent implements OnInit {
    public loanpayments: LoanPayment[];

    displayedColumns = ['periodFormatted', 'principal', 'interest', 'total'];
    dataSource = new ExampleDataSource(this.loanpayments);

    constructor(private loanService: LoanService) {
        
    }

    

    ngOnInit() {
        this.loanService.currentLoanPayments.subscribe(lp => this.dataSource = new ExampleDataSource(lp));
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