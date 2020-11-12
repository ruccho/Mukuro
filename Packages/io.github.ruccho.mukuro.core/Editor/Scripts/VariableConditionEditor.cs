using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    
    [CustomPropertyDrawer(typeof(VariableCondition))]
    public class VariableConditionEditor : PropertyDrawer
    {
        public static string GetDescription(SerializedProperty property)
        {
            var leftProp = property.FindPropertyRelative("left");
            var rightProp = property.FindPropertyRelative("right");

            var operatorProp = property.FindPropertyRelative("conditionOperator");

            var symbol = GetOperatorSymbol(operatorProp);

            var left = VariableReferencePropertyDrawer.GetDescription(leftProp);
            var right = VariableReferencePropertyDrawer.GetDescription(rightProp);

            return $"{left} {symbol} {right}";

        }

        private static string GetOperatorSymbol(SerializedProperty operatorProp)
        {
            int operatorEnumValueIndex = operatorProp.enumValueIndex;
            ConditionOperatorType operatorEnumValue = (ConditionOperatorType)Enum.GetValues(typeof(ConditionOperatorType)).GetValue(operatorEnumValueIndex);
            switch (operatorEnumValue)
            {
                case ConditionOperatorType.Equal:
                    return "=";
                case ConditionOperatorType.NotEqual:
                    return "!=";
                case ConditionOperatorType.LessThan:
                    return "<";
                case ConditionOperatorType.MoreThan:
                    return ">";
                case ConditionOperatorType.LessThanEqual:
                    return "<=";
                case ConditionOperatorType.MoreThanEqual:
                    return ">=";
                default:
                    return "(?)";
            }
        }
        
        private static readonly string uxml = "Packages/io.github.ruccho.mukuro.core/Editor/Layout/VariableCondition.uxml";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxml);
            var root = tree.CloneTree();
            var a = root.Q("BoxA");
            var b = root.Q("BoxB");
            var p = new TypeSelectableVariableReferencePropertyDrawer();
            var aField = p.CreatePropertyGUI(property.FindPropertyRelative("left"));// new PropertyField();
            var bField = p.CreatePropertyGUI(property.FindPropertyRelative("right"));//new PropertyField();
            //aField.bindingPath = property.FindPropertyRelative("left").propertyPath;
            //bField.bindingPath = property.FindPropertyRelative("right").propertyPath;

            var operatorButton = root.Q<Button>("OperatorButton");
            var operatorProp = property.FindPropertyRelative("conditionOperator");
            int operatorEnumValueIndex = operatorProp.enumValueIndex;
            var operatorEnumValue = Enum.GetValues(typeof(ConditionOperatorType)).GetValue(operatorEnumValueIndex);
            ConditionOperatorType[] types = new[]
            {
                ConditionOperatorType.Equal,
                ConditionOperatorType.NotEqual,
                ConditionOperatorType.LessThan,
                ConditionOperatorType.MoreThan,
                ConditionOperatorType.LessThanEqual,
                ConditionOperatorType.MoreThanEqual
            };
            string[] typeLabels = new[]
            {
                "=",
                "≠",
                "<",
                ">",
                "≦",
                "≧",
            };

            var labelIndex = Array.IndexOf(types, operatorEnumValue);
            if (labelIndex == -1)
            {
                operatorButton.text = "";
            }
            else
            {
                operatorButton.text = typeLabels[labelIndex];
            }
            
            operatorButton.clickable.clicked += () =>
            {
                var menu = new GenericMenu();
                    
                for (var i = 0; i < types.Length; i++)
                {
                    Action<SerializedProperty, int> onItemSelected = (prop, index) =>
                    {
                        menu.AddItem(new GUIContent(typeLabels[index]), false, () =>
                        {
                            prop.enumValueIndex = (int)types[index];
                            prop.serializedObject.ApplyModifiedProperties();
                            operatorButton.text = typeLabels[index];
                        });
                    };
                    onItemSelected(operatorProp, i);
                }
                
                var menuPosition = new Vector2(operatorButton.layout.xMin, operatorButton.layout.height);
                menuPosition = operatorButton.LocalToWorld(menuPosition);
                var menuRect = new Rect(menuPosition, Vector2.zero);
 
                menu.DropDown(menuRect);
            };
            
            a.Add(aField);
            b.Add(bField);
            return root;

        }
    }
    
}