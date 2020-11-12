using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    public abstract class EventCommandEditor
    {
        /// <summary>
        /// コマンドリストに表示されるカスタムUI要素のルートを示します。
        /// </summary>
        protected VisualElement CustomDetailRoot { get; }

        /// <summary>
        /// コマンドリスト内の要素を示します。
        /// </summary>
        protected CommandItem CommandItem { get; }

        public EventCommandEditor(CommandItem commandItem, VisualElement customDetailRoot)
        {
            CommandItem = commandItem;
            CustomDetailRoot = customDetailRoot;
        }

        /// <summary>
        /// コマンドリストに表示されるラベルテキストを設定します。
        /// </summary>
        public virtual string GetLabelText()
        {
            var typeInfo = CommandItem.CommandProperty.GetProperty().managedReferenceFullTypename?.Split(' ');
            if (typeInfo?.Length >= 2)
            {
                var type = Type.GetType($"{typeInfo[1]},{typeInfo[0]}");
                var attrs = type?.GetCustomAttributes(typeof(EventCommandAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((EventCommandAttribute) attrs[0]).DisplayName;
                }

                return typeInfo[1];
            }
            else
            {
                return "<Type Missing>";
            }
        }

        public virtual Texture2D GetIcon()
        {
            return null;
        }

        public virtual Color? GetLabelColor()
        {
            return null;
        }

        /// <summary>
        /// EventCommandEditorが作成されたときに呼び出されます。
        /// </summary>
        public virtual void OnCreated()
        {
            //ShowListViewInCustomDetailAttributeがついてるEventCommandListをDetailに追加する
            var commandProp = CommandItem.CommandProperty.GetProperty();
            var typeDesc = commandProp.managedReferenceFullTypename.Split(' ');

            var assemblyName = typeDesc[0];
            var typeName = typeDesc[1];

            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
            if (assembly == default) return;

            var type = assembly.DefinedTypes.FirstOrDefault(t => t.FullName == typeName);
            if (type == default) return;
            
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            var field = fields.FirstOrDefault(f => Enumerable.Any<CustomAttributeData>(f.CustomAttributes,
                a => a.AttributeType == typeof(ShowListViewInCustomDetailAttribute)));
            if (field == default) return;
            if (field.FieldType != typeof(EventCommandList)) return;

            var listProp = CommandItem.CommandProperty.GetChildProperty(field.Name);
            if (listProp.GetProperty() == null) return;

            var list = new CommandListView(CommandItem.ParentList, listProp.GetChildProperty("commands"));
            CustomDetailRoot.Add(list);
        }

        /// <summary>
        /// このイベントコマンドの情報をUIに反映する処理を記述します。
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// コマンドエディタに表示されるUI要素を構築します。
        /// </summary>
        /// <returns></returns>
        public virtual VisualElement CreateCommandEditorGUI()
        {
            VisualElement root = new VisualElement();
            var prop = CommandItem.CommandProperty.GetProperty().Copy();

            if (prop.hasChildren)
            {
                int rootDepth = prop.depth;
                prop.NextVisible(true);

                do
                {
                    PropertyField propertyField = new PropertyField();
                    propertyField.bindingPath = prop.propertyPath;
                    root.Add(propertyField);
                } while (prop.NextVisible(false) && prop.depth > rootDepth);
            }

            root.Bind(prop.serializedObject);
            return root;
        }
    }

    public sealed class GenericEventCommandEditor : EventCommandEditor
    {
        public GenericEventCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem,
            customDetailRoot)
        {
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnUpdate()
        {
        }
    }
}