using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Codery.TextCount.PropertyValueConverters
{
    /// <summary>
    ///     Converts text count wrapped property value.
    /// </summary>
    public class TextCountConverter : PropertyValueConverterBase, IPropertyValueConverterMeta
    {
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("Codery.TextCount");
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType,
            PropertyCacheValue cacheValue)
        {
            PropertyCacheLevel returnLevel;

            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            if (wrappedPropertyType != null)
                switch (cacheValue)
                {
                    case PropertyCacheValue.Object:
                        returnLevel = wrappedPropertyType.ObjectCacheLevel;
                        break;
                    case PropertyCacheValue.Source:
                        returnLevel = wrappedPropertyType.SourceCacheLevel;
                        break;
                    case PropertyCacheValue.XPath:
                        returnLevel = wrappedPropertyType.XPathCacheLevel;
                        break;
                    case PropertyCacheValue.All:
                        returnLevel = PropertyCacheLevel.None;
                        break;
                    default:
                        returnLevel = PropertyCacheLevel.None;
                        break;
                }
            else
                returnLevel = PropertyCacheLevel.None;

            return returnLevel;
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            return wrappedPropertyType != null ? wrappedPropertyType.ClrType : typeof(string);
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            return wrappedPropertyType != null ? wrappedPropertyType.ConvertDataToSource(source, preview) : source;
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            return wrappedPropertyType != null ? wrappedPropertyType.ConvertSourceToObject(source, preview) : source;
        }

        public override object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview)
        {
            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            return wrappedPropertyType != null ? wrappedPropertyType.ConvertSourceToXPath(source, preview) : source;
        }

        #region Wrapper methods

        private readonly Dictionary<int, PublishedPropertyType> _wrappedPropertyTypes =
            new Dictionary<int, PublishedPropertyType>();

        private readonly object _wrappedPropertyTypesLock = new object();

        /// <summary>
        ///     Gets property type of wrapped data type.
        /// </summary>
        /// <param name="wrapperPropertyType"></param>
        /// <returns></returns>
        private PublishedPropertyType GetWrappedPropertyType(PublishedPropertyType wrapperPropertyType)
        {
            lock (_wrappedPropertyTypesLock)
            {
                // check if wrapped property already exists in list
                if (_wrappedPropertyTypes.ContainsKey(wrapperPropertyType.DataTypeId))
                    return _wrappedPropertyTypes[wrapperPropertyType.DataTypeId];

                PublishedPropertyType propertyType = null;

                var wrappedDataTypeId = GetWrappedDataTypeId(wrapperPropertyType.DataTypeId);

                if (wrappedDataTypeId != 0)
                    propertyType = GetPublishedPropertyType(wrappedDataTypeId, wrapperPropertyType.ContentType);

                // cache the current object for future calls
                _wrappedPropertyTypes.Add(wrapperPropertyType.DataTypeId, propertyType);

                return propertyType;
            }
        }

        /// <summary>
        ///     Gets the wrapped data type ID from the current data type's prevalues.
        /// </summary>
        /// <param name="wrapperDataTypeId"></param>
        /// <returns></returns>
        private static int GetWrappedDataTypeId(int wrapperDataTypeId)
        {
            var dataTypeId = 0;

            var preValues =
                ApplicationContext.Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(wrapperDataTypeId);

            if (preValues == null)
                return dataTypeId;

            var dict = preValues.PreValuesAsDictionary;

            if (!dict.ContainsKey("dataType"))
                return dataTypeId;

            if (!int.TryParse(dict["dataType"].Value, out dataTypeId))
                dataTypeId = 0;

            return dataTypeId;
        }

        /// <summary>
        ///     Get published property type of the wrapped data type.
        /// </summary>
        /// <param name="dataTypeId"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private PublishedPropertyType GetPublishedPropertyType(int dataTypeId, PublishedContentType contentType)
        {
            var dataTypeDefinition =
                ApplicationContext.Current.Services.DataTypeService.GetDataTypeDefinitionById(dataTypeId);

            if ((dataTypeDefinition == null) || (contentType == null))
                return null;

            var propertyType = new PropertyType(dataTypeDefinition);

            var publishedPropertyType = new PublishedPropertyType(contentType, propertyType);

            return publishedPropertyType;
        }

        #endregion
    }
}