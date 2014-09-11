using System;
using System.Collections.Generic;
using System.Linq;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core
{
    public class DataExtensionClient : IDataExtensionClient
    {
        private readonly IExactTargetApiClient _client;

        public DataExtensionClient(IExactTargetApiClient client)
        {
            _client = client;
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
            _client.Create(de);
        }

        public void Delete(string externalKey)
        {
            var de = new DataExtension
            {
                Client = _client.Config.ClientId.HasValue 
                    ? new ClientID { ID = _client.Config.ClientId.Value, IDSpecified = true } 
                    : null,
                CustomerKey = externalKey
            };
            _client.Delete(de);
        }

        public bool DoesDataExtensionExist(string externalKey)
        {
            return _client.DoesObjectExist("CustomerKey", externalKey, "DataExtension");
        }

        public DataExtensionDto RetrieveDefinition(string externalKey)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _client.Config.ClientId.HasValue
                            ? new[] { new ClientID { ID = _client.Config.ClientId.Value, IDSpecified = true } }
                            : null,
                ObjectType = "DataExtension",
                Properties = GetRetrievableProperties("DataExtension").ToArray(),
                Filter = new SimpleFilterPart
                {
                    Property = "CustomerKey",
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { externalKey }
                }
            };

            var de = (DataExtension)_client.Retrieve(request).FirstOrDefault();
            return MapToDto(de);
        }

        public IEnumerable<Field> GetFields(string externalKey)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _client.Config.ClientId.HasValue
                            ? new[] { new ClientID { ID = _client.Config.ClientId.Value, IDSpecified = true } }
                            : null,
                ObjectType = "DataExtensionField",
                Properties = GetRetrievableProperties("DataExtensionField").ToArray(),
                Filter = new SimpleFilterPart
                {
                    Property = "CustomerKey",
                    SimpleOperator = SimpleOperators.like,
                    Value = new[] { externalKey }
                }
            };

            return _client.Retrieve(request)
                .Cast<DataExtensionField>()
                .Select(Field.MapFrom)
                .OrderBy(f => f.Ordinal);
        }

        public string RetrieveTriggeredSendDataExtensionTemplateObjectId()
        {
            return _client.RetrieveObjectId("Name", "TriggeredSendDataExtension", "DataExtensionTemplate");
        }

        public string RetrieveObjectId(string externalKey)
        {
            return _client.RetrieveObjectId("CustomerKey", externalKey, "DataExtension");
        }

        public void Insert(string externalKey, Dictionary<string, string> values)
        {
            var apiProperties = new List<APIProperty>();
            foreach (var key in values.Keys)
            {
                apiProperties.Add(new APIProperty
                {
                  Name  = key,
                  Value = values[key]
                });
            }

            APIObject[] apiObjects =
            {
                new DataExtensionObject
                {
                    Client = _client.Config.ClientId.HasValue ? new ClientID{ID = _client.Config.ClientId.Value, IDSpecified = true} : null,
                    Properties = apiProperties.ToArray(),
                    CustomerKey = externalKey,
                }
            };
            _client.Create(apiObjects);
        }

        private IEnumerable<string> GetRetrievableProperties(string objectType)
        {
            var results = _client.Describe(new[] 
            {
                new ObjectDefinitionRequest
                {
                    Client = _client.Config.ClientId.HasValue ? new ClientID{ID = _client.Config.ClientId.Value, IDSpecified = true} : null,
                    ObjectType =  objectType //"DataExtension"
                }  
            });
            var firstOrDefault = results.FirstOrDefault();
            if (firstOrDefault != null)
            {
                return results.Any() 
                    ? firstOrDefault.Properties
                        .Where(p => p.IsRetrievable && !p.Name.Equals("DataRetentionPeriod", StringComparison.InvariantCultureIgnoreCase))
                        .Select(p => p.Name) 
                    : Enumerable.Empty<string>();
            }
            return Enumerable.Empty<string>();
        }

        public IEnumerable<DataExtensionRecordDto> RetrieveRecords(string externalKey, string fieldName, string fieldValue)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _client.Config.ClientId.HasValue
                    ? new[] {new ClientID {ID = _client.Config.ClientId.Value, IDSpecified = true}}
                    : null,
                ObjectType = "DataExtensionObject[" + externalKey + "]",
                Properties = GetFields(externalKey).Select(f => f.Name).ToArray(),
                Filter = new SimpleFilterPart
                {
                    Property = fieldName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] {fieldValue}
                }
            };
            var results = _client.Retrieve(request);
            return results.Cast<DataExtensionObject>()
                .Select(DataExtensionRecordDto.From);
        }

        private DataExtension MapFrom(DataExtensionRequest request)
        {
            if (request == null)
            {
                return null;
            }

            DataExtensionFieldType etFieldType;
            return new DataExtension
            {
                Client = _client.Config.ClientId.HasValue ? new ClientID { ID = _client.Config.ClientId.Value, IDSpecified = true } : null,
                Name = request.Name,
                CustomerKey = request.ExternalKey,
                Template = string.IsNullOrEmpty(request.TemplateObjectId)
                    ? null
                    : new DataExtensionTemplate { ObjectID = request.TemplateObjectId },
                Fields = request.Fields.Select(field => new DataExtensionField
                {
                    Name = field.Key,
                    FieldType = Enum.TryParse(field.Value.ToString(), true, out etFieldType)  ? etFieldType :  DataExtensionFieldType.Text,
                    FieldTypeSpecified = true,
                }).ToArray(),
            };
        }

        private static DataExtensionDto MapToDto(DataExtension dataExtension)
        {
            return dataExtension != null 
                ? new DataExtensionDto
                    {
                        ExternalKey = dataExtension.CustomerKey,
                        Name = dataExtension.Name,
                    }
                : new DataExtensionDto();
        }
    }
}
