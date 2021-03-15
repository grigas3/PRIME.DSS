# PRIME.DSS
## Introduction
This first project is the main backend for the architecture of the PRIME solution. The project has three main APIs

The CDSS API. This API is for reading and getting CDSS APIs.

The definition of the API can be found at [CDSS API](http:195.251.192.95/CDSS/API)

The Prescription API. With this API drug-drug interactions are checked with every prescription order


## Interoperability.
In order the API to be interoperable a FHIR version of the API has been implemented.
The implementation is based on the guidelines of [Clinical Quality Framework](https://wiki.hl7.org/Clinical_Quality_Framework).

A REST FHIR API was implementing supporting the Guidance model from the [FHIR specification](http://hl7.org/fhir/clinicalreasoning-module.html)
The implementation is based on the sample impelemtation probided by [CQF](https://github.com/cqframework/cds-on-fhir/tree/master/Src/net/CQF)


