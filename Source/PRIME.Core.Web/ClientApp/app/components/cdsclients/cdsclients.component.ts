import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'cdsclients',
    templateUrl: './cdsclients.component.html'
})
export class CDSClientComponent {
    public clients: CDSClient[];
    public newClient: CDSClient;
    public logOutput: LogOutput[];

    private httpHandler: Http;
    private cdssUrl: string;
    

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.httpHandler = http;
        this.cdssUrl = baseUrl + 'api/v1/cdsclient/';
        this.clients = [];
        this.logOutput = [];
        this.newClient = {} as CDSClient;
        this.httpHandler.get(this.cdssUrl).subscribe(result => {
            this.clients = result.json() as CDSClient[];
            this.logOutput.push({
                message: 'DSS Models loaded', color: "#982315", error: false
            });
        }, error => console.error(error));
    }
    public refresh(): void {

        this.httpHandler.get(this.cdssUrl).subscribe(result => {
            this.clients = result.json() as CDSClient[];
            this.logOutput.push({
                message: 'DSS Models loaded', color: "#982315", error: false});
        }, error => console.error(error));

    };

    public onSubmit(): void {

        console.log(this.newClient);

        this.httpHandler.post(this.cdssUrl,this.newClient).subscribe(result => {
          
            this.logOutput.push({
                message: 'DSS Models Save', color: "#982315", error: false
            });
            this.refresh();
        }, error => console.error(error));

    }

    public delete(client:CDSClient):void {

        console.log(client.id);

        this.httpHandler.delete(this.cdssUrl+'?id='+client.id).subscribe(result => {

            this.logOutput.push({
                message: 'DSS Model '+client.id +' Deleted', color: "#ff2222", error: false
            });
            this.refresh();
        }, error => console.error(error));

    };

 
}

interface LogOutput {

    message: string;
    error: boolean;
    color: string;
    

}
interface CDSClient {
    id: number;
    name: string;
    description: string;
    code: string;
    createdBy: string;
    url: string;
    authorizationUrl: string;
    authenticationUrl: string;
    resourceUrl: string;
 
    
}
