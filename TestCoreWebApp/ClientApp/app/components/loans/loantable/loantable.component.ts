import { Component, Inject, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { LoanService, LoanPayment } from '../services/loan.service';

@Component({
    selector: 'loantable',
    templateUrl: './loantable.component.html'
})
export class LoanDataComponent implements OnInit {
    public loanpayments: LoanPayment[];

    constructor(private loanService: LoanService) {

    }

    ngOnInit() {
        this.loanService.currentLoanPayments.subscribe(lp => this.loanpayments = lp);
    }

}
