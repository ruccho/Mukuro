using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(CalculationCommand), IconTexturePath = iconPath)]
    public class CalculationCommandEditor : EventCommandEditor
    {
        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.core/Editor/Scripts/CommandEditors/Icons/Calculation.png";
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

        public CalculationCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem,
            customDetailRoot)
        {
            var prop = commandItem.CommandProperty.GetProperty();
            /*
            prop.FindPropertyRelative("left").managedReferenceValue = new IntVariableReference();
            prop.FindPropertyRelative("right0").managedReferenceValue = new IntVariableReference();
            prop.FindPropertyRelative("right1").managedReferenceValue = new IntVariableReference();
            */
            prop.serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateCommandEditorGUI()
        {
            return base.CreateCommandEditorGUI();
        }

        public override string GetLabelText()
        {
            var desc = VariableCalculationEditor.GetDescription(CommandItem.CommandProperty.GetProperty()
                .FindPropertyRelative("calculation"));
            return $"演算: {desc}";
        }


        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#7ee5fc", out Color c))
            {
                return c;
            }

            return null;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }
    }
}