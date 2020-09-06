using System.Collections;
using System.Collections.Generic;
using Mukuro.Editors;
using Mukuro.Samples;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Samples
{
    [CustomEventCommandEditor(typeof(SampleEventCommand2))]
    public class SampleEventCommand2Editor : EventCommandEditor
    {
        public SampleEventCommand2Editor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            var sampleString = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("sampleString")?.stringValue;
            if (sampleString == null) return "";
            return "サンプルコマンド2: " + sampleString;
        }
    }
}