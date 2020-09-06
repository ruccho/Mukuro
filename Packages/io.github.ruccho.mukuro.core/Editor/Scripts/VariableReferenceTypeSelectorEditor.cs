using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomPropertyDrawer(typeof(VariableReferenceTypeSelector))]
    public class VariableReferenceTypeSelectorEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            var target = property.FindPropertyRelative("variable");

            if (target.FindPropertyRelative("key") == null)
            {
                target.managedReferenceValue = new IntVariableReference();
                target.serializedObject.ApplyModifiedProperties();
            }
            
            List<Type> compatibleTypes = new List<Type>
            {
                typeof(IntVariableReference),
                typeof(BoolVariableReference),
                typeof(FloatVariableReference),
                typeof(StringVariableReference),
                typeof(ColorVariableReference),
                typeof(ObjectVariableReference),
                typeof(LayerMaskVariableReference),
                typeof(Vector2VariableReference),
                typeof(Vector3VariableReference),
                typeof(Vector4VariableReference),
                typeof(RectVariableReference),
                typeof(AnimationCurveVariableReference),
                typeof(BoundsVariableReference),
                typeof(GradientVariableReference),
                typeof(QuaternionVariableReference),
                typeof(Vector2IntVariableReference),
                typeof(Vector3IntVariableReference),
                typeof(RectIntVariableReference),
                typeof(BoundsIntVariableReference),
            };

            Func<Type, string> formatter = (type) =>
            {
                if (type.IsGenericType)
                {
                    return type.GetGenericArguments()[0].Name;
                }
                else
                {
                    return type.Name;
                }
            };

            var typeSelector = new PopupField<Type>(compatibleTypes, 0, formatter, formatter);
            typeSelector.label = "型の変更";
            typeSelector.RegisterCallback<ChangeEvent<Type>>(evt =>
            {
                var type = evt.newValue;
                var targetProp = property.FindPropertyRelative("variable");

                targetProp.managedReferenceValue = Activator.CreateInstance(type);
                
                targetProp.serializedObject.ApplyModifiedProperties();
                
                root.Unbind();
                root.Bind(targetProp.serializedObject);
            });

            var defaultType = typeSelector.value;

            root.Add(typeSelector);
            
            var variableField = new PropertyField(target);
            
            root.Bind(target.serializedObject);
            
            root.Add(variableField);
            return root;

        }
    }
}