using System.Collections;
using System.Collections.Generic;
using Mukuro.Editors;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(NestEventScriptCommand))]
    public class NestEventScriptCommandEditor : EventCommandEditor
    {
        public NestEventScriptCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            var nestedScript = CommandItem.CommandProperty.GetChildProperty("scriptAsset").GetProperty().objectReferenceValue;
            var labelText = (nestedScript == null ? "" : nestedScript.name);
            return $"イベントの埋め込み: {labelText}";
        }

        public override void OnCreated()
        {
        }

        public override void OnUpdate()
        {
        }
     
    }
}