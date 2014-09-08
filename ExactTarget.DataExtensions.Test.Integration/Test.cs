using System.Collections.Generic;
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
                ClientId = 0
                
            };
        }
      
        [Test]
        public void TestS()
        {
            var apiClient = new ExactTargetApiClient(_config);

            var client = new DataExtensionClient(_config, apiClient);

            var dataExtensions = new List<DataExtensionRequest>
            {
                new DataExtensionRequest
                {
                    ExternalKey = "alwyn-1",
                    Fields = new HashSet<string> {"field 1", "field 2"},
                    Name = "alwyn name - 1"
                },
                new DataExtensionRequest
                {
                    ExternalKey = "alwyn-2",
                    Fields = new HashSet<string> {"field 1", "field 2"},
                    Name = "alwyn name - 2"
                },
                new DataExtensionRequest
                {
                    ExternalKey = "alwyn-3",
                    Fields = new HashSet<string> {"field 1", "field 2"},
                    Name = "alwyn name - 3"
                },
                new DataExtensionRequest
                {
                    ExternalKey = "alwyn-4",
                    Fields = new HashSet<string> {"field 1", "field 2"},
                    Name = "alwyn name - 4"
                }
            };

            foreach (var dataExtensionRequest in dataExtensions)
            {
                client.CreateDataExtension(dataExtensionRequest);
            }
            
            Assert.Fail();
        }
    }
}
