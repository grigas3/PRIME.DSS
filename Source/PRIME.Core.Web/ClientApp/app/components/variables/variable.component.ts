import { Component, OnInit, Inject  } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
@Component({
    selector: 'variable',
    templateUrl: './variable.component.html',
    styleUrls: ['./variable.component.css']
})

export class VariableComponent implements OnInit {
  
    
    private httpClient: Http;
    private modelId: string;
    private baseUrl: string;
    variables: DSSVariable[];
  
    constructor(private route: ActivatedRoute,http: Http, @Inject('BASE_URL') baseUrl: string) {

        this.baseUrl = baseUrl;
        this.httpClient = http;
        this.variables = [];
        this.modelId = "";
    }

    ngOnInit() {

        this.route.params.subscribe(params => {
            console.log(params.id);
            this.modelId = params.id;
         

        });

        if (!this.modelId) {

            var url = this.baseUrl + 'api/v1/variable';

            this.httpClient.get(url).subscribe(result => {
                    console.log(result.json());
                    this.variables = result.json() as DSSVariable[];

                },
                error => console.error(error));
        } else {
            var url = this.baseUrl + 'api/v1/variable?id='+this.modelId;

            this.httpClient.get(url).subscribe(result => {
                    console.log(result.json());
                    this.variables = result.json() as DSSVariable[];

                },
                error => console.error(error));
        }
    }
  
   

}


interface DSSVariable {
    
    name: string;   
    code: string;
    cdss: string;
    codeNamespace: string;
    aggregator: string;
    modelsStr: string;
    exists: boolean;
   
}
