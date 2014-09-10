using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ExactTarget.DataExtensions.Core;
using ExactTarget.DataExtensions.Core.Configuration;
using ExactTarget.DataExtensions.Core.ExactTargetApi;
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
        public void Describe()
        {
            var apiClient = new ExactTargetApiClient(_config);
            var results = apiClient.Describe(new[] 
            {
                new ObjectDefinitionRequest
                {
                    Client = _config.ClientId.HasValue ? new ClientID(){ClientID1 = _config.ClientId.Value} : null,
                    ObjectType = "DataExtension"
                }  
            });
            var fields = results.FirstOrDefault()
                .Properties
                .Where(p => p.IsRetrievable)
                .Select(p => p);
                

            Assert.Fail();
        }

        [Test]
        public void Exist()
        {
            var dataExtensionClient = new DataExtensionClient(new ExactTargetApiClient(_config));
            Assert.That(dataExtensionClient.DoesDataExtensionExist("alwyn-1"), Is.True);
            Assert.That(dataExtensionClient.DoesDataExtensionExist("alwyn-dkjhgsdkfkhsdkjjh"), Is.False);
        }

        [Test]
        public void Create()
        {
            var externalKey = "alwyn-" + DateTime.Now.ToString("MM-HH-mm");
            var apiClient = new ExactTargetApiClient(_config);
            var client = new DataExtensionClient(apiClient);

            var createRequest = new DataExtensionRequest
            {
                ExternalKey = externalKey,
                Fields =
                    new Dictionary<string, DataExtensionRequestFieldType>()
                    {
                        {"field 1", DataExtensionRequestFieldType.Boolean},
                        {"field 2", DataExtensionRequestFieldType.Date},
                        {"Text", DataExtensionRequestFieldType.Text}
                    },
                Name = "Name:" + externalKey,
            };

            Assert.That(client.DoesDataExtensionExist(externalKey), Is.False);
            Assert.DoesNotThrow(() => client.CreateDataExtension(createRequest));
            Assert.That(client.DoesDataExtensionExist(externalKey), Is.True);

            var fields = client.GetFields(externalKey);
            Assert.That(fields.Count(), Is.EqualTo(3));

            Assert.DoesNotThrow(() =>  client.Insert(externalKey, new Dictionary<string, string>()
            {
                { "field 1", "true" },
                { "field 2", "12 Apr 2012" },
                { "Text", "Hello text" },
            }));

            Assert.DoesNotThrow(() => client.Insert(externalKey, new Dictionary<string, string>()
            {
                { "field 1", "false" },
                { "field 2", "13 Apr 2012" },
                { "Text", "Hello text 2" },
            }));

            var records = client.RetrieveRecords(externalKey, "field 1", "true");
            Assert.That(records.Count(), Is.EqualTo(1));


            //var objectId = client.RetrieveObjectId("alwyn-1");
            //var fields = client.GetFields("alwyn-13:13");
            //var extension = client.RetrieveDefinition("alwyn-1");
        }
    }
}
