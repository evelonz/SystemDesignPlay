import { Component, Inject, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { LoanService, LoanPayment } from '../services/loan.service';

@Component({
    selector: 'loanform',
    templateUrl: './loanform.component.html'
})
export class LoanFormComponent {
    public tenure: number;
    public principal: number;
    public interest: number;
    private payoutDate: string;

    constructor(private loanService: LoanService) {
        this.tenure = 10;
        this.principal = 10000;
        this.interest = 10.0;
        this.payoutDate = '2017-10-01';

    }

    onClickMe() {
        this.loanService.updateLoanPaymentPlan(this.tenure, this.principal, this.interest, this.payoutDate);
    }
}
