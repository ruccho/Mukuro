using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mukuro.Editors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEventCommandEditor(typeof(SetVariableCommand), IconTexturePath = iconPath)]
    public class SetVariableCommandEditor : EventCommandEditor
    {
        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.core/Editor/Scripts/CommandEditors/Icons/SetVariable.png";
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
        private VisualElement CommandEditorRootElement { get; set; }
        public SetVariableCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem,
            customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            var target = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");
            var valueToSet = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("value");
            if (target.hasChildren)
            {
                var iter = target.Copy();
                iter.Next(true);
                if (iter.depth > target.depth && target.FindPropertyRelative("key") != null)
                {
                    //空っぽじゃない
                    var targetKey = target.FindPropertyRelative("key").stringValue;
                    var valueKey = valueToSet.FindPropertyRelative("key").stringValue;

                    var targetDesc = VariableReferencePropertyDrawer.GetDescription(target);
                    var valueDesc = VariableReferencePropertyDrawer.GetDescription(valueToSet);
                    /*
                    var targetTypeProp = target.FindPropertyRelative("storeType");
                    var valueTypeProp = valueToSet.FindPropertyRelative("storeType");
                    var targetType = targetTypeProp.enumDisplayNames[targetTypeProp.enumValueIndex];
                    var valueType = valueTypeProp.enumDisplayNames[valueTypeProp.enumValueIndex];

                    string targetText = (targetType == "Constant" ? "(定数)" :  $"<{targetKey}>({(targetType == "Event" ? "イベント変数" : "一時変数")})");
                    string valueText = (valueType == "Constant" ? "(定数)" :  $"<{valueKey}>({(valueType == "Event" ? "イベント変数" : "一時変数")})");
*/
                    return $"変数代入: {targetDesc} = {valueDesc}";
                }
            }
            return "変数代入: (未設定)";
        }

        public override void OnCreated()
        {
            var target = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");
            var valueToSet = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("value");
            if (target.hasChildren)
            {
                var iter = target.Copy();
                iter.Next(true);
                if (iter.depth > target.depth && target.FindPropertyRelative("key") != null)
                {
                    //空っぽじゃない
                    return;
                }
            }

            target.managedReferenceValue = Activator.CreateInstance(typeof(IntVariableReference));
            valueToSet.managedReferenceValue = Activator.CreateInstance(typeof(IntVariableReference));
            target.serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateCommandEditorGUI()
        {
            var root = new VisualElement();

            List<Type> compatibleReferenceTypes = new List<Type>
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

            List<Type> compatibleTypes = compatibleReferenceTypes.Select(t => t.BaseType.GenericTypeArguments[0]).ToList();

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
                var type = compatibleReferenceTypes[compatibleTypes.IndexOf(evt.newValue)];
                var targetProp = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");
                var valueToSetProp = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("value");

                targetProp.managedReferenceValue = Activator.CreateInstance(type);
                valueToSetProp.managedReferenceValue = Activator.CreateInstance(type);
                targetProp.serializedObject.ApplyModifiedProperties();
                
                root.Unbind();
                root.Bind(targetProp.serializedObject);
            });

            var defaultType = typeSelector.value;
            var target = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");
            var valueToSet = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("value");

            root.Add(typeSelector);


            if (target.hasChildren)
            {
                var typeInfo = target.managedReferenceFullTypename.Split(' ');
                if (typeInfo.Length >= 2)
                {
                    var name = typeInfo[1];
                    var matched = compatibleTypes.FirstOrDefault(t => t.FullName == name);
                    if (matched != default)
                    {
                        typeSelector.value = matched;
                    }
                }
                else
                {
                }
            }

            var targetField = new PropertyField();
            targetField.bindingPath = target.propertyPath;
            var valueToSetField = new PropertyField();
            valueToSetField.bindingPath = valueToSet.propertyPath;

            root.Add(targetField);
            root.Add(valueToSetField);

            root.Bind(target.serializedObject);
            CommandEditorRootElement = root;
            return root;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
        
        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#7ee5fc", out Color c))
            {
                return c;
            }

            return null;
        }
    }
}