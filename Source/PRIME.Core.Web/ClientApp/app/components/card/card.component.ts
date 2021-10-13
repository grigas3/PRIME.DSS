import { Component, OnInit, Inject } from "@angular/core";
import { Http } from "@angular/http";
import { ActivatedRoute } from "@angular/router";

@Component({
    selector: "card",
    templateUrl: "./card.component.html",
    styleUrls: ['./card.component.css']
})
export class CardComponent implements OnInit {
    dssModels: DSSModel[];
    inputVariables: Condition[];
    patients: Patient[];
    cards: Cards[];

    
    patientId: string;
    code: string;
    clientAuthorizationUrl: string;
    clientAuthenticationUrl: string;
    clientResourceUrl: string;
    public hasLaunch:boolean;
    clientName: string;
    public inputVariablesReceived:boolean;
    accessToken: string;
    loading:boolean;
    public accessTokenReceived:boolean;
    public urlsReceived:boolean;
    public recommendationsReceived:boolean;
    private httpClient: Http;
    private baseUrl: string;
    private modelId: string;

    constructor(private route: ActivatedRoute, http: Http, @Inject("BASE_URL") baseUrl: string) {


        this.baseUrl = baseUrl;
        this.inputVariablesReceived = false;
        this.httpClient = http;
        this.loading = false;
        this.urlsReceived = false;
        
        //   this.code = "";
        this.modelId = "";
        this.patientId = "";
        this.hasLaunch = false;
        this.clientName = "";
        this.clientAuthorizationUrl = "";
        this.clientAuthenticationUrl = "";
        this.clientResourceUrl = "";
     //   this.accessToken = "";
        this.dssModels = [];
        this.cards = [];
        this.patients = [];
        this.inputVariables = [];
        this.accessToken = "";
        this.accessTokenReceived = false;
        this.recommendationsReceived = false;
    }

    ngOnInit() {

        this.route.params.subscribe(params => {
            console.log(params);
            this.modelId = params.id;
        });

        //Get Fixed query parameters
        this.code = this.route.snapshot.queryParams["code"];
        this.clientAuthorizationUrl = this.route.snapshot.queryParams["authorUrl"];
        this.clientAuthenticationUrl = this.route.snapshot.queryParams["authentUrl"];
        this.clientResourceUrl = this.route.snapshot.queryParams["resUrl"];
        var launch = this.route.snapshot.queryParams["launch"];

        if (launch) {

            this.hasLaunch = true;
            this.patientId = launch;
            
        }

        //First Get list of CDSS Client DSS models
        const url = this.baseUrl + "api/v1/cdsClient?id=" + this.modelId + "&includeModels=true";

        this.httpClient.get(url).subscribe(
            res => {
                console.log(res);
                this.clientName = (res.json() as ClientModel).name;
                this.dssModels = (res.json() as ClientModel).models;
            },
            err => {
                console.log("Error occured");
            }
        );

        //Get the path to redirect back to
        var path = window.location.href;
        if (path.indexOf('?') > 0) {
            path = window.location.href.split('?')[0];

            //if (this.hasLaunch) {
            //    path = path + '?launch=' + this.patientId;
            //}
        }


        if ((this.code) &&(this.clientAuthenticationUrl)) {
            //Get Access token if we have the code
            console.log(this.code);
            console.log(this.clientAuthenticationUrl );
           this.getAccessToken();

        } else if (this.code) {

            this.loading = true;
            //Get capabilities and token
            var fhirurl = this.baseUrl + "api/v1/fhir?id=" + this.modelId;
            this.httpClient.get(fhirurl).subscribe(
                
                res => {

                    this.loading = false;
                    console.log(fhirurl);
                    this.urlsReceived = true;
                    this.clientAuthenticationUrl = (res.json() as FHIREndpoint).authenticationUrl;
                    this.clientAuthorizationUrl = this.fixUrl((res.json() as FHIREndpoint).authorizationUrl, path);
                    this.clientResourceUrl = (res.json() as FHIREndpoint).resourceUrl;
                    this.getAccessToken();

                },
                err => {
                    this.loading = false;
                    console.log("Error occured");
                }
            );
        } else {


            this.loading = true;
            //Get capabilities (no token)
            var fhirurl = this.baseUrl + "api/v1/fhir?id=" + this.modelId;
            this.httpClient.get(fhirurl).subscribe(
                res => {
                    this.loading = false;
                    console.log(fhirurl);

                  //  this.urlsReceived = true;
                    this.clientAuthenticationUrl = (res.json() as FHIREndpoint).authenticationUrl;
                    this.clientAuthorizationUrl = this.fixUrl((res.json() as FHIREndpoint).authorizationUrl,path);
                    this.clientResourceUrl = (res.json() as FHIREndpoint).resourceUrl;

                },
                err => {
                    console.log("Error occured");
                }
            );

        }


    }

