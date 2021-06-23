using Newtonsoft.Json;
using System;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Models.ContentEditing;

namespace Codery.TextCount.Configuration
{
    internal sealed class TextCountConfiguration
    {
        [ConfigurationField("dataType", "Data Type", DataTypePicker.Constants.ViewPath, Description = "Choose which data type to apply the text count to. " +
                              "Note that the count will be applied to all textstring and textarea fields in the wrapped data type.")]
        public object DataType { get; set; }

        /// <summary>
        ///     Returns data type ID if old integer value is stored.
        /// </summary>
        [JsonProperty("dataTypeId")]
        public int? DataTypeId
        {
            get
            {
                if (int.TryParse(DataType?.ToString(), out var dataTypeId))
                {
                    return dataTypeId;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///     Returns data type key if new GUID value is stored.
        /// </summary>
        [JsonProperty("dataTypeKey")]
        public Guid? DataTypeKey
        {
            get
            {
                if (Guid.TryParse(DataType?.ToString(), out var dataTypeKey))
                {
                    return dataTypeKey;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///     Returns the wrapped data type used to render the property editor.
        /// </summary>
        [JsonProperty("wrappedProperty")]
        public ContentPropertyDisplay WrappedProperty
        {
            get
            {
                var wrappedDataType = (DataTypeKey.HasValue ?
                    Current.Services.DataTypeService.GetDataType(DataTypeKey.Value) :
                    Current.Services.DataTypeService.GetDataType(DataTypeId.GetValueOrDefault())
                );

                if (wrappedDataType == null)
                {
                    return null;
                }

                var configuration = wrappedDataType.Configuration;
                var editor = Current.PropertyEditors[wrappedDataType.EditorAlias];

                return new ContentPropertyDisplay()
                {
                    Editor = wrappedDataType.EditorAlias,
                    Validation = new PropertyTypeValidation(),
                    View = editor.GetValueEditor().View,
                    Config = editor.GetConfigurationEditor().ToConfigurationEditor(configuration)
                };
            }
        }

        [ConfigurationField("countType", "Characters or words?", "/App_Plugins/Codery.TextCount/configuration/count-type.html", Description = "Choose whether to count characters or words.")]
        public string CountType { get; set; }

        [ConfigurationField("limit", "Limit", "number", Description = "Number of characters or words which are the limit. Leave empty if there is no limit.")]
        public int Limit { get; set; }

        [ConfigurationField("limitType", "Limit Type", "/App_Plugins/Codery.TextCount/configuration/limit-type.html", Description = "Choose what happens when the limit is reached.")]
        public string LimitType { get; set; }
    }
}