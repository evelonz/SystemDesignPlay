import { Component, Inject, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { LoanService, LoanPayment } from '../services/loan.service';
import { MatFormFieldModule } from '@angular/material';
import { FormControl } from '@angular/forms';

@Component({
    selector: 'loanform',
    templateUrl: './loanform.component.html'
})
export class LoanFormComponent {
    private formdata: LoanFormData;

    constructor(private loanService: LoanService) {
        this.formdata = new LoanFormData(10, 1000, 10.0, new Date('2017-10-01'));
    }

    onSubmit() {
        this.loanService.updateLoanPaymentPlan(this.formdata.tenure, this.formdata.principal, this.formdata.interest, this.formdata.payoutDate);
    }
}

export class LoanFormData {
    constructor(
        public tenure: number,
        public principal: number,
        public interest: number,
        public payoutDate: Date
    ) { }
}