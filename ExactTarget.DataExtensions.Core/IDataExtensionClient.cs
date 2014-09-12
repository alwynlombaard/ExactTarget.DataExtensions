﻿using System.Collections.Generic;
using ExactTarget.DataExtensions.Core.Dto;
using ExactTarget.DataExtensions.Core.SoapApiClient;

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

        IEnumerable<Field> GetFields(string externalKey);
        void InsertOrUpdate(string externalKey, Dictionary<string, string> values);
        void InsertOrUpdate(string externalKey, IEnumerable<Dictionary<string, string>> values);
        IEnumerable<DataExtensionRecordDto> RetrieveRecords(string externalKey, string fieldName, string fieldValue);
    }
}