using System.Collections.Generic;
using ExactTarget.DataExtensions.Core.Configuration;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core.SoapApiClient
{
    public interface IExactTargetApiClient
    {
       IExactTargetConfiguration Config { get; }
       void Create(APIObject apiObject);
       void Update(APIObject apiObject);
       IEnumerable<ResultError> Create(APIObject[] apiObject);
       IEnumerable<ResultError> Update(APIObject[] apiObject);
       void Delete(APIObject apiObject);
       APIObject[] Retrieve(RetrieveRequest request);
       bool DoesObjectExist(string propertyName, string value, string objectType);
       string RetrieveObjectId(string propertyName, string value, string objectType);
       ObjectDefinition[] Describe(ObjectDefinitionRequest[] requests);
    }
}
