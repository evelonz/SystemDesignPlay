import { Injectable, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()
export class LoanService {
    private baseUrl: string;
    private http: Http;

    private loanPaymentSource = new BehaviorSubject<LoanPayment[]>(new Array<LoanPayment>(0));
    currentLoanPayments = this.loanPaymentSource.asObservable();

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
        this.http = http;
    }

    updateLoanPaymentPlan(tenure: number, principal: number, interest: number, payoutDate: Date) {
        let date = [
            payoutDate.getFullYear(),
            ('0' + (payoutDate.getMonth() + 1)).slice(-2),
            ('0' + payoutDate.getDate()).slice(-2)
        ].join('-');
        var queryString = this.baseUrl + 'api/Loan/Loan?' +
            'loanType=FixedEmiLoan' + //loanType +
            '&tenure=' + tenure +
            '&interestRate=' + interest +
            '&principal=' + principal +
            '&payoutDate=' + date;
        this.http.get(queryString).subscribe(result => {
            //console.error(result.json());
            var p = result.json() as LoanPayment[];
            this.loanPaymentSource.next(p);
        }, error => console.error(error));
    }
    
}

export class LoanPayment {
    periodFormatted: string;
    principal: number;
    interest: number;
    total: number;
}


