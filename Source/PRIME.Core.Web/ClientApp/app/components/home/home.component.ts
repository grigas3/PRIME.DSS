import { Component, OnInit, Inject  } from '@angular/core';
import { Http } from '@angular/http';
//import { ActivatedRoute, Router, Params } from '@angular/router';
import { StateService } from '../../services/state.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    public navInfo: NavInfo[];
    private cdssUrl: string;
    private httpHandler: Http;
    constructor(http: Http,  @Inject('BASE_URL') baseUrl: string, public stateService: StateService) {
        this.cdssUrl = baseUrl + 'api/v1/cdsclient/';
        this.navInfo = [];
        this.httpHandler = http;

    }
    ngOnInit() {
        
        this.httpHandler.get(this.cdssUrl).subscribe(result => {
            var clients = result.json() as CDSClient[];

            this.navInfo.push({
                title: 'Go to admin page',
                description: 'From admin you can add clients, dss, aggregators and validate models',
                url: '/admin'
            } as NavInfo);
            var self = this;
            clients.forEach(function(k) {

                self.navInfo.push({
                    title: k.name,
                    description: k.description,
                    url: '/smartapp/card/' + k.id
            } as NavInfo);
            });

          
        }, error => console.error(error));


        //this.route.queryParams
        //    .subscribe(params => {
        //            console.log(params); // { orderby: "price" }
        //        this.launch = params.launch;
        //        console.log(this.launch); // price
        //        }
        //    );


        //if (this.iis) {
        //    this.httpHandler.get(this.authUrl);
        //}
    }
    
    public authorize(): void {

        //this.httpHandler.get(this.authUrl).subscribe(result => {
          
        //}, error => console.error(error));

    };



    
}
interface CDSClient {
    id: number;
    name: string;
    description: string;
    code: string;
    createdBy: string;


}

interface NavInfo {

    title: string;
    description: string;
    admin: boolean;
    url:string;

}
