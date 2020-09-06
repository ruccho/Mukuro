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
    [CustomEventCommandEditor(typeof(LogVariableCommand))]
    public class LogVariableCommandEditor : EventCommandEditor
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
        public LogVariableCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            var target = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");
            if (target.FindPropertyRelative("key") != null)
            {
                //空っぽじゃない
                var s = VariableReferencePropertyDrawer.GetDescription(target);

                return $"{s}の内容をログ出力";
            }
            else
            {
                return "変数ログ出力";
            }
        }

        public override VisualElement CreateCommandEditorGUI()
        {
            var root = new VisualElement();
            var target = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");
            var p = new TypeSelectableVariableReferencePropertyDrawer();
            root.Add(p.CreatePropertyGUI(target));
            return root;
            /*
            var root = new VisualElement();

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
                var targetProp = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");

                targetProp.managedReferenceValue = Activator.CreateInstance(type);
                targetProp.serializedObject.ApplyModifiedProperties();
                
                root.Unbind();
                root.Bind(targetProp.serializedObject);
            });

            var defaultType = typeSelector.value;
            var target = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");

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

            root.Add(targetField);

            root.Bind(target.serializedObject);
            return root;
            */
        }

        public override void OnCreated()
        {
            return;
            var target = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("target");
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
            target.serializedObject.ApplyModifiedProperties();
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