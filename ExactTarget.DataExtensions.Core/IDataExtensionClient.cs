using System.Collections.Generic;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core
{
    public interface IDataExtensionClient
    {
        void CreateDataExtension(DataExtensionRequest request);
        IEnumerable<ResultError> CreateDataExtensions(IEnumerable<DataExtensionRequest> dataExtensions);
        DataExtensionDto RetrieveDefinition(string externalKey);
        bool DoesDataExtensionExist(string externalKey);

        string RetrieveObjectId(string externalKey);
        string RetrieveTriggeredSendDataExtensionTemplateObjectId();

        IEnumerable<DataExtensionField> GetFields(string externalKey);
        void Insert(string externalKey, Dictionary<string, string> values);
        IEnumerable<DataExtensionRecordDto> RetrieveRecords(string externalKey, string fieldName, string fieldValue);
    }
}