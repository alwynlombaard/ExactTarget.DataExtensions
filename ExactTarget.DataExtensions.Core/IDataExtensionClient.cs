using System.Collections.Generic;

namespace ExactTarget.DataExtensions.Core
{
    public interface IDataExtensionClient
    {
        void CreateDataExtension(DataExtensionRequest dataExtension);
        
        void CreateDataExtensions(IEnumerable<DataExtensionRequest> dataExtensions);

        bool DoesDataExtensionExist(string externalKey);
        
        string RetrieveTriggeredSendDataExtensionTemplateObjectId();
    }
}