using System.Xml.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PropertyEditors;

namespace Codery.TextCount.PropertyEditors
{
    public class TextCountValueEditorWrapper : PropertyValueEditorWrapper
    {
        private readonly PropertyValueEditor _wrapped;

        public TextCountValueEditorWrapper(PropertyValueEditor wrapped) : base(wrapped)
        {
            _wrapped = wrapped;
        }

        private PropertyValueEditor GetPropertyValueEditorWrapper(int dataTypeId, IDataTypeService dataTypeService)
        {
            var preValues = dataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId);

            return GetPropertyValueEditorWrapper(preValues, dataTypeService);
        }

        /// <summary>
        ///     Returns the property value editor for the wrapped data type.
        /// </summary>
        /// <param name="preValues"></param>
        /// <param name="dataTypeService"></param>
        /// <returns></returns>
        private PropertyValueEditor GetPropertyValueEditorWrapper(PreValueCollection preValues, IDataTypeService dataTypeService)
        {
            var preValuesDictionary = preValues?.FormatAsDictionary();

            int wrappedDataTypeId = 0;

            if (preValuesDictionary == null || !preValuesDictionary.ContainsKey("dataType") || !int.TryParse(preValuesDictionary["dataType"].Value, out wrappedDataTypeId))
            {
                return _wrapped;
            }

            var wrappedDataType = dataTypeService.GetDataTypeDefinitionById(wrappedDataTypeId);

            var wrappedValueEditor = PropertyEditorResolver.Current.GetByAlias(wrappedDataType?.PropertyEditorAlias)?.ValueEditor;

            return wrappedValueEditor ?? _wrapped;
        }

        public override object ConvertDbToEditor(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
        {
            return GetPropertyValueEditorWrapper(propertyType.DataTypeDefinitionId, dataTypeService).ConvertDbToEditor(property, propertyType, dataTypeService);
        }

        public override string ConvertDbToString(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
        {
            return GetPropertyValueEditorWrapper(propertyType.DataTypeDefinitionId, dataTypeService).ConvertDbToString(property, propertyType, dataTypeService);
        }

        public override XNode ConvertDbToXml(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
        {
            return GetPropertyValueEditorWrapper(propertyType.DataTypeDefinitionId, dataTypeService).ConvertDbToXml(property, propertyType, dataTypeService);
        }

        public override object ConvertEditorToDb(ContentPropertyData editorValue, object currentValue)
        {
            return GetPropertyValueEditorWrapper(editorValue.PreValues, ApplicationContext.Current.Services.DataTypeService).ConvertEditorToDb(editorValue, currentValue);
        }
    }
}