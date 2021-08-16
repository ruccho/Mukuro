using System.Collections;
using System.Collections.Generic;
using Mukuro.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace Mukuro.Timeline.Editors
{
    [CustomEventCommandEditor(typeof(WaitForSignalCommand), IconTexturePath = iconPath)]
    public class WaitForSignalCommandEditor : EventCommandEditor
    {
        private static Texture2D Icon { get; set; }

        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.timeline/Editor/Mukuro/CommandEditors/Icons/TimelineMarker.png";

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

        public WaitForSignalCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem,
            customDetailRoot)
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
            SignalAsset signalAsset = commandProp.GetChildProperty("target").GetProperty().objectReferenceValue as SignalAsset;
            
            return $"シグナルの待ち受け：{(signalAsset != null ? signalAsset.name : "")}";
        }


        public override void OnCreated()
        {
            BuildNestedCommandList();
        }

        private void BuildNestedCommandList()
        {
            var commandProp = CommandItem.CommandProperty;
            var nestedCommandProp = commandProp.GetChildProperty("nestedCommands.commands");
            CustomDetailRoot.Clear();
            var commandListView = new CommandListView(CommandItem.ParentList, nestedCommandProp);

            CustomDetailRoot.Add(commandListView);
        }
    }
}