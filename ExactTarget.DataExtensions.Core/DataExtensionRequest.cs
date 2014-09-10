using System.Collections.Generic;

namespace ExactTarget.DataExtensions.Core
{
    public class DataExtensionRequest
    {
        public DataExtensionRequest()
        {
            Fields = new Dictionary<string, DataExtensionRequestFieldType>();
        }
        public string TemplateObjectId { get; set; }
        public string ExternalKey { get; set; }
        public string Name { get; set; }
        public Dictionary<string, DataExtensionRequestFieldType> Fields { get; set; } 
    }

    public enum DataExtensionRequestFieldType
    {
        Text,
        Number,
        Date,
        Boolean,
        EmailAddress,
        Phone,
        Decimal,
        Locale
    }
}