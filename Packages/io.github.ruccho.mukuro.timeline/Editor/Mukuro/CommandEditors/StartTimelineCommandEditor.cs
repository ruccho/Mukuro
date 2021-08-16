using System.Collections;
using System.Collections.Generic;
using Mukuro.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Timeline.Editors
{
    [CustomEventCommandEditor(typeof(StartTimelineCommand), IconTexturePath = iconPath)]
    public class StartTimelineCommandEditor : EventCommandEditor
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
        
        public StartTimelineCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#92ddc0", out Color c)) return c;
            return null;
        }

        public override string GetLabelText() => "タイムラインの開始";
        

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