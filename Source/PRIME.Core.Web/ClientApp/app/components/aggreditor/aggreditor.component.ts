import { Component, OnInit, Inject  } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
@Component({
    selector: 'aggreditor',
    templateUrl: './aggreditor.component.html'
})

export class AggrEditorComponent implements OnInit {
    public aggrConfig: AggrConfig;
    
    public newVar: AggrVariable;
    public aggrVariables: AggrVariable[];
    public aggrTypes: AggrType[];
    private httpClient: Http;
    private baseUrl: string;
    private modelId: string;
    public threshold: boolean;
    public predictedValue: number;
    constructor(private route: ActivatedRoute,http: Http, @Inject('BASE_URL') baseUrl: string) {
        
        this.baseUrl = baseUrl;
        this.httpClient = http;
        this.aggrConfig = {} as AggrConfig;
        this.newVar = {} as AggrVariable;
        this.aggrVariables = [];
        this.aggrTypes = [];
        this.addAggType('Max');
        this.addAggType('Min');
        this.addAggType('Average');
        this.addAggType('Any');
        this.modelId = "";
        this.threshold = false;
        this.predictedValue = 0;

    }

    addAggType(id:string) {
        var t = {} as AggrType;
        t.name = id;
        t.id = id;
        this.aggrTypes.push(t);
     

    }

    ngOnInit() {
        this.route.params.subscribe(params => {
            console.log(params.id);
            this.modelId = params.id;
            this.getModel();
          
        });
    }
    getModel():void {


        var url = this.baseUrl + 'api/v1/aggrconfig?id=' + this.modelId;
        console.log(url);

        this.httpClient.get(url).subscribe(result => {
            console.log(result);
            var config = result.json() as AggrConfig;

            if (config && config.variables)
                this.aggrVariables = config.variables;

            this.aggrConfig = config;


        }, error => console.error(error));


    }


    enableThreshold(): void {

        this.threshold = true;
    };
    disableThreshold(): void {
        this.threshold = false;
    };

    addVariable(): void {

        var aggVar = {} as AggrVariable;
        aggVar.code = this.newVar.code;
        aggVar.weight = this.newVar.weight;
        aggVar.source = this.newVar.source;
        this.aggrVariables.push(aggVar);

    };
    
    deleteVariable(index:number): void {
        this.aggrVariables.splice(index, 1);
        
        
    };

   
    saveAggr(): void {


        var url = this.baseUrl + 'api/v1/aggrconfig/';
        var self = this;
        this.aggrConfig.id = this.modelId;
        this.aggrConfig.variables = this.aggrVariables;
        var request = this.httpClient.post(url, this.aggrConfig).subscribe(
            res => {
                console.log(res);

                alert('Saved successfully!');
            },
            err => {
                console.log("Error occured");
            }
        );
        
        
    };


 

    evaluate(): void {


        var url = this.baseUrl + 'api/v1/aggreval/';
        var self = this;
        this.aggrConfig.id = this.modelId;
        this.aggrConfig.variables = this.aggrVariables;
        var request = this.httpClient.post(url, this.aggrConfig).subscribe(
            res => {
                console.log(res);

                alert('Saved successfully!');
            },
            err => {
                console.log("Error occured");
            }
        );


    };


}
interface AggrConfig {
    id: string;
    name: string;
    code: string;
    codeNameSpace: string;
    inputCode: string;
    outputScale: string;
    description: string;
    version: string;
    metaScale: number;
    metaScaleA: number;
    metaScaleB: number;
    max: number;
    min: number;
  
    thresholdType: string;
    metaAggregationType: string;
    thresholdValue: number;
    threshold: boolean;
    summary: string;
    option:string;
    variables: AggrVariable[];
}


interface AggrVariable {
    source: string;
    code: string;
    name: string;
    weight: number;
    value: number;
    
}
interface AggrType {
    id: string;
    name: string;
    

}
