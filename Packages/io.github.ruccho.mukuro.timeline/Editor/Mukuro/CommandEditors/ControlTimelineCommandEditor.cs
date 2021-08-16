using System;
using System.Collections;
using System.Collections.Generic;
using Mukuro.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Timeline.Editors
{
    [CustomEventCommandEditor(typeof(ControlTimelineCommand), IconTexturePath = iconPath)]
    public class ControlTimelineCommandEditor : EventCommandEditor
    {
        private static Texture2D Icon { get; set; }

        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.timeline/Editor/Mukuro/CommandEditors/Icons/Timeline.png";

        private static void EnsureIconLoaded()
        {
            if (Icon == null)
                Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        }

        public override Texture2D GetIcon()
        {
            EnsureIconLoaded();
            return Icon;
        }
        
        public ControlTimelineCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#92ddc0", out Color c)) return c;
            return null;
        }

        public override string GetLabelText()
        {
            var commandProp = CommandItem.CommandProperty;
            var controlProp = commandProp.GetChildProperty("control").GetProperty();

            ControlTimelineCommand.ControlType
                control = (ControlTimelineCommand.ControlType) controlProp.enumValueIndex;

            string controlLabel = "";
            switch(control)
            {
                case ControlTimelineCommand.ControlType.Resume:
                    controlLabel = "再開";
                    break;
                case ControlTimelineCommand.ControlType.Pause:
                    controlLabel = "一時停止";
                    break;
                case ControlTimelineCommand.ControlType.Stop:
                    controlLabel = "完全停止";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return $"タイムライン制御({controlLabel})";
        }
    }
}