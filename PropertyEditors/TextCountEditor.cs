using ClientDependency.Core;
using Codery.TextCount.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PropertyEditors;

namespace Codery.TextCount.PropertyEditors
{
    /// <summary>
    ///     Text Count property editor.
    /// </summary>
    [DataEditor(EditorAlias, "Text Count", "~/App_Plugins/Codery.TextCount/textcount.html", ValueType = ValueTypes.Text)]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/Codery.TextCount/textcount.controller.js")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/Codery.TextCount/textcount.directive.js")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataTypePicker.Constants.ControllerPath)]
    internal sealed class TextCountEditor : DataEditor
    {
        public const string EditorAlias = "Codery.TextCount";

        private readonly IDataTypeService _dataTypeService;

        public TextCountEditor(ILogger logger, IDataTypeService dataTypeService) : base(logger)
        {
            _dataTypeService = dataTypeService;
        }

        protected override IConfigurationEditor CreateConfigurationEditor()
        {
            return new TextCountConfigurationEditor();
        }

        protected override IDataValueEditor CreateValueEditor()
        {
            return new TextCountDataValueEditor(Attribute, base.CreateValueEditor(), _dataTypeService);
        }
    }
}