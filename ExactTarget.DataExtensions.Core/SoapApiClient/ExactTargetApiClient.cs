﻿using System.Collections.Generic;
using System.Linq;
using ExactTarget.DataExtensions.Core.Configuration;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core.SoapApiClient
{
    public class ExactTargetApiClient : IExactTargetApiClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;

        public IExactTargetConfiguration Config {
            get { return _config; }
        }

        public ExactTargetApiClient(IExactTargetConfiguration config)
        {
            _config = config;
            _client = SoapClientFactory.Manufacture(config);
        }

        public void Delete(APIObject apiObject)
        {
            string requestId, status;
            var result = _client.Delete(new DeleteOptions(), new[] {apiObject}, out requestId, out status);
            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());
        }

        public void Create(APIObject apiObject)
        {
            string requestId, status;
            var result = _client.Create(new CreateOptions(), 
                new [] { apiObject }, 
                out requestId, 
                out status);
            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());
        }
        
        public void Update(APIObject apiObject)
        {
            string requestId, status;
            var result = _client.Update(new UpdateOptions
            {
                SaveOptions = new[] { new SaveOption { SaveAction = SaveAction.UpdateAdd, PropertyName = "DataExtensionObject" } }
            },
                new[] { apiObject },
                out requestId,
                out status);
            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());
        }

        public IEnumerable<ResultError> Create(APIObject[] apiObjects)
        {
            string requestId, status;
            var results = _client.Create(new CreateOptions(), 
                apiObjects, 
                out requestId, 
                out status);
            return ExactTargetResultChecker.CheckResults(results);
        }

        public IEnumerable<ResultError> Update(APIObject[] apiObjects)
        {
            string requestId, status;
            var results = _client.Update(new UpdateOptions
            {
                SaveOptions = new[] { new SaveOption { SaveAction = SaveAction.UpdateAdd, PropertyName = "DataExtensionObject" } }
            },
                apiObjects,
                out requestId,
                out status);
            return ExactTargetResultChecker.CheckResults(results);
        }

        public APIObject[] Retrieve(RetrieveRequest request)
        {
            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            return results;
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

            var results = Retrieve(request);

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

            var results = Retrieve(request);

            if (results != null && results.Any())
            {
                return results.First().ObjectID;
            }

            return string.Empty;
        }

        public ObjectDefinition[] Describe(ObjectDefinitionRequest[] requests)
        {
            string requestId;
            var results = _client.Describe(requests, out requestId);
            return results;
        }
    }
}