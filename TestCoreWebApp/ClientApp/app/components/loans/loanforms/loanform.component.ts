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
    loantypes = [
        { value: 'FixedEmiLoan', viewValue: 'Fixed EMI' },
        { value: 'FixedAmortizationLoan', viewValue: 'Fixed Amortization' },
        { value: 'FixedInterestLoan', viewValue: 'Fixed Interest' }
    ];

    constructor(private loanService: LoanService) {
        this.formdata = new LoanFormData('FixedEmiLoan', 10, 1000, 10.0, new Date('2017-10-01'), false);
    }

    onSubmit() {
        this.loanService.updateLoanPaymentPlan(this.formdata.loantype, this.formdata.tenure, this.formdata.principal,
            this.formdata.interest, this.formdata.payoutDate, this.formdata.addSinglePayment);
    }
}

export class LoanFormData {
    constructor(
        public loantype: string,
        public tenure: number,
        public principal: number,
        public interest: number,
        public payoutDate: Date,
        public addSinglePayment: boolean,
    ) { }
}