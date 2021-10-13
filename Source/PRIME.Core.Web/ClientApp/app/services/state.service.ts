export class StateService {

    private patientId: string;
    

    constructor() {
        this.patientId = "";
    }
    public setPatientId(id:string): void {

        this.patientId = id;
     

    };
    public getPatientId(): string {

        return this.patientId;

    };
  
}

