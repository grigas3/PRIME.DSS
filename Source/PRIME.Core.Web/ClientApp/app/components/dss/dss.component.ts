import { ViewChild,Component, OnInit, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'dss',
    templateUrl: './dss.component.html'
})
export class DSSComponent implements  OnInit {
    public dssModels: DSSModel[];
    public clients: CDSClient[];
    public logOutput: LogOutput[];
    public templates: CDSTemplate[];
    public newDSS: DSSModel;
    private httpHandler: Http;
    private clientId: number;
    private dssFetchUrl: string;
    private clientFetchUrl: string;
    private dssDummyUrl: string;
    private dssTemplateUrl: string;
    private dssSchemaUrl: string;
    @ViewChild("fileInput") fileInput: any;
    constructor(private route: ActivatedRoute,http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.httpHandler = http;
        this.dssFetchUrl = baseUrl + 'api/v1/dss/';
        this.dssTemplateUrl = baseUrl + 'api/v1/dsstemplate/';
        this.clientFetchUrl = baseUrl + 'api/v1/cdsclient/';
        this.dssDummyUrl = baseUrl + 'api/v1/dss/dummy';        
        this.dssSchemaUrl = baseUrl + 'api/v1/dss/schema';        
        this.logOutput = [];
        this.dssModels = [];
        this.clientId = 0;
        
        this.newDSS = {} as DSSModel;

        this.templates = [];
        this.templates.push( { name: 'ManagePD1' } as CDSTemplate);
        this.templates.push( { name: 'ManagePD2' } as CDSTemplate);
        this.templates.push( { name: 'ManagePD3' } as CDSTemplate);
        this.templates.push({ name: 'ManagePDS' } as CDSTemplate);
        this.templates.push({ name: 'PDManager1' } as CDSTemplate);


        this.httpHandler.get(this.dssFetchUrl).subscribe(result => {
            this.dssModels = result.json() as DSSModel[];
            this.logOutput.push({
                message: 'DSS Models loaded', color: "#982315", error: false
            });
        }, error => console.error(error));


        this.httpHandler.get(this.clientFetchUrl).subscribe(result => {
            this.clients = result.json() as CDSClient[];
            this.logOutput.push({
                message: 'Clients loaded', color: "#982315", error: false
            });
        }, error => console.error(error));

    }
    public refresh(): void {

        this.httpHandler.get(this.dssFetchUrl).subscribe(result => {
            this.dssModels = result.json() as DSSModel[];
            this.logOutput.push({
                message: 'DSS Models loaded', color: "#982315", error: false});
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
    public addTemplate():void {

        this.httpHandler.get(this.dssTemplateUrl + "?id=" + this.newDSS.selectedTemplate + '&name=' + this.newDSS.name+'&code='+this.newDSS.code).subscribe(result => {
            console.log(result.json());
            this.newDSS.config = JSON.stringify(result.json());
        }, error => console.error(error));

    };
    public onSubmit(): void {

        let fi = this.fileInput.nativeElement;
        if (fi.files && fi.files[0]) {
            let fileToUpload = fi.files[0];

            if (fileToUpload) {
                let input = new FormData();
                input.append("file", fileToUpload);


                this.httpHandler
                    .post("/api/v1/dexi", input).subscribe(result => {
                        var file = result.json() as FileOutput;

                        this.newDSS.dexiFile = file.fileName;
                        this.submitDss();
                    });


                //this.uploadService.upload(fileToUpload)
                //    .subscribe(res => {
                //        console.log(res);
                //    });
            } else {
                console.log("FileToUpload was null or undefined.");
            }
        } else {
            this.submitDss();
        }

        

    }


    private submitDss(): void {
        this.httpHandler.post(this.dssFetchUrl, this.newDSS).subscribe(result => {

            this.logOutput.push({
                message: 'DSS Models Save', color: "#982315", error: false
            });
            alert('Model Saved');
            this.refresh();
        }, error => console.error(error));
    }

    public edit(client: DSSModel): void {

        console.log(client.id);
        this.newDSS = client;


    };
    public delete(client: DSSModel): void {

        console.log(client.id);

        this.httpHandler.delete(this.dssFetchUrl + '?id=' + client.id).subscribe(result => {

            this.logOutput.push({
                message: 'DSS Model ' + client.id + ' Deleted', color: "#ff2222", error: false
            });
            this.refresh();

        }, error => console.error(error));

    };

    ngOnInit() {
        console.log('init');
        this.route.params.subscribe(params => {
            console.log(params.id);
            this.clientId = params.id;
            

        });


    }
}
interface FileOutput {

    fileName: string;
    
}
interface LogOutput {

    message: string;
    error: boolean;
    color: string;
}
interface DSSModel {
    id: number;
    cdsClientId:number,
    name: string;
    code: string;
    version: string;
    dexiFile:string;
    cdsClientName: string;
    description: string;
    treatmentSuggestion: boolean;
    config:string;
    createdBy: string;
    selectedTemplate:string;
    execute(): void;
    
}
interface CDSClient {
    id: number;
    name: string;
    
}


interface CDSTemplate {
  
    name: string;
    
}
