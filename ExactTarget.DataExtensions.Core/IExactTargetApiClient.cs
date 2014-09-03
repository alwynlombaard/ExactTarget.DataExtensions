using System.Linq;
using ExactTarget.DataExtensions.Core.Configuration;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core
{
    public interface IExactTargetApiClient
    {
       CreateResult Create(APIObject apiObject);
       CreateResult[] Create(APIObject[] apiObject);
       APIObject[] Retrieve(RetrieveRequest request);
    }

    public class ExactTargetApiClient : IExactTargetApiClient
    {
        private readonly SoapClient _client;

        public ExactTargetApiClient(IExactTargetConfiguration config)
        {
            _client = SoapClientFactory.Manufacture(config);
        }

        public CreateResult Create(APIObject apiObject)
        {
            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { apiObject }, out requestId, out status);
            return result.FirstOrDefault();
        }

        public CreateResult[] Create(APIObject[] apiObjects)
        {
            string requestId, status;
            var results = _client.Create(new CreateOptions(), apiObjects, out requestId, out status);
            return results;
        }

        public APIObject[] Retrieve(RetrieveRequest request)
        {
            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            return results;
        }
    }
}
