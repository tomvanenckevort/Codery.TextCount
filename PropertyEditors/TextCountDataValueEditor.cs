using Codery.TextCount.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Codery.TextCount.PropertyEditors
{
    internal sealed class TextCountDataValueEditor : DataValueEditor
    {
        private readonly IDataValueEditor _wrapped;
        private readonly IDataTypeService _dataTypeService;

        public TextCountDataValueEditor(DataEditorAttribute attribute, IDataValueEditor wrapped, IDataTypeService dataTypeService) : base(attribute)
        {
            _wrapped = wrapped;
            _dataTypeService = dataTypeService;
        }

        private IDataValueEditor GetPropertyValueEditor(int dataTypeId)
        {
            return GetPropertyValueEditor(dataTypeId, out _);
        }

        private IDataValueEditor GetPropertyValueEditor(int dataTypeId, out object wrappedConfiguration)
        {
            var dataType = _dataTypeService.GetDataType(dataTypeId);

            return GetPropertyValueEditor(dataType?.ConfigurationAs<TextCountConfiguration>(), out wrappedConfiguration);
        }

        /// <summary>
        ///     Returns the data value editor for the wrapped data type.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="wrappedConfiguration"></param>
        /// <returns></returns>
        private IDataValueEditor GetPropertyValueEditor(TextCountConfiguration configuration, out object wrappedConfiguration)
        {
            if (configuration == null || configuration.DataType == default)
            {
                wrappedConfiguration = null;

                return _wrapped;
            }

            var wrappedDataType = _dataTypeService.GetDataType(configuration.DataType);

            wrappedConfiguration = wrappedDataType?.Configuration;

            var wrappedValueEditor = wrappedDataType?.Editor?.GetValueEditor();

            return wrappedValueEditor ?? _wrapped;
        }

        public new IEnumerable<ValidationResult> Validate(object value, bool required, string format)
        {
            return GetPropertyValueEditor(null, out _).Validate(value, required, format);
        }

        public override object FromEditor(ContentPropertyData editorValue, object currentValue)
        {
            var propertyValueEditor = GetPropertyValueEditor(editorValue.DataTypeConfiguration as TextCountConfiguration, out object wrappedConfiguration);
            var wrappedEditorValue = new ContentPropertyData(editorValue.Value, wrappedConfiguration);

            return propertyValueEditor.FromEditor(wrappedEditorValue, currentValue);
        }

        public override object ToEditor(Property property, IDataTypeService dataTypeService, string culture = null, string segment = null)
        {
            return GetPropertyValueEditor(property.PropertyType.DataTypeId).ToEditor(property, dataTypeService, culture, segment);
        }

        public new IEnumerable<XElement> ConvertDbToXml(Property property, IDataTypeService dataTypeService, ILocalizationService localizationService, bool published)
        {
            return GetPropertyValueEditor(property.PropertyType.DataTypeId).ConvertDbToXml(property, dataTypeService, localizationService, published);
        }

        public new XNode ConvertDbToXml(PropertyType propertyType, object value, IDataTypeService dataTypeService)
        {
            return GetPropertyValueEditor(propertyType.DataTypeId).ConvertDbToXml(propertyType, value, dataTypeService);
        }

        public override string ConvertDbToString(PropertyType propertyType, object value, IDataTypeService dataTypeService)
        {
            return GetPropertyValueEditor(propertyType.DataTypeId).ConvertDbToString(propertyType, value, dataTypeService);
        }
    }
}