    fixUrl(authorizationUrl:string,path:string): string {

        
            return authorizationUrl.replace(/\${redirectUrl}/, path).replace(/\${launch}/, this.patientId);
        
    };





    /**
     * Get Access Token using the code provided after user authorization
     */
    getAccessToken(): void {

        //Get capabilities
        var fhirurl = this.baseUrl + "api/v1/fhirAuth";
        const model = { id: this.modelId, code: this.code, fhirAuthUrl: this.clientAuthenticationUrl };

        this.loading = true;
        this.httpClient.post(fhirurl, model).subscribe(
            res => {
                console.log(fhirurl);
                this.loading =false;
                this.accessToken = (res.json() as AuthResult).access_token;

                if (this.accessToken != null) {

                    this.accessTokenReceived = true;

                    if (this.hasLaunch) {

                        this.execute();


                    } else {
                        this.getPatients();

                    }
                }
                else {
                    this.code = "";
                    this.accessTokenReceived = false;

                }
                

                console.log(this.accessToken);
            },
            err => {
                this.loading = false;
                console.log("Error occured");
            }
        );
    }
    changeValue(id:string): void {

        this.patientId = id;
    }
    
    getPatients(): void {
        console.log("Getting patients");
        //Get capabilities
        var fhirurl = this.baseUrl + "api/v1/patient";
     
        console.log("patientId " + this.patientId);
        const model = {
            fhirServer: this.clientResourceUrl,
            context: {
                PatientId: this.patientId,
                ClientId: this.modelId,
            },
            fhirAuthorization: { Access_Token: this.accessToken },
            requiresAuthentication: true,
          

        };
     
        this.loading = true;
        this.httpClient.post(fhirurl, model).subscribe(
            res => {
                console.log(fhirurl);
                this.loading = false;
                this.patients = (res.json() as Patient[]);

            },
            err => {
                this.loading = false;
                console.log("Error occured");
            }
        );
    }

    /**
     * Redirect to the Authorization Url
     */
    authorize(): void {

        var url = this.clientAuthorizationUrl;
        window.location.href = url;
    }

/**
 * Execute Method
 * @returns {} 
 */
    execute(): void {
        const url = this.baseUrl + "api/v1/CDSServices";

        console.log(this.patientId);
        const model = {
            fhirServer: this.clientResourceUrl,
            context: {
                PatientId: this.patientId,
                ClientId: this.modelId,
            },
            fhirAuthorization: { Access_Token: this.accessToken },
            requiresAuthentication: true,
            evaluateDSS:false

        };
        console.log(model);
        this.loading = true;
        const request = this.httpClient.post(url, model).subscribe(res => {
                console.log(res);
            var self = this;
            this.loading = false;
            this.inputVariablesReceived = true;
            this.inputVariables = res.json() as Condition[];

            this.evaluate();

        },
            err => {
                this.loading = false;
                console.log("Error occured");
            }
        );


    }

    selectValues(): void
    {
        this.recommendationsReceived = false;
    };

    evaluate(): void {

        var url = this.baseUrl + 'api/v1/dsseval';
        var model = { 'patientId': this.patientId, 'clientId': this.modelId, 'variables': this.inputVariables };
        this.loading = true;
        var request = this.httpClient.post(url, model).subscribe(
            res => {
                console.log(res);
                this.loading = false;
                this.cards = res.json() as Cards[];

                this.recommendationsReceived = true;

            },
            err => {
                this.loading = false;
                console.log("Error occured");
            }
        );

    };



    

}
interface Cards {
    indication: string;
    detail: string;
    summary: string;

}

interface ClientModel {
    models: DSSModel[];
    name: string;
    url: string;
}

interface FHIREndpoint {
    authorizationUrl: string;
    authenticationUrl: string;
    resourceUrl: string;
}

interface DSSModel {
    name: string;
    evaluated: boolean;
}

interface LoginResult {

    name: string;
    evaluated: boolean;

}
interface  Condition {

    code: string;
    description: string
    codeNamespace: string;
    value:number;

}

interface AuthResult {
    access_token: string;
}


interface Patient {
    
    familyName: string;
    givenName: string;
    id: string;
}
