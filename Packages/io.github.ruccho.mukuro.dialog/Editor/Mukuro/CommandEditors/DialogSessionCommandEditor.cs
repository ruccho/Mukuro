using System.Collections;
using System.Collections.Generic;
using Mukuro.Editors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Dialog.Editors
{
    [CustomEventCommandEditor(typeof(DialogSessionCommand), IconTexturePath = iconPath)]
    public class DialogSessionCommandEditor : EventCommandEditor
    {
        private static Texture2D Icon { get; set; }

        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.dialog/Editor/Mukuro/CommandEditors/Icons/DialogSession.png";

        private static void EnsureIconLoaded()
        {
            if (Icon == null)
                Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        }

        public DialogSessionCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem,
            customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            var providerName = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("providerName")
                .stringValue;
            
            return $"ダイアログセッション ({providerName})";
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

        public override VisualElement CreateCommandEditorGUI()
        {
            var root = new VisualElement();
            root.Add(new PropertyField(CommandItem.CommandProperty.GetProperty().FindPropertyRelative("providerName")));
            
            root.Bind(CommandItem.CommandProperty.SerializedObject);
            return root;
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