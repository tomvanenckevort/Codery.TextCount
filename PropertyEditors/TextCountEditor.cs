using System.Collections.Generic;
using ClientDependency.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;
using Constants = Codery.DataTypePicker.Constants;

namespace Codery.TextCount.PropertyEditors
{
    /// <summary>
    ///     Link picker property editor.
    /// </summary>
    [PropertyEditor("Codery.TextCount", "Text Count",
         editorView: "~/App_Plugins/Codery.TextCount/textcount.html", valueType: "TEXT")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/Codery.TextCount/textcount.controller.js")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/Codery.TextCount/textcount.directive.js")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, Constants.ControllerPath)]
    public class TextCountEditor : PropertyEditor
    {
        private IDictionary<string, object> _defaultPreValues;

        public TextCountEditor()
        {
            _defaultPreValues = new Dictionary<string, object>
            {
                {"countType", "characters"},
                {"limitType", "none"}
            };
        }

        public override IDictionary<string, object> DefaultPreValues
        {
            get { return _defaultPreValues; }

            set { _defaultPreValues = value; }
        }

        protected override PreValueEditor CreatePreValueEditor()
        {
            var editor = base.CreatePreValueEditor();

            editor.Fields.Add(new PreValueField
            {
                Description = "Choose which data type to apply the text count to. " +
                              "Note that the count will be applied to all textstring and textarea fields in the wrapped data type.",
                Key = "dataType",
                Name = "Data Type",
                View = Constants.ViewPath
            });

            editor.Fields.Add(new PreValueField
            {
                Description = "Choose whether to count characters or words.",
                Key = "countType",
                Name = "Characters or words?",
                View = "/App_Plugins/Codery.TextCount/prevalues/count-type.html"
            });

            editor.Fields.Add(new PreValueField
            {
                Description = "Number of characters or words which are the limit. Leave empty if there is no limit.",
                Key = "limit",
                Name = "Limit",
                View = "number"
            });

            editor.Fields.Add(new PreValueField
            {
                Description = "Choose what happens when the limit is reached.",
                Key = "limitType",
                Name = "Limit Type",
                View = "/App_Plugins/Codery.TextCount/prevalues/limit-type.html"
            });

            return editor;
        }
    }
}