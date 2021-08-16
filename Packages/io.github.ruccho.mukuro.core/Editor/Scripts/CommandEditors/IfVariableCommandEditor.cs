using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(IfVariableCommand), IconTexturePath = iconPath)]
    public class IfVariableCommandEditor : EventCommandEditor
    {
        private const string iconPath = "Packages/io.github.ruccho.mukuro.core/Editor/Scripts/CommandEditors/Icons/Bifurcation.png";
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
        
        public IfVariableCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override VisualElement CreateCommandEditorGUI()
        {
            var root = new VisualElement();
            
            var condition = new PropertyField();
            condition.label = "条件";
            condition.bindingPath = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("condition").propertyPath;
            root.Add(condition);
            
            var onFailed = new PropertyField();
            onFailed.label = "評価失敗時の分岐";
            onFailed.bindingPath = CommandItem.CommandProperty.GetChildProperty("onFailed").GetProperty().propertyPath;
            root.Add(onFailed);
            
            root.Bind(CommandItem.CommandProperty.SerializedObject);
            return root;
        }

        public override string GetLabelText()
        {
            var condition = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("condition");
            var desc = VariableConditionEditor.GetDescription(condition);
            return $"条件分岐 (変数) : {desc}";
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
            
            var l0 = new Label("真の場合");
            CustomDetailRoot.Add(l0);
            
            var commandProp = CommandItem.CommandProperty;
            
            var trueProp = commandProp.GetChildProperty("ifTrue.commands");
            var trueListView = new CommandListView(CommandItem.ParentList,trueProp);
            CustomDetailRoot.Add(trueListView);
            
            var l1 = new Label("偽の場合");
            CustomDetailRoot.Add(l1);
            
            var falseProp = commandProp.GetChildProperty("ifFalse.commands");
            var falseListView = new CommandListView(CommandItem.ParentList,falseProp);
            CustomDetailRoot.Add(falseListView);
            
            
        }
    }
}