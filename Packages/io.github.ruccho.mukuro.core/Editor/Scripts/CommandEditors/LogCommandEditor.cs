using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(LogCommand))]
    public class LogEditor : EventCommandEditor
    {
        
        private static Texture2D Icon { get; set; }

        private static void EnsureIconLoaded()
        {
            if (Icon == null)
                Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/io.github.ruccho.mukuro.core/Editor/Scripts/CommandEditors/Icons/Log.png");
        }

        public override Texture2D GetIcon()
        {
            EnsureIconLoaded();
            return Icon;
        }
        public LogEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            return $"「{CommandItem.CommandProperty.GetProperty().FindPropertyRelative("message").stringValue}」をログ出力";
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