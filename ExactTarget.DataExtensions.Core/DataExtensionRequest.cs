using System.Collections.Generic;

namespace ExactTarget.DataExtensions.Core
{
    public class DataExtensionRequest
    {
        public DataExtensionRequest()
        {
            Fields = new Dictionary<string, FieldType>();
        }
        public string TemplateObjectId { get; set; }
        public string ExternalKey { get; set; }
        public string Name { get; set; }
        public Dictionary<string, FieldType> Fields { get; set; } 
    }

    public enum FieldType
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