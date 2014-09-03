using System.Collections.Generic;

namespace ExactTarget.DataExtensions.Core
{
    public interface IDataExtensionClient
    {
        void CreateDataExtension(DataExtensionRequest request);

        IEnumerable<ResultError> CreateDataExtensions(IEnumerable<DataExtensionRequest> dataExtensions);

        bool DoesDataExtensionExist(string externalKey);
        
        string RetrieveTriggeredSendDataExtensionTemplateObjectId();
    }
}