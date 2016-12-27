using System.Collections.Generic;
using ClientDependency.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace Codery.TextCount.PropertyEditors
{
    /// <summary>
    /// Link picker property editor.
    /// </summary>
    [PropertyEditor(alias: "Codery.TextCount", name: "Text Count", 
        editorView: "~/App_Plugins/Codery.TextCount/textcount.html", valueType: PropertyEditorValueTypes.Text)]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/Codery.TextCount/textcount.controller.js")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/Codery.TextCount/textcount.directive.js")]
    [PropertyEditorAsset(ClientDependencyType.Javascript, DataTypePicker.Constants.ControllerPath)]
    public class TextCountEditor : PropertyEditor
    {
        private IDictionary<string, object> defaultPreValues;

        public TextCountEditor()
        {
            defaultPreValues = new Dictionary<string, object>
            {
                { "countType", "characters" },
                { "limitType", "none" }
            };
        }

        protected override PreValueEditor CreatePreValueEditor()
        {
            var editor = base.CreatePreValueEditor();

            editor.Fields.Add(new PreValueField()
            {
                Description = "Choose which data type to apply the text count to. " + 
                    "Note that the count will be applied to all textstring and textarea fields in the wrapped data type.",
                Key = "dataType",
                Name = "Data Type",
                View = DataTypePicker.Constants.ViewPath
            });

            editor.Fields.Add(new PreValueField()
            {
                Description = "Choose whether to count characters or words.",
                Key = "countType",
                Name = "Characters or words?",
                View = "/App_Plugins/Codery.TextCount/prevalues/count-type.html"
            });

            editor.Fields.Add(new PreValueField()
            {
                Description = "Number of characters or words which are the limit. Leave empty if there is no limit.",
                Key = "limit",
                Name = "Limit",
                View = "number"
            });

            editor.Fields.Add(new PreValueField()
            {
                Description = "Choose what happens when the limit is reached.",
                Key = "limitType",
                Name = "Limit Type",
                View = "/App_Plugins/Codery.TextCount/prevalues/limit-type.html"
            });

            return editor;
        }

        public override IDictionary<string, object> DefaultPreValues
        {
            get
            {
                return defaultPreValues;
            }

            set
            {
                defaultPreValues = value;
            }
        }
    }
}