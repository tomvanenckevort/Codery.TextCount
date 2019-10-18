using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Codery.TextCount.Configuration
{
    /// <summary>
    ///     Text Count editor configuration.
    /// </summary>
    internal sealed class TextCountConfigurationEditor : ConfigurationEditor<TextCountConfiguration>
    {
        private readonly IDictionary<string, object> _defaultConfiguration;

        public TextCountConfigurationEditor()
        {
            _defaultConfiguration = new Dictionary<string, object>
            {
                {"countType", "characters"},
                {"limitType", "none"}
            };
        }

        public override IDictionary<string, object> DefaultConfiguration => _defaultConfiguration;
    }
}