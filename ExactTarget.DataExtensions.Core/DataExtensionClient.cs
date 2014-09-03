using System.Collections.Generic;
using System.Linq;
using ExactTarget.DataExtensions.Core.Configuration;
using ExactTarget.DataExtensions.Core.Shared;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core
{
    public class DataExtensionClient : IDataExtensionClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly IExactTargetApiClient _client;
        private readonly ISharedCoreRequestClient _sharedCoreRequestClient;

        public DataExtensionClient(IExactTargetConfiguration config, IExactTargetApiClient client, ISharedCoreRequestClient sharedCoreRequestClient)
        {
            _config = config;
            _client = client;
            _sharedCoreRequestClient = sharedCoreRequestClient;
        }

        public IEnumerable<ResultError> CreateDataExtensions(IEnumerable<DataExtensionRequest> requests)
        {
            var dataExtensionRequests = requests as DataExtensionRequest[] ?? requests.ToArray();
            
            if (requests == null || !dataExtensionRequests.Any())
            {
                return Enumerable.Empty<ResultError>();
            }
            var dataExtensions = new List<APIObject>();

            foreach (var request in dataExtensionRequests)
            {
                var de = MapFrom(request);
                if (de != null)
                {
                    dataExtensions.Add(de);
                }
            }

            var result = _client.Create(dataExtensions.ToArray());

            return ExactTargetResultChecker.CheckResults(result);

        }

        public void CreateDataExtension(DataExtensionRequest request)
        {
            if (request == null)
            {
                return;
            }

            var de = MapFrom(request);

            var result = _client.Create(de);

            ExactTargetResultChecker.CheckResult(result); 
        }

        public bool DoesDataExtensionExist(string externalKey)
        {
            return _sharedCoreRequestClient.DoesObjectExist("CustomerKey", externalKey, "DataExtension");
        }

        public string RetrieveTriggeredSendDataExtensionTemplateObjectId()
        {
            return _sharedCoreRequestClient.RetrieveObjectId("Name", "TriggeredSendDataExtension", "DataExtensionTemplate");
        }

        private DataExtension MapFrom(DataExtensionRequest request)
        {
            if (request == null)
            {
                return null;
            }

            return new DataExtension
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                Name = request.Name,
                CustomerKey = request.ExternalKey,
                Template = string.IsNullOrEmpty(request.TemplateObjectId)
                    ? null
                    : new DataExtensionTemplate { ObjectID = request.TemplateObjectId },
                Fields = request.Fields.Select(field => new DataExtensionField
                {
                    Name = field,
                    FieldType = DataExtensionFieldType.Text,
                    FieldTypeSpecified = true,
                }).ToArray(),
            };
        }
    }
}
