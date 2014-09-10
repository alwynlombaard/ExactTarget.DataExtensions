using System.Collections.Generic;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core
{
    public class DataExtensionRecordDto
    {
        public DataExtensionRecordDto()
        {
            Values = new Dictionary<string,string>();
        }
        public Dictionary<string, string> Values { get; set; }

        public static DataExtensionRecordDto From(DataExtensionObject dataExtensionObject)
        {
            var dto = new DataExtensionRecordDto();
            if (dataExtensionObject == null)
            {
                return dto;
            }

            foreach (var value in dataExtensionObject.Keys)
            {
                dto.Values.Add(value.Name, value.Value);
            }
            return dto;
        }
    }
}