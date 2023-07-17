import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
import { InternalFormsSharedModule } from '@angular/forms/src/directives';

@Component({
    selector: 'medcheck',
    templateUrl: './medcheck.component.html'
})

    
export class MedCheckComponent {
    
    public geneResults: CheckResults[];
    public drugResults: CheckResults[];
    public genes: string;
    public drugs: string;
    private httpClient: Http;
    private baseUrl: string;      


    constructor(private route: ActivatedRoute, http: Http, @Inject('BASE_URL') baseUrl: string) {


        this.baseUrl = baseUrl;
        this.httpClient = http;
        this.genes = "";
        this.drugs = "";
        

    }
    public refresh(): void {

        //this.httpHandler.get(this.dssFetchUrl).subscribe(result => {
        //    this.dssModels = result.json() as MedCheckModel[];
        //    this.logOutput.push({
        //        message: 'DSS Models loaded', color: "#982315", error: false});
        //}, error => console.error(error));

    };

    ngOnInit() {

        this.route.params.subscribe(params => {

          


        });
    }

    execute(): void {


        var url = this.baseUrl + 'api/v1/medcheck/?genes=' + this.genes + '&drugs=' + this.drugs;
        this.httpClient.get(url).subscribe(result => {
            var self = this;
            this.drugResults = (result.json() as CheckResult).drugs;
            this.geneResults = (result.json() as CheckResult).genes;

        }, error => console.error(error));

        

    }
   
}

interface LogOutput {

    message: string;
    error: boolean;
    color: string;
    
}

interface CheckResults {
    source: string;
    target: string;    
    interaction: string;
}

interface Result {

    result: CheckResult;
}
interface CheckResult {
    drugs: CheckResults[];
    genes: CheckResults[];
}