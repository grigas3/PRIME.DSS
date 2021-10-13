import { Component, OnInit, Inject  } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
@Component({
    selector: 'dsseditor',
    templateUrl: './dsseditor.component.html'
})

export class DSSEditorComponent implements OnInit {
    //public dssConfig: FuzyCollection[];
    public dssRule: AndRule;
    public newRule: AndRule;
    public newAndRule: FuzzyFunc;
    public dssRules: AndRule[];
    public variables: FuzzyVariable[];
    public treatment: TreatmentClassifier;
    public dssAndRules: FuzzyFunc[];
    public fuzzyRules: FuzyRule[];
    public newfuzzyRule: FuzyRule;
    private httpClient: Http;
    private baseUrl: string;
    public preconditionCode:string;
    private modelId: string;
    constructor(private route: ActivatedRoute,http: Http, @Inject('BASE_URL') baseUrl: string) {
        
        this.baseUrl = baseUrl;
        this.httpClient = http;
        this.fuzzyRules = [];
        this.newRule = {} as AndRule;
        this.dssRule = {} as AndRule;
        this.newAndRule = {} as FuzzyFunc;
        this.newfuzzyRule = {} as FuzyRule;
        this.dssRules = [];
        this.dssAndRules = [];
        this.variables = [];
        this.modelId = "";
        this.preconditionCode = "";
        this.treatment = {} as TreatmentClassifier;
        this.treatment.preconditions = [];
    }

    ngOnInit() {
        this.route.params.subscribe(params => {
            console.log(params.id);
            this.modelId = params.id;
            this.getModel();
          
        });
    }
    getModel():void {

        var url = this.baseUrl + 'api/v1/dssconfig?id=' + this.modelId;
        console.log(url);

        this.httpClient.get(url).subscribe(result => {
            console.log(result);
            this.treatment = result.json() as TreatmentClassifier;

            
            if (this.treatment.ruleModel &&
                this.treatment.ruleModel.rules &&
                this.treatment.ruleModel.rules.length > 0) {

                this.fuzzyRules = this.treatment.ruleModel.rules;

                this.dssRules = this.treatment.ruleModel.rules[0].orRules;

                //this.dssRules = this.treatment.ruleModel.rules[0].orRules;
            }

        }, error => console.error(error));


    }



    addFuzzyRule() {

        var addRule = {} as FuzyRule;
        addRule.name = this.newfuzzyRule.name;
        addRule.fuzzy = true;
        addRule.orRules = [];
        this.dssRules = addRule.orRules;
        this.fuzzyRules.push(addRule);

    }
    addRule(): void {

       // this.dssRule.rules = [];
        //var addRule = {} as FuzyRule;
        //addRule.orRules = [];
        //addRule.fuzzy = true;
        //addRule.name = this.newRule.name;
        var addAndRule = {} as AndRule;
        addAndRule.name = this.newRule.name;
        addAndRule.andVariables = [];
        var fuzzyFunc = {} as FuzzyFunc;
        fuzzyFunc.name = this.newRule.name;
        fuzzyFunc.property = this.newRule.name;
        fuzzyFunc.not = false;
        addAndRule.andVariables.push(fuzzyFunc);
        //addRule.orRules.push((addAndRule));
        this.dssRules.push(addAndRule);
        
        
    };
    addAndRule(): void {

     //   this.dssRule.rules = [];
        var addRule = {} as FuzzyFunc;
        addRule.property = this.newAndRule.name;
        addRule.name = this.newAndRule.name;
        addRule.not = false;
        this.dssAndRules.push(addRule);
        


    };
    deleteRule(index: number): void {

        this.dssRules.splice(index, 1);// = this.dssAndRules.filter(item => item !== rule);
       
    };

    editRule(rule: AndRule): void {
        
        this.dssRule = rule;
        this.dssAndRules = rule.andVariables;
    };

    editFuzzyRule(rule: FuzyRule): void {


        this.dssRules = rule.orRules;
        // this.dssRule = rule;
        // this.dssAndRules = rule.andVariables;
    };


    deleteFuzzyRule(index: number): void {
        this.fuzzyRules.splice(index, 1);
    }

    deleteAndRule(index: number): void {

        this.dssAndRules.splice(index, 1);// = this.dssAndRules.filter(item => item !== rule);
        this.updateVariables();
    };

    updateVariables(): void {
        var self = this;
        self.variables = [];
        this.fuzzyRules.forEach(function(fuzzy) {

            fuzzy.orRules.forEach(function(rule) {

                rule.andVariables.forEach(function(r) {
                    var variable = {} as FuzzyVariable;
                    variable.name = r.name;
                    variable.code = r.property;
                    self.variables.push(variable);
                });

            });
        });

    }

    addPreCond(): void {

        if (!this.treatment.preconditions)
            this.treatment.preconditions = [];

        this.treatment.preconditions.push({ code: this.preconditionCode } as FuzzyVariable);

    }

    removePreCond(index: number): void {
        
        this.treatment.preconditions.splice(index, 1);// = this.dssAndRules.filter(item => item !== rule);
    }

    saveRule(): void {


        var url = this.baseUrl + 'api/v1/dssconfig';
        var self = this;
        this.updateVariables();
        this.treatment.ruleModel = {} as FuzyCollection;
       
        //var fRule = {} as FuzyRule;
        //fRule.fuzzy = true;
        //fRule.name = this.treatment.name;
        //fRule.orRules = (this.dssRules);
        this.treatment.ruleModel.rules = this.fuzzyRules;
        this.treatment.ruleModel.variables = this.variables;
        this.treatment.id = this.modelId;
        console.log(this.treatment);
        var request = this.httpClient.post(url, this.treatment).subscribe(
            res => {
                console.log(res);
                alert('DSS Changes saved');
                //   this.dssOutput = res.json() as DSSOutputValue[];
            },
            err => {
                console.log("Error occured");
            }
        );
        
        
    };


}
interface TreatmentClassifier {
    id: string;
    name: string;
    code: string;
    description: string;
    codeNamespace: string;
    replacementCode: string;
    replacementCodeNamespace: string;
    summary: string;
    option: string;
    treatmentSuggestion:boolean,
    ruleModel: FuzyCollection;
    preconditionCode:string;
    preconditions: FuzzyVariable[];
}

interface FuzyCollection {
    id:string;
    variables: FuzzyVariable[];    
    rules: FuzyRule[];
}


interface FuzyRule {
    name: string;
    code: string;
    fuzzy: boolean;
    orRules: AndRule[];
}


interface AndRule {
    
    name: string;
    code: string;
    property: string;
    andVariables: FuzzyFunc[];

}
interface FuzzyFunc {
    code: string;
    name: string;
    property: string;
    not: boolean;

}
interface FuzzyVariable {
    code: string;
    name: string;
    
}
