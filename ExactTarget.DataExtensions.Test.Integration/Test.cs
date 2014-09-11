using System;
using System.Collections.Generic;
using System.Linq;
using ExactTarget.DataExtensions.Core;
using ExactTarget.DataExtensions.Core.Configuration;
using NUnit.Framework;

namespace ExactTarget.DataExtensions.Test.Integration
{
    [TestFixture]
    public class Test
    {
        private ExactTargetConfiguration _config;

        [SetUp]
        public void SetUp()
        {
            _config = new ExactTargetConfiguration
            {
                EndPoint = "https://webservice.s6.exacttarget.com/Service.asmx",
                ApiUserName = "",
                ApiPassword = "",
                ClientId = 6269489
                
            };
        }
     
        [Test]
        public void IntegrationTest()
        {
            var externalKey = "alwyn-" + DateTime.Now.ToString("MM-HH-mm");
            var apiClient = new ExactTargetApiClient(_config);
            var client = new DataExtensionClient(apiClient);

            var createRequest = new DataExtensionRequest
            {
                ExternalKey = externalKey,
                Fields =
                    new Dictionary<string, FieldType>
                    {
                        {"field 1", FieldType.Boolean},
                        {"field 2", FieldType.Date},
                        {"Text", FieldType.Text}
                    },
                Name = "Name:" + externalKey,
            };

            Assert.That(client.DoesDataExtensionExist(externalKey), Is.False);
            Assert.DoesNotThrow(() => client.CreateDataExtension(createRequest));
            Assert.That(client.DoesDataExtensionExist(externalKey), Is.True);

            var fields = client.GetFields(externalKey).ToArray();
            Assert.That(fields.Count(), Is.EqualTo(3));

            Assert.DoesNotThrow(() =>  client.Insert(externalKey, new Dictionary<string, string>
            {
                { "field 1", "true" },
                { "field 2", "12 Apr 2012" },
                { "Text", "Hello text" },
            }));

            Assert.DoesNotThrow(() => client.Insert(externalKey, new Dictionary<string, string>
            {
                { "field 1", "false" },
                { "field 2", "13 Apr 2012" },
                { "Text", "Hello text 2" },
            }));

            var records = client.RetrieveRecords(externalKey, "field 1", "true");
            Assert.That(records.Count(), Is.EqualTo(1));

            records = client.RetrieveRecords(externalKey).ToArray();
            Assert.That(records.Count(), Is.EqualTo(2));

            
            Assert.DoesNotThrow(() => client.Insert(externalKey,
                new List<Dictionary<string, string>>{
                    new Dictionary<string, string>
                        {
                            { "field 1", "false" },
                            { "field 2", "14 Apr 2012" },
                            { "Text", "Hello text 3" },
                        },
                        new Dictionary<string, string>
                        {
                            { "field 1", "true" },
                            { "field 2", "15 Apr 2012" },
                            { "Text", "Hello text 4" },
                        }}
                   ));

            records = client.RetrieveRecords(externalKey).ToArray();
            Assert.That(records.Count(), Is.EqualTo(4));

            var batch = new List<Dictionary<string, string>>();
            for (var i = 0; i < 200; i++)
            {
                var fieldValues = new Dictionary<string, string>();
                foreach (var field in fields)
                {
                    switch (field.FieldType)
                    {
                        case FieldType.Boolean:
                            fieldValues.Add(field.Name, "true");
                            break;
                        case FieldType.Date:
                             fieldValues.Add(field.Name, DateTime.Now.AddDays(i).ToString("u"));
                             break;
                        case FieldType.Text:
                             fieldValues.Add(field.Name, "Text value");
                             break;
                        case FieldType.EmailAddress:
                             fieldValues.Add(field.Name, "name@domain.uri");
                             break;
                    }
                }
                batch.Add(fieldValues);
            }

            Assert.DoesNotThrow(() => client.Insert(externalKey, batch));
            records = client.RetrieveRecords(externalKey);
            Assert.That(records.Count(), Is.EqualTo(204));

            Assert.DoesNotThrow(() => client.Delete(externalKey));
            Assert.That(client.DoesDataExtensionExist(externalKey), Is.False);

        }
    }
}
