using System.Collections.Generic;

namespace ExactTarget.DataExtensions.Core
{
    public class DataExtensionRequest
    {
        public string TemplateObjectId { get; set; }
        public string ExternalKey { get; set; }
        public string Name { get; set; }
        public HashSet<string> Fields { get; set; } 
    }
}