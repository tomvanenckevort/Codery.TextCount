using Umbraco.Core.PropertyEditors;

#pragma warning disable CA1812
namespace Codery.TextCount.Configuration
{
    internal sealed class TextCountConfiguration
    {
        [ConfigurationField("dataType", "Data Type", DataTypePicker.Constants.ViewPath, Description = "Choose which data type to apply the text count to. " +
                              "Note that the count will be applied to all textstring and textarea fields in the wrapped data type.")]
        public int DataType { get; set; }

        [ConfigurationField("countType", "Characters or words?", "/App_Plugins/Codery.TextCount/configuration/count-type.html", Description = "Choose whether to count characters or words.")]
        public string CountType { get; set; }

        [ConfigurationField("limit", "Limit", "number", Description = "Number of characters or words which are the limit. Leave empty if there is no limit.")]
        public int Limit { get; set; }

        [ConfigurationField("limitType", "Limit Type", "/App_Plugins/Codery.TextCount/configuration/limit-type.html", Description = "Choose what happens when the limit is reached.")]
        public string LimitType { get; set; }
    }
}
#pragma warning restore CA1812