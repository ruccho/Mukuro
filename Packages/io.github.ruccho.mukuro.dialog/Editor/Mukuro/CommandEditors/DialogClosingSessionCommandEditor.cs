using System.Collections;
using System.Collections.Generic;
using Mukuro.Editors;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Dialog.Editors
{
    [CustomEventCommandEditor(typeof(DialogClosingSessionCommand))]
    public class DialogClosingSessionCommandEditor : EventCommandEditor
    {
        private static Texture2D Icon { get; set; }

        private static void EnsureIconLoaded()
        {
            if (Icon == null)
                Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/io.github.ruccho.mukuro.dialog/Editor/Mukuro/CommandEditors/Icons/DialogSession.png");
        }

        public DialogClosingSessionCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem,
            customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            return "ダイアログセッションの一時停止";
        }

        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#e38b4e", out Color c))
            {
                return c;
            }

            return null;
        }

        public override Texture2D GetIcon()
        {
            EnsureIconLoaded();
            return Icon;
        }

        public override void OnCreated()
        {
            BuildNestedCommandList();
        }

        public override void OnUpdate()
        {
            //
        }

        private void BuildNestedCommandList()
        {
            var commandProp = CommandItem.CommandProperty;
            var nestedCommandProp = commandProp.GetChildProperty("commandList.commands");
            CustomDetailRoot.Clear();
            var commandListView = new CommandListView(CommandItem.ParentList, nestedCommandProp);

            CustomDetailRoot.Add(commandListView);
        }
    }
}