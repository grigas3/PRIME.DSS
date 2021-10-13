import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { ChartsModule } from 'ng2-charts';
import { StateService } from './services/state.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './components/app/app.component';
import { AdminComponent } from './components/admin/admin.component';
import { SmartAppComponent } from './components/smartapp/smartapp.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { DSSValueComponent } from './components/dssvalue/dssvalue.component';
import { DSSEvalComponent } from './components/dsseval/dsseval.component';
import { DSSEditorComponent } from './components/dsseditor/dsseditor.component';
import { CDSComponent } from './components/cds/cds.component';
import { AlertComponent } from './components/alert/alert.component';
import { AlertValueComponent } from './components/alertvalue/alertvalue.component';
import { AggrValueComponent } from './components/aggrvalue/aggrvalue.component';
import { AggrEditorComponent } from './components/aggreditor/aggreditor.component';
import { DSSComponent } from './components/dss/dss.component';
import { CDSClientComponent } from './components/cdsclients/cdsclients.component';
import { MedCheckComponent } from './components/medcheck/medcheck.component';
import { AggregationComponent } from './components/aggregation/aggr.component';
import { VariableComponent } from './components/variables/variable.component';
import { CardComponent } from './components/card/card.component';
//import { ToasterModule, ToasterService } from 'angular2-toaster';

@NgModule({
    declarations: [
        AppComponent,
        SmartAppComponent,
        NavMenuComponent,
        AdminComponent,
        DSSValueComponent,        
        DSSComponent,
        DSSEditorComponent,
        CDSComponent,
        MedCheckComponent,
        AlertComponent,
        AlertValueComponent,
        AggregationComponent,
        AggrEditorComponent,
        VariableComponent,
        AggrValueComponent,  
        DSSEvalComponent,
        CDSClientComponent,
        CardComponent,
        HomeComponent
    ],
    providers:[StateService],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        ChartsModule,
       // BrowserAnimationsModule, ToasterModule.forRoot(),
        RouterModule.forRoot([
            
            { path: 'smartapp',component:SmartAppComponent,children:[{ path: 'card/:id', component: CardComponent }]
            },
            {
               path: 'admin',
               component: AdminComponent,
               children: [
                
               //    { path: 'dssvalue/:id', component: DSSValueComponent },
                   { path: 'aggrvalue/:id', component: AggrValueComponent },
                   { path: 'alertvalue/:id', component: AlertValueComponent },
                   { path: 'dss/:id', component: DSSComponent },
                   { path: 'dss', component: DSSComponent },
                   { path: 'cdsclients', component: CDSClientComponent },
               { path: 'variable', component: VariableComponent },
                   //{ path: 'cds', component: CDSComponent },
                   { path: 'cds/:id', component: CDSComponent },
                   { path: 'dsseditor/:id', component: DSSEditorComponent },
                   { path: 'aggreditor/:id', component: AggrEditorComponent },
                   { path: 'variable/:id', component:VariableComponent },
             
                   { path: 'medcheck', component: MedCheckComponent },
                   { path: 'alert', component: AlertComponent },
                   { path: 'aggregation/:id', component: AggregationComponent },
                   { path: 'aggregation', component: AggregationComponent },
                   { path: '', redirectTo: 'cdsclients', pathMatch: 'full' },
               ]
           },
            {
                path: 'home', component: HomeComponent,
                children:[
                    {
                        path: 'dssvalue/:id', component: DSSValueComponent

                    },
                        { path: 'card/:id/', component: CardComponent }
                    ]


            },
            { path: '', redirectTo: 'home', pathMatch: 'full' },
          
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}

