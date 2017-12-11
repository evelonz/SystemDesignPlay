import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { LoanDataComponent } from './components/loans/loantable/loantable.component';
import { MaterialLoanDataComponent } from './components/loans/loantable/materialloantable.component';
import { LoanService } from './components/loans/services/loan.service';
import { LoanFormComponent } from './components/loans/loanforms/loanform.component';
import { LoanCalculatorComponent } from './components/loans/loancalculator.component';
import { LoanChartsComponent } from './components/loans/loancharts/loancharts.component';

import { Ng2GoogleChartsModule } from 'ng2-google-charts';

import {
    MatAutocompleteModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDatepickerModule,
    MatDialogModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatStepperModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
} from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
//import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { A11yModule             } from '@angular/cdk/a11y';
import { BidiModule             } from '@angular/cdk/bidi';
import { ObserversModule        } from '@angular/cdk/observers';
import { OverlayModule          } from '@angular/cdk/overlay';
import { PlatformModule         } from '@angular/cdk/platform';
import { PortalModule           } from '@angular/cdk/portal';
import { ScrollDispatchModule   } from '@angular/cdk/scrolling';
import { CdkStepperModule       } from '@angular/cdk/stepper';
import { CdkTableModule         } from '@angular/cdk/table';


@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        LoanDataComponent,
        LoanFormComponent,
        LoanCalculatorComponent,
        LoanChartsComponent,
        MaterialLoanDataComponent,
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'loan-calculator', component: LoanCalculatorComponent },
            { path: '**', redirectTo: 'home' }
        ]),
        Ng2GoogleChartsModule,
        BrowserAnimationsModule,
        //NoopAnimationsModule,
        ReactiveFormsModule,
        MatAutocompleteModule,
        MatButtonModule,
        MatButtonToggleModule,
        MatCardModule,
        MatCheckboxModule,
        MatChipsModule,
        MatDatepickerModule,
        MatDialogModule,
        MatExpansionModule,
        MatGridListModule,
        MatIconModule,
        MatInputModule,
        MatListModule,
        MatMenuModule,
        MatNativeDateModule,
        MatProgressBarModule,
        MatProgressSpinnerModule,
        MatRadioModule,
        MatRippleModule,
        MatSelectModule,
        MatSidenavModule,
        MatSliderModule,
        MatSlideToggleModule,
        MatSnackBarModule,
        MatStepperModule,
        MatTableModule,
        MatTabsModule,
        MatToolbarModule,
        MatTooltipModule,
        A11yModule,          
        BidiModule ,         
        ObserversModule     ,
        OverlayModule       ,
        PlatformModule      ,
        PortalModule        ,
        ScrollDispatchModule,
        CdkStepperModule    ,
        CdkTableModule      ,
    ],
    providers: [
        LoanService
    ]
})
export class AppModuleShared {
}
