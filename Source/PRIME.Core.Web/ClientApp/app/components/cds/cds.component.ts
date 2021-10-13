import { Component, OnInit, Inject  } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
@Component({
    selector: 'cds',
    templateUrl: './cds.component.html',
    styleUrls: ['./cds.component.css']
})

export class CDSComponent implements OnInit {
  
    public meds: CDSCondition[];
    public cards: Cards[];
    public pdConditions: CDSCondition[];
  
    private httpClient: Http;
    private baseUrl: string;
    private dssId: string;
  
    constructor(private route: ActivatedRoute,http: Http, @Inject('BASE_URL') baseUrl: string) {


        this.baseUrl = baseUrl;
        this.httpClient = http;
        


    }

    ngOnInit() {

      
            console.log('init');
            this.route.params.subscribe(params => {
                console.log(params.id);
                this.dssId = params.id;
            });

        console.log(this.dssId);


            if (this.dssId) {

                var url = this.baseUrl + 'api/v1/condition?id=' + this.dssId;

                this.httpClient.get(url).subscribe(result => {
                        console.log(result.json());
                    this.pdConditions = result.json() as CDSCondition[];

                    },
                    error => console.error(error));

            } else {


                var url = this.baseUrl + 'api/v1/condition/med';


                this.httpClient.get(url).subscribe(result => {
                        console.log(result.json());
                        this.meds = result.json() as CDSCondition[];

                    },
                    error => console.error(error));
                var url = this.baseUrl + 'api/v1/condition/pd';


                this.httpClient.get(url).subscribe(result => {
                        console.log(result.json());
                        this.pdConditions = result.json() as CDSCondition[];

                    },
                    error => console.error(error));
            }


    }
  
    onSubmit(form: any): void {


        var url = this.baseUrl + 'api/v1/cds';

        var model = {
            id: this.dssId,
            input: JSON.stringify(form)
        };
      
      
        var request = this.httpClient.post(url, model).subscribe(
            res => {
                console.log(res);
                this.cards = res.json() as Cards[];
            },
            err => {
                console.log("Error occured");
            }
        );
        
        
    }


    getData(): void {
        //var url = this.baseUrl + 'api/v1/dsseval/input/' + this.modelId+'?patientId='+this.patientId;
        //var request = this.httpClient.get(url).subscribe(res => {
        //    console.log(res);
        //    var self = this;
        //    var sourceInput = res.json() as DSSValue[];

        //    var newInput = Object.assign(this.dssInput) as DSSValue[];

        //    sourceInput.forEach(function (newi) {

        //        var dssI = newInput.filter(function (item) {
        //            return item.name == newi.name;
        //        }).forEach(function (newitem) {

        //            newitem.value = newi.value;                 
        //        })
                
                
        //    });
        //    this.dssInput = newInput;

        //},
        //    err => {
        //        console.log("Error occured");
        //    }
        //);


    }

}


interface CDSCondition {
    value: number;
    name: string;   
    code: string;
   
}


interface Cards {
    indication: string;
    detail: string;
    summary: string;

}
