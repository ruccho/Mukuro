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
    [CustomPropertyDrawer(typeof(VariableReference))]
    public class VariableReferencePropertyDrawer : PropertyDrawer
    {
        public static string GetDescription(SerializedProperty prop)
        {
            var storeTypeProp = prop?.FindPropertyRelative("storeType");
            if (storeTypeProp == null)
            {
                return "(Unknown)";
            }
            var keyProp = prop.FindPropertyRelative("key");
            var key = keyProp.stringValue;
            var constantValueProp = prop.FindPropertyRelative("constantValue");
            var storeType = storeTypeProp.enumDisplayNames[storeTypeProp.enumValueIndex];

            switch (storeType)
            {
                case "Constant":
                    return GetPropertyValue(constantValueProp).ToString();
                
                case "Temporary":
                    return $"一時変数<{key}>";
                
                case "Event":
                    return $"イベント変数<{key}>";
                
                default:
                    return "(Unknown)";
            }
        }

        private static object GetPropertyValue(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Generic:
                    return "(Generic)";
                
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.LayerMask:
                    return prop.intValue;
                
                case SerializedPropertyType.Boolean:
                    return prop.boolValue;
                
                case SerializedPropertyType.Float:
                    return prop.floatValue;
                
                case SerializedPropertyType.String:
                    return prop.stringValue;
                
                case SerializedPropertyType.Color:
                    return prop.colorValue;
                
                case SerializedPropertyType.ObjectReference:
                    if (prop.objectReferenceValue)
                    {
                        return prop.objectReferenceValue;
                    }
                    else
                    {
                        return "(null)";
                    }
                
                case SerializedPropertyType.Enum:
                    return prop.enumValueIndex;
                
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value;
                
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value;
                
                case SerializedPropertyType.Vector4:
                    return prop.vector4Value;
                
                case SerializedPropertyType.Rect:
                    return prop.rectValue;
                
                case SerializedPropertyType.ArraySize:
                    return prop.arraySize;
                
                case SerializedPropertyType.Character:
                    throw new NotImplementedException();
                case SerializedPropertyType.AnimationCurve:
                    return prop.animationCurveValue;
                
                case SerializedPropertyType.Bounds:
                    return prop.boundsValue;
                
                case SerializedPropertyType.Gradient:
                    return "(Gradient)";
                case SerializedPropertyType.Quaternion:
                    return prop.quaternionValue;
                
                case SerializedPropertyType.ExposedReference:
                    return "ExposedReference";
                
                case SerializedPropertyType.FixedBufferSize:
                    return prop.fixedBufferSize;
                
                case SerializedPropertyType.Vector2Int:
                    return prop.vector2IntValue;
                
                case SerializedPropertyType.Vector3Int:
                    return prop.vector3IntValue;
                
                case SerializedPropertyType.RectInt:
                    return prop.rectIntValue;
                
                case SerializedPropertyType.BoundsInt:
                    return prop.boundsIntValue;
                
                case SerializedPropertyType.ManagedReference:
                    return "ManagedReference";
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            UpdatePropertyGUI(property, root);
            return root;
        }

        private static void UpdatePropertyGUI(SerializedProperty property, VisualElement root)
        {
            root.Clear();

            var targetTypeProp = property.FindPropertyRelative("storeType");
            if (targetTypeProp == null)
            {
                property.managedReferenceValue = new IntVariableReference();
                property.serializedObject.ApplyModifiedProperties();
                targetTypeProp = property.FindPropertyRelative("storeType");
            }

            var targetType = targetTypeProp.enumDisplayNames[targetTypeProp.enumValueIndex];

            var typeToggle = new PropertyField(targetTypeProp, "ストア");
            root.Add(typeToggle);

            switch (targetType)
            {
                case "Constant":
                    var constantField = new PropertyField(property.FindPropertyRelative("constantValue"), "定数値");
                    root.Add(constantField);
                    break;
                default:
                    var targetKey = property.FindPropertyRelative("key");
                    var keyField = new PropertyField(targetKey, "キー");
                    root.Add(keyField);
                    break;
            }

            root.Bind(property.serializedObject);
            
            typeToggle.RegisterCallback<ChangeEvent<string>>(evt => { UpdatePropertyGUI(property, root); });
        }
    }

    [CustomPropertyDrawer(typeof(VariableReference.TypeSelectableAttribute))]
    public class TypeSelectableVariableReferencePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            UpdatePropertyGUI(property, root);
            return root;
        }

        private static void UpdatePropertyGUI(SerializedProperty property, VisualElement root)
        {
            root.Clear();
            
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
                if (type.BaseType.IsGenericType)
                {
                    return type.BaseType.GetGenericArguments()[0].Name;
                }
                else
                {
                    return type.Name;
                }
            };

            var typeSelector = new PopupField<Type>(compatibleTypes, 0, formatter, formatter);
            typeSelector.label = "タイプ";

            var typeInfo = property.managedReferenceFullTypename.Split(' ');
            if (typeInfo.Length >= 2)
            {
                var matched = compatibleTypes.FirstOrDefault(t => t.FullName == typeInfo[1]);
                if (matched != default)
                {
                    typeSelector.value = matched;
                }
            }

            root.Add(typeSelector);

            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                property.managedReferenceValue = new IntVariableReference();
                property.serializedObject.ApplyModifiedProperties();
            }
            
            var targetTypeProp = property.FindPropertyRelative("storeType");

            var targetType = targetTypeProp.enumDisplayNames[targetTypeProp.enumValueIndex];

            var typeToggle = new PropertyField(targetTypeProp, "ストア");
            root.Add(typeToggle);

            switch (targetType)
            {
                case "Constant":
                    var constantField = new PropertyField(property.FindPropertyRelative("constantValue"), "定数値");
                    root.Add(constantField);
                    break;
                default:
                    var targetKey = property.FindPropertyRelative("key");
                    var keyField = new PropertyField(targetKey, "キー");
                    root.Add(keyField);
                    break;
            }

            root.Bind(property.serializedObject);
            
            typeSelector.RegisterCallback<ChangeEvent<Type>>(evt =>
            {
                var type = evt.newValue;

                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
                UpdatePropertyGUI(property, root);
            });
            typeToggle.RegisterCallback<ChangeEvent<string>>(evt => { UpdatePropertyGUI(property, root); });
        }
    }
}