# PRIME.DSS
## Introduction
The project is the DSS engine for the PRIME project which is funded by ESPA
The PRIMS aims to enable physicians in the better management of the disease. This includes four main arms of the PRIME which are the
1) test using sensors to access motor symptoms
2) a decision support tool  based on PD treatment guidelines
3) a drug to drug and drug to gene interaction checker
4) a physician dashboard to more easily monitor patient status and disease progress.

The specific repository provides the code and description for the Decision support and D2D and D2G checker.

## DSS
The main functionality of the PRIME DSS is to collect information from EMRs such as NetMed360 or any other FHIR compatible sources and provide decision support recommendations for  patients with Parkinson's disease. The DSS workflow has three main steps
1) Acquire raw FHIR resources (observations/patient information). A FHIR Proxy configuration has been developed in order to enable the collection of FHIR resources from different sources (typically EMR). The resources are collected in a temporary patient specific repository which is used only to evaluate the DSS for the current request.
2) From raw EMR of 3rd party FHIR resources repository aggregate information and translate the information to the input expected by the DSS. In the current version there is an Aggregation Model which can be used to define different type of aggregations.
3) A number of DSS inference methods (Naive bayes or simple rules) are applied taking as input the aggregated information which resulted from the previous step. Each DSS engine has a suggestion as main outcome and a suggestion card, similar to Smart on FHIR [cards](https://cds-hooks.org/)). 

In the current version a UI has been implemented in ASP .NET MVC Code and Angular.js in order to test the API. Using the UI all main functionalities can be tested. 
In the next version a [FHIR compatible](https://www.hl7.org/fhir/clinicalreasoning-cds-on-fhir.html) CDSS functionality will be implemented.


## Prescription
The prescription API of the PRIME includes the following functionalities:

1) Identify RX CUI code based on drug name. Users may enter medications in their commercial name or by the substance. Therefore, before checking any drug-drug interaction the standard name for the medication is needed. To do so the DSS API extract the RXCUI id from the RXNAV api. RxNav is a browser for several drug information sources, including RxNorm and RxTerms. RxNav finds drugs in RxNorm from the names and codes in its constituent vocabularies. Getting the  a structured EMR 
2) Check for Drug-Drug Interactions. Using the RXCUIs of the current prescription drugs the [RXNAV api](https://rxnav.nlm.nih.gov/InteractionAPIs.html) for checking drug to drug interactions. Currently, the API uses two sources for its interaction information - ONCHigh and DrugBank. ONCHigh is a list of high-priority drug-drug interactions derived by a panel of experts and contained in a JAMIA article. ONCHigh is updated using CredibleMeds with drugs that have a known risk of Torsades de Pointes (TdP). DrugBank contains the drug-drug interactions contained in the DrugBank database. DrugBank does not contain severity level information for the drug-drug interactions.
3) Check for Drug-Gene Interactions. For Drug-Drug interaction the [DGI](http://www.dgidb.org). DGIdb attempts to organize the druggable genome under two main classes. The first class includes genes with known drug interactions. Such drug-gene interactions are useful for the case where a researcher has a list of candidate genes predicted to be activated in disease, and wishes to identify drugs that might inhibit or otherwise modulate those genes. The second class includes genes that are 'potentially' druggable according to their membership in gene categories associated with druggability (e.g., kinases). Membership in these categories is useful for prioritizing a list of genes according to their potential for drug development. The former are established interactions between genes and drugs, based largely on literature mining and obtained from existing publicly available reviews and databases. These can come from either gene- or drug-centric database models and are not limited by functional category or drug modality. The latter represent genes that have properties making them suitable for drug targeting but may not currently have a drug targeting them. There are various ways to define this class of potentially 'druggable' genes. We drew from several existing efforts and local domain knowledge to define categories that are most relevant to druggability. These categories tend to be biased towards genes that are amenable to targeting by small molecules such as kinases, ion channels, etc. For both classes of druggable genes, sources were manually curated and semi-automatically imported. The database can be accessed programmatically or through a web-based interface or API at dgidb.org. Search results can be filtered and ranked in multiple ways and are easily exported for further analysis or visualization. We believe DGIdb represents a powerful resource for hypothesis generation. DGIdb may also facilitate prioritization of gene-level events for review by clinical experts and ultimately aid in treatment decision-making.




