import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
import { InternalFormsSharedModule } from '@angular/forms/src/directives';

@Component({
    selector: 'stitch',
    templateUrl: './stitch.component.html'
})

    
export class StitchComponent {
    
    public matchResults: MatchResults[];
    public interactionResults:InteractionResults[];
    
    public term: string;
    
    private httpClient: Http;
    private baseUrl: string;      


    constructor(private route: ActivatedRoute, http: Http, @Inject('BASE_URL') baseUrl: string) {


        this.baseUrl = baseUrl;
        this.httpClient = http;
        this.term = "";


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

    match(): void {


        var url = this.baseUrl + 'api/v1/stitch/match?term=' + this.term;
        this.httpClient.get(url).subscribe(result => {
            var self = this;

            this.matchResults = (result.json() as MatchResult).results;

        }, error => console.error(error));

        

    }
    public interactions(client: MatchResults): void {
        var url = this.baseUrl + 'api/v1/stitch/interactions?term=' + client.stringId;
        this.httpClient.get(url).subscribe(result => {
            var self = this;

            this.interactionResults = (result.json() as InteractionsResult).results;
            console.log('Interaction results assigned');
        }, error => console.error(error));

    };
  
}

interface LogOutput {

    message: string;
    error: boolean;
    color: string;
    
}

interface MatchResult {

    results: MatchResults[];
}

interface InteractionsResult {

    results: InteractionResults[];
}

interface MatchResults {
       queryIndex:string;
          queryItem:string;
          stringId:string;
          ncbiTaxonId:string;
          taxonName:string;
          preferredName:string;
          annotation:string;
}


interface InteractionResults {
    idA: string;
    idB: string;
    nameA: string;
    nameB: string;
    score: string;
    
}

