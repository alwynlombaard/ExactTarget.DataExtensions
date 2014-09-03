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
        private readonly SoapClient _client;
        private readonly SharedCoreRequestClient _sharedCoreRequestClient;

        public DataExtensionClient(IExactTargetConfiguration config)
        {
            _config = config;
            _client = SoapClientFactory.Manufacture(config);
            _sharedCoreRequestClient = new SharedCoreRequestClient(config);
        }

        public void CreateDataExtensions(IEnumerable<DataExtensionRequest> dataExtensions)
        {
        }

        public void CreateDataExtension(DataExtensionRequest dataExtension)
        {
            if (dataExtension == null)
            {
                return;
            }

            var de = new DataExtension
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                Name = dataExtension.Name,
                CustomerKey = dataExtension.ExternalKey,
                Template =  string.IsNullOrEmpty(dataExtension.TemplateObjectId) 
                    ? null
                    : new DataExtensionTemplate { ObjectID = dataExtension.TemplateObjectId },
                Fields = dataExtension.Fields.Select(field => new DataExtensionField
                {
                    Name = field,
                    FieldType = DataExtensionFieldType.Text,
                    FieldTypeSpecified = true,
                }).ToArray(),
            };

            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { de }, out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
        }

        public bool DoesDataExtensionExist(string externalKey)
        {
            return _sharedCoreRequestClient.DoesObjectExist("CustomerKey", externalKey, "DataExtension");
        }

        public string RetrieveTriggeredSendDataExtensionTemplateObjectId()
        {
            return _sharedCoreRequestClient.RetrieveObjectId("Name", "TriggeredSendDataExtension", "DataExtensionTemplate");
        }
    }
}
