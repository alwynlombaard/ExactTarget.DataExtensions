using System;
using System.Collections.Generic;
using System.Linq;
using ExactTarget.DataExtensions.Core;
using ExactTarget.DataExtensions.Core.Configuration;
using ExactTarget.DataExtensions.Core.Dto;
using ExactTarget.DataExtensions.Core.SoapApiClient;
using NUnit.Framework;

namespace ExactTarget.DataExtensions.Test.Integration
{
    [TestFixture]
    public class Test
    {
        private ExactTargetConfiguration _config;
        private string _externalKey;
        private ExactTargetApiClient _apiClient;
        private HashSet<Field> _fieldDefinitions;
        private DataExtensionClient _client;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _externalKey = "test-" + DateTime.Now.ToString("u");
            _config = new ExactTargetConfiguration
            {
                EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",
                ApiUserName = "",
                ApiPassword = "",
                ClientId = 6269489
            };
            _apiClient = new ExactTargetApiClient(_config);
            _fieldDefinitions = new HashSet<Field>
            {
                new Field{Name =  "IdField", FieldType = FieldType.Text, IsPrimaryKey = true, MaxLength = Guid.Empty.ToString().Length},
                new Field{Name =  "BooleanField", FieldType = FieldType.Boolean},
                new Field{Name =  "DateField", FieldType = FieldType.Date},
                new Field{Name =  "TextField", FieldType = FieldType.Text, MaxLength = 300},
                new Field{Name =  "EmailAddressField", FieldType = FieldType.EmailAddress}
            };
            _client = new DataExtensionClient(_apiClient);
            var createRequest = new DataExtensionRequest
            {
                ExternalKey = _externalKey,
                Fields = _fieldDefinitions,
                Name = "Name:" + _externalKey
            };

            Assert.That(_client.DoesDataExtensionExist(_externalKey), Is.False);
            Assert.DoesNotThrow(() => _client.CreateDataExtension(createRequest));
            Assert.That(_client.DoesDataExtensionExist(_externalKey), Is.True);

        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Assert.DoesNotThrow(() => _client.Delete(_externalKey));
            Assert.That(_client.DoesDataExtensionExist(_externalKey), Is.False);
        }
        
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void IntegrationTest()
        {
           
            var dataExtensionDef = _client.RetrieveDefinition(_externalKey);
            Assert.That(dataExtensionDef.ExternalKey, Is.EqualTo(_externalKey));

            var fields = _client.GetFields(_externalKey).ToArray();
            Assert.That(fields.Count(), Is.AtLeast(_fieldDefinitions.Count));

            Assert.DoesNotThrow(() =>  _client.InsertOrUpdate(_externalKey, GenerateTestRecord(_fieldDefinitions)));

            var record = GenerateTestRecord(_fieldDefinitions);
            Assert.DoesNotThrow(() => _client.InsertOrUpdate(_externalKey, record));

            var records = _client.RetrieveRecords(_externalKey, "IdField", record.Values["IdField"]);
            Assert.That(records.Count(), Is.EqualTo(1));

            records = _client.RetrieveRecords(_externalKey).ToArray();
            Assert.That(records.Count(), Is.EqualTo(2));
            
            Assert.DoesNotThrow(() => _client.InsertOrUpdate(_externalKey,
                new List<DataExtensionRecordDto>{
                       GenerateTestRecord(_fieldDefinitions),
                       GenerateTestRecord(_fieldDefinitions),
                }));

            records = _client.RetrieveRecords(_externalKey).ToArray();
            Assert.That(records.Count(), Is.EqualTo(4));

            var batch = new List<DataExtensionRecordDto>();
            for (var i = 0; i < 200; i++)
            {
                batch.Add(GenerateTestRecord(_fieldDefinitions));
            }

            Assert.DoesNotThrow(() => _client.InsertOrUpdate(_externalKey, batch));
            records = _client.RetrieveRecords(_externalKey);
            Assert.That(records.Count(), Is.EqualTo(204));

          

        }

        private static DataExtensionRecordDto GenerateTestRecord(IEnumerable<Field> fields)
        {
            var record = new DataExtensionRecordDto();
            var fieldValues = new Dictionary<string, string>();
            foreach (var field in fields)
            {
                switch (field.FieldType)
                {
                    case FieldType.Boolean:
                        fieldValues.Add(field.Name, "true");
                        break;
                    case FieldType.Date:
                        fieldValues.Add(field.Name, DateTime.Now.AddDays(DateTime.Now.Millisecond % 500).ToString("u"));
                        break;
                    case FieldType.Text:
                        fieldValues.Add(field.Name, Guid.NewGuid().ToString());
                        break;
                    case FieldType.EmailAddress:
                        fieldValues.Add(field.Name, "name@domain.uri");
                        break;
                }
            }
            record.Values = fieldValues;
            return record;
        }
    }
}
