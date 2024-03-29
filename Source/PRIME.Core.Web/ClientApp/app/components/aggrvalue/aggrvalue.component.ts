import { ViewChild,Component, OnInit, Inject  } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
@Component({
    selector: 'aggrvalue',
    templateUrl: './aggrvalue.component.html'
})

export class AggrValueComponent implements OnInit {
    public aggrConfig: AggrConfig;
    public aggrOutput: PDObservation[];
    public results: ValidationResult[];
    public patientId: string;
    public bundle: string;
    private httpClient: Http;
    private baseUrl: string;
    private modelId: string;    
    private code: string;
    

    constructor(private route: ActivatedRoute,http: Http, @Inject('BASE_URL') baseUrl: string) {


        this.baseUrl = baseUrl;
        this.httpClient = http;
        this.bundle = "";
        this.patientId = "";

    }
    @ViewChild("fileInput") fileInput: any;
    ngOnInit() {

        this.route.params.subscribe(params => {
            
            this.modelId = params.id; 
            this.getModel();
          
        });

      //  this.randomize();

    }
    getModel(): void {
        
        var url = this.baseUrl + 'api/v1/aggregation/config/' + this.modelId;        

        this.httpClient.get(url).subscribe(result => {
            console.log(result.json());
            this.aggrConfig = result.json() as AggrConfig;
            this.code = this.aggrConfig.code;
            console.log(this.aggrConfig);
            console.log(this.code);
        }, error => console.error(error));


    }
    addFile(): void {
        let fi = this.fileInput.nativeElement;
        if (fi.files && fi.files[0]) {
            let fileToUpload = fi.files[0];

            if (fileToUpload) {
                let input = new FormData();
                input.append("file", fileToUpload);

                this.httpClient
                    .post("/api/v1/aggrvalidate?id="+this.modelId, input).subscribe(result => {
                        this.results = result.json() as ValidationResult[];
                    });
                //this.uploadService.upload(fileToUpload)
                //    .subscribe(res => {
                //        console.log(res);
                //    });
            } else
                console.log("FileToUpload was null or undefined.");
        }
    }
    execute(): void {

        var url = this.baseUrl + 'api/v1/fhireval/';

        var model = {} as FHIREvalModel;

        model.patientId = this.patientId;
        model.id = this.modelId;
        model.bundleJson = this.bundle;

        this.httpClient.post(url, model).subscribe(result => {
            console.log(result);
            //this.logOutput.push({
            //    message: 'Aggr Models Saved', color: "#982315", error: false
            //});
            //this.refresh();
        }, error => console.error(error));

     
        
        
    }

    // lineChart
    public lineChartData: Array<any>;
    public lineChartLabels: Array<any>;
    public lineChartOptions: any = {
        responsive: true
    };
    public lineChartColors: Array<any> = [
        { // grey
            backgroundColor: 'rgba(148,159,177,0.2)',
            borderColor: 'rgba(148,159,177,1)',
            pointBackgroundColor: 'rgba(148,159,177,1)',
            pointBorderColor: '#fff',
            pointHoverBackgroundColor: '#fff',
            pointHoverBorderColor: 'rgba(148,159,177,0.8)'
        },
      
    ];
    public lineChartLegend: boolean = true;
    public lineChartType: string = 'line';

    public randomize(): void {
        let _lineChartData: Array<any> = new Array(this.lineChartData.length);
        for (let i = 0; i < this.lineChartData.length; i++) {
            _lineChartData[i] = { data: new Array(this.lineChartData[i].data.length), label: this.lineChartData[i].label };
            for (let j = 0; j < this.lineChartData[i].data.length; j++) {
                _lineChartData[i].data[j] = Math.floor((Math.random() * 100) + 1);
            }
        }
        this.lineChartData = _lineChartData;
    }

    private  timeConverter(UNIX_timestamp:number):string{
    var a = new Date(UNIX_timestamp);
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var year = a.getFullYear();
    var month = months[a.getMonth()];
    var date = a.getDate();
    var hour = a.getHours();
    var min = a.getMinutes();
    var sec = a.getSeconds();

    if (UNIX_timestamp < 100000000)
        return  hour + ':' + min + ':' + sec;
        else
        return date + ' ' + month + ' ' + year + ' ' + hour + ':' + min + ':' + sec;
    
    }
    // events
    public chartClicked(e: any): void {
        console.log(e);
    }

    public chartHovered(e: any): void {
        console.log(e);
    }

}

//Aggregation Confi Model
interface AggrConfig {
    version: number;
    name: string;
    code: string;
}

interface ValidationResult {
    input: string;
    expectedOutput: string;
    output: string;
    valid:boolean;
}

//Aggregation Confi Model
interface FHIREvalModel {
    bundleJson: string;
    patientId: string;
    id: string;
}
//Aggregation Confi Model
interface PDObservation {
    patientId: string;
    codeid: string;
    timestamp: number;
    id: string;
    value: string;
}
