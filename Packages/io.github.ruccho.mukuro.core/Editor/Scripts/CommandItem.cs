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
    public class CommandItem : BindableElement
    {
        private static readonly string visualTreeAssetPath =
            "Packages/io.github.ruccho.mukuro.core/Editor/Layout/CommandItem.uxml";

        public EventCommandEditor Editor { get; private set; }

        public PersistentSerializedProperty CommandProperty { get; private set; }

        public CommandListView ParentList { get; }
        private Action<CommandItem> OnSelect;

        public CommandItem(CommandListView parentList, PersistentSerializedProperty property,
            Action<CommandItem> onSelect,
            Action<CommandItem, CommandListView, int> onMove) : base()
        {
            ParentList = parentList;
            CommandProperty = property;
            OnSelect = onSelect;

            VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreeAssetPath);
            VisualElement root = uiAsset.CloneTree();
            this.Add(root);

            this.AddManipulator(new CommandMovableManipulator(this));
            this.AddManipulator(new CommandMoveTargetManipulator(
                ParentList,
                part =>
                {
                    switch (part)
                    {
                        case MovementPart.Top:
                            return GetIndex();
                        case MovementPart.Bottom:
                            return GetIndex() + 1;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(part), part, null);
                    }
                },
                () =>
                {
                    this.style.borderBottomWidth = new StyleFloat(0f);
                    this.style.borderTopWidth = new StyleFloat(3f);
                    this.style.borderTopColor = new StyleColor(new Color(0.2f, 0.3411765f, 0.8509805f));
                },
                () =>
                {
                    this.style.borderTopWidth = new StyleFloat(0f);
                    this.style.borderBottomWidth = new StyleFloat(3f);
                    this.style.borderBottomColor = new StyleColor(new Color(0.2f, 0.3411765f, 0.8509805f));
                },
                () =>
                {
                    this.style.borderTopWidth = new StyleFloat(0f);
                    this.style.borderBottomWidth = new StyleFloat(0f);
                }));

            this.RegisterCallback<MouseDownEvent>(evt =>
            {
                evt.StopPropagation();
                Select();
            });

            var enabledToggle = this.Q<Toggle>("CommandEnabled");
            enabledToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                var prop = CommandProperty.GetProperty();
                var propEnabled = prop.FindPropertyRelative("enabled");
                propEnabled.boolValue = evt.newValue;
                propEnabled.serializedObject.ApplyModifiedProperties();
            });

            string typeName = GetTypeName();

            var customContentContainer = this.Q<VisualElement>("CommandCustomDetailContent");
            customContentContainer.Clear();
            Editor = CreateEditor(typeName, this, customContentContainer);
            UpdateCommandDetail();
        }

        private string GetTypeName()
        {
            var prop = CommandProperty.GetProperty();
            var typeInfo = prop.managedReferenceFullTypename.Split(' ');

            if (typeInfo.Length >= 2)
            {
                return typeInfo[1];
            }
            else
            {
                return "";
            }
        }

        public void UpdateCommandDetail()
        {
            if (Editor != null)
            {
                var label = this.Q<Label>("CommandLabel");
                label.text = Editor.GetLabelText();
                
                Color? c = Editor.GetLabelColor();
                if (c != null)
                {
                    label.style.color = new StyleColor(c.Value);
                }

                var iconImage = this.Q("CommandIcon");
                Texture2D icon = Editor.GetIcon();
                if (icon == null)
                {
                    iconImage.style.width = 0f;
                }
                else
                {
                    iconImage.style.backgroundImage = icon;
                    iconImage.style.width = 16f;
                }
                

                var prop = CommandProperty.GetProperty();

                var enabledToggle = this.Q<Toggle>("CommandEnabled");
                var enabledProp = prop.FindPropertyRelative("enabled");

                if (enabledProp != null)
                    enabledToggle.value = enabledProp.boolValue;
                else
                {
                    enabledToggle.value = false;
                    enabledToggle.SetEnabled(false);
                }
                
                Editor.OnUpdate();
            }
        }

        public int GetIndex()
        {
            return ParentList.Children().ToList().IndexOf(this);
        }

        public void Select()
        {
            OnSelect(this);
        }

        private static EventCommandEditor CreateEditor(string typeName, CommandItem item,
            VisualElement customContentContainer)
        {
            EventCommandEditor editor = null;
            if (!string.IsNullOrEmpty(typeName))
            {
                var mukuroEditorAssemblyFullName = Assembly.GetAssembly(typeof(EventCommandEditor)).FullName;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                    a.GetReferencedAssemblies()
                        .FirstOrDefault(
                            n => n.FullName == mukuroEditorAssemblyFullName
                        ) != default || a.FullName == mukuroEditorAssemblyFullName
                );

                IEnumerable<Type> editorTypes = null;
                foreach (var assembly in assemblies)
                {
                    var typesInAssembly =
                        assembly
                            .GetTypes().Where(t =>
                            {
                                return t.IsSubclassOf(typeof(EventCommandEditor)) && !t.IsAbstract;
                            });

                    if (editorTypes == null)
                    {
                        editorTypes = typesInAssembly;
                    }
                    else
                    {
                        editorTypes = editorTypes.Concat(typesInAssembly);
                    }
                }

                foreach (var editorType in editorTypes)
                {
                    object[] attributes =
                        editorType.GetCustomAttributes(typeof(CustomEventCommandEditorAttribute), false);
                    if (attributes == null || attributes.Length == 0)
                    {
                        //No Custom Attributes
                        continue;
                    }
                    else
                    {
                        //該当TypeのAttributeを取得
                        var matches = attributes.Where((a) =>
                            (a as CustomEventCommandEditorAttribute).CommandType.FullName == typeName);
                        if (matches.Count() == 0)
                        {
                            continue;
                        }
                        else
                        {
                            editor =
                                Activator.CreateInstance(editorType, item, customContentContainer) as
                                    EventCommandEditor;
                        }
                    }
                }
            }


            if (editor == null)
            {
                editor = new GenericEventCommandEditor(item, customContentContainer);
            }

            editor.OnCreated();
            return editor;
        }

        private Color defaultBackgroundColor = default;

        public void OnSelected()
        {
            //Debug.Log(Property.propertyPath);
            if (defaultBackgroundColor == default)
                defaultBackgroundColor = this.style.backgroundColor.value;
            this.style.backgroundColor = new StyleColor(new Color(0.172549f, 0.3647059f, 0.5294118f));
            UpdateCommandDetail();
        }

        public void OnDeselected()
        {
            this.style.backgroundColor = new StyleColor(defaultBackgroundColor);
        }
    }

    public static class CommandMovementManager
    {
        private static CommandMovableManipulator currentMoving;
        private static CommandMoveTargetManipulator currentHovering;
        private static MovementPart currentPart;

        public static void StartMovement(CommandMovableManipulator target)
        {
            currentMoving = target;
        }

        public static void Hover(CommandMoveTargetManipulator target, MovementPart part)
        {
            if (currentMoving == null) return;

            if (target != currentHovering && currentHovering != null)
            {
                currentHovering.ReleaseHovering();
            }

            currentPart = part;
            currentHovering = target;
            currentHovering.SetHovering(part);
        }


        public static void ApplyMovement(CommandMoveTargetManipulator target)
        {
            if (currentMoving == null) return;
            if (currentHovering == null) return;
            
            currentHovering.ReleaseHovering();
            
            var index = currentHovering.GetIndexFunc(currentPart);
            var list = currentHovering.CommandList;

            currentMoving.Move(list, index);

            AbortMovement();
        }

        public static void AbortMovement()
        {
            currentMoving = null;
            currentHovering = null;
        }
    }

    public class CommandMoveTargetManipulator : MouseManipulator
    {
        public CommandListView CommandList { get; }

        private Action OnHoverTop { get; }
        private Action OnHoverBottom { get; }
        private Action OnHoverRelease { get; }
        public Func<MovementPart, int> GetIndexFunc { get; }

        public CommandMoveTargetManipulator(CommandListView list, Func<MovementPart, int> getIndexFunc, Action onHoverTop,
            Action onHoverBottom,
            Action onHoverRelease)
        {
            CommandList = list;
            GetIndexFunc = getIndexFunc;
            this.OnHoverTop = onHoverTop;
            this.OnHoverBottom = onHoverBottom;
            this.OnHoverRelease = onHoverRelease;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (evt.pressedButtons == 0)
            {
                CommandMovementManager.AbortMovement();
                return;
            }


            float y = evt.localMousePosition.y;
            float half = target.layout.height * 0.5f;

            if (y < half)
            {
                //top
                CommandMovementManager.Hover(this, MovementPart.Top);
            }
            else
            {
                //bottom
                CommandMovementManager.Hover(this, MovementPart.Bottom);
            }

            evt.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            CommandMovementManager.ApplyMovement(this);
            evt.StopPropagation();
        }

        public void SetHovering(MovementPart part)
        {
            switch (part)
            {
                case MovementPart.Top:
                    OnHoverTop?.Invoke();
                    break;
                case MovementPart.Bottom:
                    OnHoverBottom?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(part), part, null);
            }
        }

        public void ReleaseHovering()
        {
            OnHoverRelease?.Invoke();
        }
    }

    public enum MovementPart
    {
        Top,
        Bottom
    }

    public class CommandMovableManipulator : MouseManipulator
    {
        public CommandMovableManipulator(CommandItem item)
        {
            //if (target != item) throw new ArgumentException("Target must be CommandItem.");
        }


        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            evt.StopPropagation();
            CommandMovementManager.StartMovement(this);
        }

        public void Move(CommandListView dest, int index)
        {
            CommandListView.Move(target as CommandItem, dest, index);
        }
    }
}