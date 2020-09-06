using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomPropertyDrawer(typeof(VariableCalculation))]
    public class VariableCalculationEditor : PropertyDrawer
    {
        public static string GetDescription(SerializedProperty property)
        {
            var leftProp = property.FindPropertyRelative("left");
            var right0Prop = property.FindPropertyRelative("right0");
            var right1Prop = property.FindPropertyRelative("right1");

            var operatorProp = property.FindPropertyRelative("calculationOperator");

            var symbol = GetOperatorSymbol(operatorProp);

            var left = VariableReferencePropertyDrawer.GetDescription(leftProp);
            var right0 = VariableReferencePropertyDrawer.GetDescription(right0Prop);
            var right1 = VariableReferencePropertyDrawer.GetDescription(right1Prop);

            return $"{left} = {right0} {symbol} {right1}";

        }

        private static string GetOperatorSymbol(SerializedProperty operatorProp)
        {
            int operatorEnumValueIndex = operatorProp.enumValueIndex;
            CalculationOperatorType operatorEnumValue = (CalculationOperatorType)Enum.GetValues(typeof(CalculationOperatorType)).GetValue(operatorEnumValueIndex);
            switch (operatorEnumValue)
            {
                case CalculationOperatorType.Add:
                    return "+";
                case CalculationOperatorType.Subtract:
                    return "-";
                case CalculationOperatorType.Multiply:
                    return "x";
                case CalculationOperatorType.Divide:
                    return "/";
                default:
                    return "(?)";
            }
        }
        
        private static readonly string uxml = "Packages/io.github.ruccho.mukuro.core/Editor/Layout/Calculation.uxml";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxml);
            var root = tree.CloneTree();
            var a = root.Q("BoxA");
            var b = root.Q("BoxB");
            var c = root.Q("BoxC");
            var p = new TypeSelectableVariableReferencePropertyDrawer();
            var aField = p.CreatePropertyGUI(property.FindPropertyRelative("left"));
            var bField = p.CreatePropertyGUI(property.FindPropertyRelative("right0"));
            var cField = p.CreatePropertyGUI(property.FindPropertyRelative("right1"));

            var operatorButton = root.Q<Button>("OperatorButton");;
            var operatorProp = property.FindPropertyRelative("calculationOperator");
            int operatorEnumValueIndex = operatorProp.enumValueIndex;
            var operatorEnumValue = Enum.GetValues(typeof(CalculationOperatorType)).GetValue(operatorEnumValueIndex);
            CalculationOperatorType[] types = new[]
            {
                CalculationOperatorType.Add,
                CalculationOperatorType.Subtract,
                CalculationOperatorType.Multiply,
                CalculationOperatorType.Divide
            };
            string[] typeLabels = new[]
            {
                "＋",
                "ー",
                "×",
                "／"
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
            c.Add(cField);
            return root;

        }
    }
    
}