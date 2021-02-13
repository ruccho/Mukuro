using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(LogVariableCommand), IconTexturePath = iconPath)]
    public class LogVariableCommandEditor : EventCommandEditor
    {
        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.core/Editor/Scripts/CommandEditors/Icons/Log.png";
        private static Texture2D Icon { get; set; }

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
        public LogVariableCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            var target = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");
            if (target.FindPropertyRelative("key") != null)
            {
                //空っぽじゃない
                var s = VariableReferencePropertyDrawer.GetDescription(target);

                return $"{s}の内容をログ出力";
            }
            else
            {
                return "変数ログ出力";
            }
        }

        public override VisualElement CreateCommandEditorGUI()
        {
            var root = new VisualElement();
            var target = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");
            var p = new TypeSelectableVariableReferencePropertyDrawer();
            root.Add(p.CreatePropertyGUI(target));
            return root;
        }
        
        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#fcdb7e", out Color c))
            {
                return c;
            }

            return null;
        }
    }
}