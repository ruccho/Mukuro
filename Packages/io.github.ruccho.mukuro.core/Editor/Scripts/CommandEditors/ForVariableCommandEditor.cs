using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(ForVariableCommand), IconTexturePath = iconPath)]
    public class ForVariableCommandEditor : EventCommandEditor
    {
        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.core/Editor/Scripts/CommandEditors/Icons/Loop.png";
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
        
        public ForVariableCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override VisualElement CreateCommandEditorGUI()
        {
            var root = new VisualElement();
            var variable = new PropertyField();
            variable.label = "条件";
            variable.bindingPath = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("count").propertyPath;
            root.Add(variable);
            root.Bind(CommandItem.CommandProperty.SerializedObject);
            return root;
        }

        public override string GetLabelText()
        {
            var prop = CommandItem.CommandProperty.GetProperty();
            var vProp = prop.FindPropertyRelative("count").FindPropertyRelative("variable");
            var v = VariableReferencePropertyDrawer.GetDescription(vProp);
            return $"回数繰り返し (変数) : {v}";
        }


        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#8585ff", out Color c))
            {
                return c;
            }

            return null;
        }

        public override void OnCreated()
        {
            CustomDetailRoot.Clear();
            
            var l0 = new Label("ルーチン");
            CustomDetailRoot.Add(l0);
            
            var commandProp = CommandItem.CommandProperty;
            
            var trueProp = commandProp.GetChildProperty("routine.commands");
            var trueListView = new CommandListView(CommandItem.ParentList,trueProp);
            CustomDetailRoot.Add(trueListView);
            
            
            
        }
    }
}