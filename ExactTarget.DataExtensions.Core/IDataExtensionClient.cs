using System;
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

        IEnumerable<Field> GetFields(string externalKey);
        void Insert(string externalKey, Dictionary<string, string> values);
        IEnumerable<DataExtensionRecordDto> RetrieveRecords(string externalKey, string fieldName, string fieldValue);
    }

    public class Field
    {
        public static Field MapFrom(DataExtensionField field)
        {
            FieldType type;
            return field == null
                ? new Field()
                : new Field
                {
                    Name = field.Name,
                    IsPrimaryKey = field.IsPrimaryKey,
                    Ordinal = field.Ordinal,
                    FieldType = Enum.TryParse(field.FieldType.ToString(), true, out type) ? type : FieldType.Text
                };
        }

        public string Name { get; set; }
        public int Ordinal { get; set; }
        public bool IsPrimaryKey { get; set; }
        FieldType FieldType { get; set; }

    }
}