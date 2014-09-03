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
                EndPoint = "",
                ApiUserName = "",
                ApiPassword = "",
                ClientId = 122
                
            };
        }

        [Test]
        public void TestS()
        {
            var client = new DataExtensionClient(_config, null, null);

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

            client.CreateDataExtensions(dataExtensions);
        }
    }
}
