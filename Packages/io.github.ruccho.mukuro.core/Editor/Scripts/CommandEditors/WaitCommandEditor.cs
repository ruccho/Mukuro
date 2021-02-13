using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(WaitCommand), IconTexturePath = iconPath)]
    public class WaitCommandEditor : EventCommandEditor
    {
        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.core/Editor/Scripts/CommandEditors/Icons/Wait.png";
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
        public WaitCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            string desc = VariableReferencePropertyDrawer.GetDescription(CommandItem.CommandProperty.GetProperty().FindPropertyRelative("duration"));
            return $"{desc} 秒ウェイト";
        }

        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#a5bbe2", out Color c))
            {
                return c;
            }

            return null;
        }
    }
}