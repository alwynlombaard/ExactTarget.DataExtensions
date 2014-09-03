using System.Linq;
using ExactTarget.DataExtensions.Core.Configuration;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core.Shared
{
    public class SharedCoreRequestClient : ISharedCoreRequestClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly IExactTargetApiClient _client;

        public SharedCoreRequestClient(IExactTargetConfiguration config, IExactTargetApiClient client)
        {
            _config = config;
            _client = client;
        }

        public bool DoesObjectExist(string propertyName, string value, string objectType)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                    ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                    : null,
                ObjectType = objectType,
                Properties = new[] { "Name", "ObjectID", "CustomerKey" },
                Filter = new SimpleFilterPart
                {
                    Property = propertyName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { value }
                }
            };

            var results = _client.Retrieve(request);

            return results != null && results.Any();
        }

        public string RetrieveObjectId(string propertyName, string value, string objectType)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                            ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                            : null,
                ObjectType = objectType,
                Properties = new[] { "Name", "ObjectID", "CustomerKey" },
                Filter = new SimpleFilterPart
                {
                    Property = propertyName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { value }
                }
            };

            var results = _client.Retrieve(request);

            if (results != null && results.Any())
            {
                return results.First().ObjectID;
            }

            return string.Empty;
        }
    }
}
