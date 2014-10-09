using System;
using ExactTarget.DataExtensions.Core.ExactTargetApi;

namespace ExactTarget.DataExtensions.Core.Dto
{
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
                    FieldType = Enum.TryParse(field.FieldType.ToString(), true, out type) ? type : FieldType.Text,
                    MaxLength = field.MaxLength
                    
                };
        }

        public string Name { get; set; }
        public int Ordinal { get; set; }
        public bool IsPrimaryKey { get; set; }
        public int? MaxLength { get; set; }
        public FieldType FieldType { get; set; }

    }
}