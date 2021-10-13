using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Task = System.Threading.Tasks.Task;

namespace PRIME.Core.Services.FHIR
{
    public class LocalFhirConditionRepository : BaseFhirConditionRepository
    {
        private readonly string _bundleJson;
        public LocalFhirConditionRepository(string bundleJson)
        {
            _bundleJson = bundleJson;
        }

        public override async Task Init(string id)
        {

            if (string.IsNullOrEmpty(_bundleJson))
                return;


            FhirJsonParser parser=new FhirJsonParser();

            var bundle= parser.Parse<Bundle>(_bundleJson.Trim(new char[]{' ','\n'}));

            SetBundle(bundle);

        }
    }
}