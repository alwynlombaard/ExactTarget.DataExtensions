using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core
{
    public class DataExtensionDto
    {
        public static DataExtensionDto MapFrom(DataExtension dataExtension)
        {
            return dataExtension != null
                ? new DataExtensionDto
                {
                    ExternalKey = dataExtension.CustomerKey,
                    Name = dataExtension.Name,
                }
                : new DataExtensionDto();
        }

        public string TemplateObjectId { get; set; }
        public string ExternalKey { get; set; }
        public string Name { get; set; }
    }
}