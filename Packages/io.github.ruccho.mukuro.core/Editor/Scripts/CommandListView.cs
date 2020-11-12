using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    public class CommandListView : BindableElement
    {
        public CommandListView ParentList { get; }
        public bool IsRoot => ParentList == null;

        private Action<CommandItem> OnItemSelected;
        
        private PersistentSerializedProperty CommandArrayProperty { get; }

        public CommandItem SelectedItem { get; private set; }

        /// <summary>
        /// ルートListViewとしてセットアップします。
        /// </summary>
        /// <param name="onItemSelected">アイテム選択時のコールバック。</param>
        internal CommandListView(Action<CommandItem> onItemSelected, SerializedProperty commandArray)
        {
            CommandArrayProperty = new PersistentSerializedProperty(commandArray);
            if(!CommandArrayProperty.IsArray) throw new ArgumentException("commandArray must be an array.");
            OnItemSelected = onItemSelected;
            this.binding = new CommandListViewBinding(() =>
            {
                if (SelectedItem != null && SelectedItem.parent != null)
                    SelectedItem.UpdateCommandDetail();
            });
            BuildList();
            
        }

        public CommandListView(CommandListView parentList, PersistentSerializedProperty commandArray)
        {
            CommandArrayProperty = commandArray;
            if(!CommandArrayProperty.IsArray) throw new ArgumentException("commandArray must be an array.");
            ParentList = parentList;
            BuildList();
        }
        
        private void BuildList()
        {
            this.style.minHeight = 20f;
            this.style.borderLeftWidth = 2f;
            this.style.borderLeftColor = new Color(0, 0, 0, 0.5f);
            
            this.AddManipulator(new CommandMoveTargetManipulator(
                this,
                part =>
                {
                    switch (part)
                    {
                        case MovementPart.Top:
                            return 0;
                        case MovementPart.Bottom:
                            return this.childCount;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(part), part, null);
                    }
                },
                () =>
                {
                    this.style.borderTopWidth = new StyleFloat(3f);
                    this.style.borderTopColor = new StyleColor(new Color(0.2f, 0.3411765f, 0.8509805f));
                },
                () =>
                {
                    this.style.borderTopWidth = new StyleFloat(3f);
                    this.style.borderTopColor = new StyleColor(new Color(0.2f, 0.3411765f, 0.8509805f));
                },
                () =>
                {
                    this.style.borderTopWidth = new StyleFloat(0f);
                }));
            
            this.style.paddingTop = new StyleLength(3f);
            this.style.paddingBottom = new StyleLength(0f);
            this.Clear();
            int count = CommandArrayProperty.GetArraySize();

            for (int i = 0; i < count; i++)
            {
                var prop = CommandArrayProperty.GetArrayElementAt(i);
                AddCommandElementAt(i, prop);
            }
        }

        private void SelectItem(CommandItem item)
        {
            if (IsRoot)
            {
                if (SelectedItem == item) return;

                SelectedItem?.OnDeselected();

                SelectedItem = item;

                item?.OnSelected();

                OnItemSelected?.Invoke(item);
            }
            else
            {
                //親に伝播
                ParentList.SelectItem(item);
            }
        }

        private CommandItem AddCommandElementAt(int index, PersistentSerializedProperty pProp)
        {
            var item = new CommandItem(this, pProp,SelectItem, Move);
            this.Insert(index, item);
            return item;
        }

        private CommandItem AddCommandAt(int index, EventCommand command)
        {
            var pProp = CommandArrayProperty.InsertArrayElementAt(index);
            var prop = pProp.GetProperty();
            prop.managedReferenceValue = command;
            prop.serializedObject.ApplyModifiedProperties();
            return AddCommandElementAt(index, pProp);
        }

        private CommandItem AddCommandAt(int index, PersistentSerializedProperty source)
        {
            var destPProp = CommandArrayProperty.InsertArrayElementAt(index);
            
            var destProp = destPProp.GetProperty();
            destProp.managedReferenceValue = null;
            var sourceProp = source.GetProperty();
            
            CopyCommandProperty(sourceProp, destProp);
            
            destProp.serializedObject.ApplyModifiedProperties();
            return AddCommandElementAt(index, destPProp);
        }

        private void RemoveCommandAt(int index)
        {
            var pProp = CommandArrayProperty;
            var prop = pProp.GetProperty();
            pProp.DeleteArrayElementAt(index);
            this.RemoveAt(index);
        }
        
        public void RemoveCommandAtSelected()
        {
            if (SelectedItem != null && SelectedItem.parent != null)
            {
                SelectedItem.ParentList.RemoveCommandAt(SelectedItem.GetIndex());
            }
            
        }
        

        public CommandItem AddCommandAtSelected(EventCommand command)
        {
            if (!IsRoot)
                throw new InvalidOperationException(
                    "AddCommandAtSelected can't be executed on non-root CommandListView.");

            if (SelectedItem != null && SelectedItem.parent != null)
            {
                int index = SelectedItem.GetIndex();
                if (index == -1)
                    throw new InvalidOperationException("Selected Item is not a member of the CommandListView. ");

                return SelectedItem.ParentList.AddCommandAt(index + 1, command);
            }
            else
            {
                return AddCommandAt(Children().Count(), command);
            }
        }

        private void UpdateStripe()
        {
            
        }
        
        public static void Move(CommandItem source, CommandListView dest, int index)
        {
            //Debug.Log($"{source.GetIndex()} to {index}");
            if (source.ParentList == dest && source.GetIndex() == index) return;

            PersistentSerializedProperty sourcePProp = source.CommandProperty;

            if (dest.CommandArrayProperty.AbsolutePath.StartsWith(sourcePProp.AbsolutePath))
            {
                EditorUtility.DisplayDialog("", "コマンドをそれ自身の子に移動することはできません。", "OK");
                return;
            }
            var destItem = dest.AddCommandAt(index, sourcePProp);
            
            //コピー元の削除
            //データの削除
            var sourceIndex = sourcePProp.GetArrayIndex();
            source.ParentList.RemoveCommandAt(sourceIndex);
            
            //コピー先の選択
            destItem.Select();
        }
        
        private void CopyCommandProperty(SerializedProperty source, SerializedProperty dest)
        {
            //Debug.Log($"{source.propertyPath} to {dest.propertyPath}");
            if (source.propertyType != SerializedPropertyType.ManagedReference) return;
            if (dest.propertyType != SerializedPropertyType.ManagedReference) return;
            CopySerializedProperty(source, dest);
        }

        private static void CopySerializedProperty(SerializedProperty source, SerializedProperty dest)
        {
            //まずは自身の値をコピー
            //Debug.Log($"{source.propertyPath} to {dest.propertyPath}");
            switch (source.propertyType)
            {
                case SerializedPropertyType.Generic:
                    //Do nothing
                    if (source.isArray)
                    {
                        dest.arraySize = source.arraySize;
                    }
                    break;
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.LayerMask:
                    dest.intValue = source.intValue;
                    break;
                case SerializedPropertyType.Boolean:
                    dest.boolValue = source.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    dest.floatValue = source.floatValue;
                    break;
                case SerializedPropertyType.String:
                    dest.stringValue = source.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    dest.colorValue = source.colorValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    dest.objectReferenceValue = source.objectReferenceValue;
                    break;
                case SerializedPropertyType.Enum:
                    dest.enumValueIndex = source.enumValueIndex;
                    break;
                case SerializedPropertyType.Vector2:
                    dest.vector2Value = source.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    dest.vector3Value = source.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    dest.vector4Value = source.vector4Value;
                    break;
                case SerializedPropertyType.Rect:
                    dest.rectValue = source.rectValue;
                    break;
                case SerializedPropertyType.ArraySize:
                    //dest.arraySize = source.arraySize;
                    break;
                case SerializedPropertyType.Character:
                    throw new NotImplementedException();
                case SerializedPropertyType.AnimationCurve:
                    dest.animationCurveValue = source.animationCurveValue;
                    break;
                case SerializedPropertyType.Bounds:
                    dest.boundsValue = source.boundsValue;
                    break;
                case SerializedPropertyType.Gradient:
                    throw new NotImplementedException();
                case SerializedPropertyType.Quaternion:
                    dest.quaternionValue = source.quaternionValue;
                    break;
                case SerializedPropertyType.ExposedReference:
                    dest.exposedReferenceValue = source.exposedReferenceValue;
                    break;
                case SerializedPropertyType.FixedBufferSize:

                    break;
                case SerializedPropertyType.Vector2Int:
                    dest.vector2IntValue = source.vector2IntValue;
                    break;
                case SerializedPropertyType.Vector3Int:
                    dest.vector3IntValue = source.vector3IntValue;

                    break;
                case SerializedPropertyType.RectInt:
                    dest.rectIntValue = source.rectIntValue;

                    break;
                case SerializedPropertyType.BoundsInt:
                    dest.boundsIntValue = source.boundsIntValue;
                    break;
                case SerializedPropertyType.ManagedReference:
                    var typeInfo = source.managedReferenceFullTypename.Split(' ');
                    if (typeInfo.Length >= 2)
                    {
                        var assemblyName = typeInfo[0];
                        var typeName = typeInfo[1];

                        var assembly = AppDomain.CurrentDomain.GetAssemblies()
                            .FirstOrDefault(a => a.GetName().Name == assemblyName);
                        if (assembly == null)
                        {
                            Debug.LogError(
                                $"SerializedProperty managed reference copying failed: Missing Assembly \"{assemblyName}\"",
                                source.serializedObject.targetObject);
                            return;
                        }

                        var type = assembly.GetType(typeName);

                        dest.managedReferenceValue = Activator.CreateInstance(type);
                    }
                    else
                    {
                        dest.managedReferenceValue = null;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //子プロパティを再帰的にコピー
            if (source.hasVisibleChildren)
            {
                var sourceDepth = source.depth;
                string sourcePath = source.propertyPath;
                var iter = source.Copy();
                iter.Next(true);
                do
                {
                    string relativePath = iter.propertyPath.Substring(sourcePath.Length + 1);
                    var iterDest = dest.FindPropertyRelative(relativePath);
                    CopySerializedProperty(iter, iterDest);
                } while (iter.Next(false) && iter.depth > sourceDepth);
            }
        }
    }


    public class CommandListViewBinding : IBinding
    {
        private Action OnUpdate;

        public CommandListViewBinding(Action onUpdate)
        {
            OnUpdate = onUpdate;
        }

        public void PreUpdate()
        {
        }

        public void Update()
        {
            OnUpdate?.Invoke();
        }

        public void Release()
        {
        }
    }
}