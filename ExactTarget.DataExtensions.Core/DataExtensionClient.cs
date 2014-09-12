﻿using System;
using System.Collections.Generic;
using System.Linq;
using ExactTarget.DataExtensions.Core.Dto;
using ExactTarget.DataExtensions.Core.ExactTargetApi;
using ExactTarget.DataExtensions.Core.SoapApiClient;

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

            return _client.Create(dataExtensions.ToArray());
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
            return DataExtensionDto.MapFrom(de);
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

        public void InsertOrUpdate(string externalKey, Dictionary<string, string> values)
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
            _client.Update(apiObjects);
        }

        public void InsertOrUpdate(string externalKey, IEnumerable<Dictionary<string, string>> values)
        {
            var apiObjects = new List<APIObject>();    
            foreach (var value in values)
            {
                var apiProperties = new List<APIProperty>();
                foreach (var key in value.Keys)
                {
                    apiProperties.Add(new APIProperty
                    {
                        Name = key,
                        Value = value[key]
                    });
                }
                apiObjects.Add(new DataExtensionObject
                {
                    Client = _client.Config.ClientId.HasValue ? new ClientID { ID = _client.Config.ClientId.Value, IDSpecified = true } : null,
                    Properties = apiProperties.ToArray(),
                    CustomerKey = externalKey,
                });
            }
            _client.Update(apiObjects.ToArray());
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

        public IEnumerable<DataExtensionRecordDto> RetrieveRecords(string externalKey, string fieldName = null, string fieldValue = null)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _client.Config.ClientId.HasValue
                    ? new[] {new ClientID {ID = _client.Config.ClientId.Value, IDSpecified = true}}
                    : null,
                ObjectType = "DataExtensionObject[" + externalKey + "]",
                Properties = GetFields(externalKey).Select(f => f.Name).ToArray(),
                Filter = !string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(fieldValue) 
                    ? new SimpleFilterPart
                        {
                            Property = fieldName,
                            SimpleOperator = SimpleOperators.@equals,
                            Value = new[] {fieldValue}
                        }
                    : null
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
            var de = new DataExtension
            {
                Client = _client.Config.ClientId.HasValue ? new ClientID { ID = _client.Config.ClientId.Value, IDSpecified = true } : null,
                Name = request.Name,
                CustomerKey = request.ExternalKey,
                Template = string.IsNullOrEmpty(request.TemplateObjectId)
                    ? null
                    : new DataExtensionTemplate { ObjectID = request.TemplateObjectId },
                Fields = request.Fields.Select(field => new DataExtensionField
                {
                    Name = field.Name,
                    FieldType = Enum.TryParse(field.FieldType.ToString(), true, out etFieldType)  ? etFieldType :  DataExtensionFieldType.Text,
                    FieldTypeSpecified = true,
                }).ToArray(),
            };

            if (de.Fields.Any())
            {
                if (de.Fields.First().FieldType == DataExtensionFieldType.Text)
                {
                    de.Fields.First().MaxLength = Guid.Empty.ToString().Length;
                    de.Fields.First().MaxLengthSpecified = true;
                }
                de.Fields.First().IsRequired = true;
                de.Fields.First().IsRequiredSpecified = true;
                de.Fields.First().IsPrimaryKey = true;
                de.Fields.First().IsPrimaryKeySpecified = true;
            }
            return de;
        }

       
    }
}
