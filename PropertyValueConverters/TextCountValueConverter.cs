using Codery.TextCount.Configuration;
using Codery.TextCount.PropertyEditors;
using System;
using System.Collections.Generic;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

#pragma warning disable CA1812
namespace Codery.TextCount.PropertyValueConverters
{
    /// <summary>
    ///     Converts text count wrapped property value.
    /// </summary>
    internal sealed class TextCountValueConverter : PropertyValueConverterBase
    {
        private readonly IDataTypeService _dataTypeService;

        public TextCountValueConverter(IDataTypeService dataTypeService)
        {
            _dataTypeService = dataTypeService;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals(TextCountEditor.EditorAlias, StringComparison.InvariantCulture);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType)
        {
            PropertyCacheLevel returnLevel;

            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            if (wrappedPropertyType != null)
            {
                returnLevel = wrappedPropertyType.CacheLevel;
            }
            else
            {
                returnLevel = PropertyCacheLevel.None;
            }

            return returnLevel;
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            return wrappedPropertyType?.ClrType ?? typeof(string);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            return wrappedPropertyType != null ? wrappedPropertyType.ConvertInterToObject(owner, referenceCacheLevel, inter, preview) : inter;
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            return wrappedPropertyType != null ? wrappedPropertyType.ConvertSourceToInter(owner, source, preview) : source;
        }

        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var wrappedPropertyType = GetWrappedPropertyType(propertyType);

            return wrappedPropertyType != null ? wrappedPropertyType.ConvertInterToXPath(owner, referenceCacheLevel, inter, preview) : inter;
        }

        #region Wrapper methods

        private readonly Dictionary<int, IPublishedPropertyType> _wrappedPropertyTypes =
            new Dictionary<int, IPublishedPropertyType>();

        private readonly object _wrappedPropertyTypesLock = new object();

        /// <summary>
        ///     Gets property type of wrapped data type.
        /// </summary>
        /// <param name="wrapperPropertyType"></param>
        /// <returns></returns>
        private IPublishedPropertyType GetWrappedPropertyType(IPublishedPropertyType wrapperPropertyType)
        {
            lock (_wrappedPropertyTypesLock)
            {
                // check if wrapped property already exists in list
                if (_wrappedPropertyTypes.ContainsKey(wrapperPropertyType.DataType.Id))
                    return _wrappedPropertyTypes[wrapperPropertyType.DataType.Id];

                IPublishedPropertyType propertyType = null;

                var wrappedDataTypeId = GetWrappedDataTypeId(wrapperPropertyType.DataType);

                if (wrappedDataTypeId != default)
                    propertyType = GetPublishedPropertyType(wrappedDataTypeId, wrapperPropertyType.ContentType);

                // cache the current object for future calls
                _wrappedPropertyTypes.Add(wrapperPropertyType.DataType.Id, propertyType);

                return propertyType;
            }
        }

        /// <summary>
        ///     Gets the wrapped data type ID from the current data type's prevalues.
        /// </summary>
        /// <param name="wrapperDataType"></param>
        /// <returns></returns>
        private static int GetWrappedDataTypeId(PublishedDataType wrapperDataType)
        {
            int dataTypeId = default;

            var configuration = wrapperDataType.ConfigurationAs<TextCountConfiguration>();

            if (configuration == null)
                return dataTypeId;

            if (configuration.DataType != default)
                dataTypeId = configuration.DataType;

            return dataTypeId;
        }

        /// <summary>
        ///     Get published property type of the wrapped data type.
        /// </summary>
        /// <param name="dataTypeId"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private IPublishedPropertyType GetPublishedPropertyType(int dataTypeId, IPublishedContentType contentType)
        {
            var dataType = _dataTypeService.GetDataType(dataTypeId);

            if ((dataType == null) || (contentType == null))
                return null;

            var propertyType = new PropertyType(dataType);

            var publishedPropertyType = Current.PublishedContentTypeFactory.CreatePropertyType(contentType, propertyType);

            return publishedPropertyType;
        }

        #endregion
    }
}
#pragma warning restore CA1812