import { ViewChild, Component, OnInit, Inject  } from '@angular/core';
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
    private dssOutputcode: string;
    public results: ValidationResult[];
    public report: ValidationReport;
    public caseValidation:boolean;
  
    constructor(private route: ActivatedRoute,http: Http, @Inject('BASE_URL') baseUrl: string) {


        this.baseUrl = baseUrl;
        this.httpClient = http;
        this.results = [];
        this.caseValidation = true;
        this.dssOutputcode = "";

    }
    @ViewChild("fileInput") fileInput: any;
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


                    this.pdConditions.forEach(function(c) {
                        if (c.values) {
                            c.options = [];
                           
                            var count = 0;
                            c.values.forEach(function(v) {
                               
                                c.options.push({ name: v, value: count } as CondOption);
                                count++;
                            });

                        }

                    });


                    },
                    error => console.error(error));

            }
            

    }
  
    onSubmit(form: any): void {


        var url = this.baseUrl + 'api/v1/cds';

        var model = {
            id: this.dssId,
            code: this.dssOutputcode,
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

    exportTemplate(): void {

        var url = this.baseUrl + 'api/v1/dssvalidate?id=' + this.dssId+'&code='+this.dssOutputcode;

        window.location.href = url;
      
    }

    

    changeType(): void {
        this.caseValidation = !this.caseValidation;

    }
    addFile(): void {
        let fi = this.fileInput.nativeElement;
        if (fi.files && fi.files[0]) {
            let fileToUpload = fi.files[0];

            if (fileToUpload) {
                let input = new FormData();
                input.append("file", fileToUpload);

                this.httpClient
                    .post("/api/v1/dssvalidate?id=" + this.dssId+'&code='+this.dssOutputcode, input).subscribe(result => {
                        this.report = result.json() as ValidationReport;
                        this.results = this.report.results;

                    });
                //this.uploadService.upload(fileToUpload)
                //    .subscribe(res => {
                //        console.log(res);
                //    });
            } else
                console.log("FileToUpload was null or undefined.");
        }
    }
    //downloadFile(data: Response) {
    //    const blob = new Blob([data], { type: 'text/csv' });
    //    const url = window.URL.createObjectURL(blob);
    //    window.open(url);
    //}
    getData(): void {
       

    }

}


interface ValidationReport {
    falsePositives: number;
    truePositives: number;
    falseNegatives: number;
    trueNegatives: number;
    results: ValidationResult[];
}

interface ValidationResult {
    input: string;
    expectedOutput: string;
    actualOutput: string;
    output: string;
    valid: boolean;
}

interface CDSCondition {
    value: number;
    name: string;   
    code: string;
    values: string[];
    options: CondOption[];

}


interface CondOption {
    name: string;
    value: number;
    

}


interface Cards {
    indication: string;
    detail: string;
    summary: string;

}
