import { Component, Inject,OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
@Component({
    selector: 'aggregation',
    templateUrl: './aggr.component.html'
})
export class AggregationComponent implements OnInit {
    public aggrModels: AggrModel[];
    public newAggr: AggrModel;
    public clients: CDSClient[];
    public logOutput: LogOutput[];    
    private httpHandler: Http;
    private dssFetchUrl: string;
    private clientId: number;
    private clientFetchUrl: string;
    private dssDummyUrl: string;
    private dssSchemaUrl: string;
    private dssExecuteUrl: string;
    constructor(private route: ActivatedRoute,http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.httpHandler = http;
        this.dssFetchUrl = baseUrl + 'api/v1/aggregation';
        this.dssDummyUrl = baseUrl + 'api/v1/aggregation/dummy';        
        this.dssSchemaUrl = baseUrl + 'api/v1/aggregation/schema';       
        this.clientFetchUrl = baseUrl + 'api/v1/cdsclient/';
        this.logOutput = [];
        this.newAggr = {} as AggrModel;
        
      
    }
    public refresh(): void {

        this.httpHandler.get(this.dssFetchUrl).subscribe(result => {
            this.aggrModels = result.json() as AggrModel[];
            this.logOutput.push({
                message: 'Aggregation Models loaded', color: "#982315", error: false});
        }, error => console.error(error));

    };  

    ngOnInit() {

        this.route.params.subscribe(params => {

            this.newAggr.cdsClientId = params.id;
        });
        this.httpHandler.get(this.dssFetchUrl).subscribe(result => {
            this.aggrModels = result.json() as AggrModel[];
            this.logOutput.push({
                message: 'Aggregation Models loaded', color: "#982315", error: false
            });
        }, error => console.error(error));
        
            this.httpHandler.get(this.clientFetchUrl).subscribe(result => {
                    this.clients = result.json() as CDSClient[];
                    this.logOutput.push({
                        message: 'Clients loaded',
                        color: "#982315",
                        error: false
                    });
                },
                error => console.error(error));
        
    };

   
    public edit(currentValue: AggrModel): void {

        this.newAggr = currentValue;
    }
    public delete(client: AggrModel): void {

        console.log(client.id);

        this.httpHandler.delete(this.dssFetchUrl + '?id=' + client.id).subscribe(result => {

            this.logOutput.push({
                message: 'Aggr Model ' + client.id + ' Deleted', color: "#ff2222", error: false
            });
            this.refresh();
        }, error => console.error(error));

    };
    public onSubmit():void {

        console.log(this.newAggr);
        this.newAggr.cdsClientId = this.clientId;

        this.httpHandler.post(this.dssFetchUrl, this.newAggr).subscribe(result => {

            this.logOutput.push({
                message: 'Aggr Models Saved', color: "#982315", error: false
            });
            this.refresh();
        }, error => console.error(error));

    };

    public getSchema(): void {

        this.httpHandler.get(this.dssSchemaUrl).subscribe(result => {
            var schema = result.json() as string;
            this.logOutput.push({
                message: schema, color: "#982315", error: false
            });
        }, error => console.error(error));

    };

    public addDummyData():void {

        this.httpHandler.get(this.dssDummyUrl).subscribe(result => {
            this.refresh();
        }, error => console.error(error));

    };
}


interface AggrModel {
    id: number;
    name: string;
    code: string;
    version: string;
    cdsClientName: number;
    cdsClientId: number;
    description: string;
    config:string;
    createdBy: string;
    execute(): void;
    
}
interface LogOutput {

    message: string;
    error: boolean;
    color: string;


}
interface CDSClient {
    id: number;
    name: string;

}
