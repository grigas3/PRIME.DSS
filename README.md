# PRIME.DSS
## Introduction

PRIME aims to enable physicians in the better management of the disease. This includes five main components:
1) a decision support tool based on PD treatment guidelines and ontologies
2) a prescription support tool which includes drug to drug, drug to gene and drug to protiens interaction checkers
3) automatic identification of similar cases with machine learning
4) functional tests to assess motor symptoms, specifically gait and tremor, using sensors
5) a physician dashboard to more easily monitor patient status and disease progress and make treatment decisions 

The specific repository provides the code and description for the 1) Decision support engine and 2) the prescription tool and the D2D and D2G checker


## DSS
The main functionality of the PRIME DSS is to collect information from EMRs such as NetMed360 or any other FHIR compatible sources and provide decision support recommendations for  patients with Parkinson's disease. The DSS workflow has three main steps:
1) Acquire raw FHIR resources (observations/patient information). A FHIR Proxy configuration has been developed in order to enable the collection of FHIR resources from different sources (typically EMR). The resources are collected in a temporary patient specific repository which is used only to evaluate the DSS for the current request.
2) From raw EMR of 3rd party FHIR resources repository aggregate information and translate the information to the input expected by the DSS. In the current version there is an Aggregation Model which can be used to define different types of aggregations.
3) A number of DSS inference methods (Naive bayes or simple rules) are applied taking as input the aggregated information which resulted from the previous step. Each DSS engine has a suggestion as main outcome and a suggestion card, similar to Smart on FHIR [cards](https://cds-hooks.org/)). 

In the current version a UI has been implemented in ASP .NET MVC Code and Angular.js in order to test the API. Using the UI all main functionalities can be tested. 
In the next version the [FHIR compatible](https://www.hl7.org/fhir/clinicalreasoning-cds-on-fhir.html) CDSS functionality will be implemented.


## Prescription
The prescription API of the PRIME includes the following functionalities:
1) Identification of RX CUI code based on drug name. Users may enter medications in their commercial name or by the substance. Therefore, before checking any drug-drug interaction the standard name for the medication is needed. To do so the DSS API extracts the RXCUI id from the RXNAV api. RxNav is a browser for several drug information sources, including RxNorm and RxTerms. RxNav finds drugs in RxNorm from the names and codes in its constituent vocabularies.  
2) Check for Drug-Drug Interactions. Using the RXCUIs of the current prescription drugs the [RXNAV api](https://rxnav.nlm.nih.gov/InteractionAPIs.html) is exploited for checking drug to drug interactions. Currently, the API uses two sources for its interaction information - ONCHigh and DrugBank. ONCHigh is a list of high-priority drug-drug interactions derived by a panel of experts and contained in a JAMIA article. ONCHigh is updated using CredibleMeds with drugs that have a known risk of Torsades de Pointes (TdP). DrugBank contains the drug-drug interactions contained in the DrugBank database. DrugBank does not contain severity level information for the drug-drug interactions.
3) Check for Drug-Gene Interactions. For Drug-Drug interaction the [DGI](http://www.dgidb.org). DGIdb attempts to organize the druggable genome under two main classes. The first class includes genes with known drug interactions. Such drug-gene interactions are useful for the case where a researcher has a list of candidate genes predicted to be activated in disease, and wishes to identify drugs that might inhibit or otherwise modulate those genes. The second class includes genes that are 'potentially' druggable according to their membership in gene categories associated with druggability (e.g., kinases). Membership in these categories is useful for prioritizing a list of genes according to their potential for drug development. The former are established interactions between genes and drugs, based largely on literature mining and obtained from existing publicly available reviews and databases. In PRIME, DGIdb aims to facilitate prioritization of gene-level events for review by clinical experts and ultimately aid in treatment decision-making.




