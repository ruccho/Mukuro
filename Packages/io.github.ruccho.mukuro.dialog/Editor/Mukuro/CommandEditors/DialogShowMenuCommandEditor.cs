using System.Collections;
using System.Collections.Generic;
using Mukuro.Editors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Dialog.Editors
{
    [CustomEventCommandEditor(typeof(DialogShowMenuCommand), IconTexturePath = iconPath)]
    public class DialogShowMenuCommandEditor : EventCommandEditor
    {

        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.dialog/Editor/Mukuro/CommandEditors/Icons/DialogShowMenu.png";
        private VisualElement CommandEditorRoot { get; set; }
        private List<Foldout> DefaultLabel { get; } = new List<Foldout>();

        private static Texture2D Icon { get; set; }

        private static void EnsureIconLoaded()
        {
            if (Icon == null)
                Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        }
        
        public DialogShowMenuCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(commandItem,
            customDetailRoot)
        {
            
        }

        public override string GetLabelText()
        {
            return $"選択肢の表示";
        }

        public override Color? GetLabelColor()
        {
            if (ColorUtility.TryParseHtmlString("#e38b4e", out Color c))
            {
                return c;
            }

            return null;
        }

        public override Texture2D GetIcon()
        {
            EnsureIconLoaded();
            return Icon;
        }

        public override void OnUpdate()
        {
            //Update Labels

            var bifurcationsProp = CommandItem.CommandProperty.GetChildProperty("bifurcations");

            var count = bifurcationsProp.GetProperty().arraySize;

            if (DefaultLabel.Count != count)
            {
                RebuildDetails();
                return;
            }

            for (int i = 0; i < count; i++)
            {
                var item = bifurcationsProp.GetArrayElementAt(i);
                var labelProp = item.GetChildProperty("label").GetProperty();
                DefaultLabel[i].text = $"「{labelProp.stringValue}」";
            }
        }

        public override void OnCreated()
        {
            RebuildDetails();
        }


        private void RebuildDetails()
        {
            CustomDetailRoot.Clear();
            DefaultLabel.Clear();

            var bifurcationsProp = CommandItem.CommandProperty.GetChildProperty("bifurcations");

            var count = bifurcationsProp.GetProperty().arraySize;

            for (int i = 0; i < count; i++)
            {
                var bifurcationBox = new VisualElement();

                var item = bifurcationsProp.GetArrayElementAt(i);
                var labelProp = item.GetChildProperty("label").GetProperty();

                var a = new Foldout();
                a.text = $"「{labelProp.stringValue}」"; // new Label($"「{labelProp.stringValue}」");
                var toggle = a.Q<Toggle>();
                toggle.style.marginBottom = 0f;
                a.style.marginTop = 10f;
                toggle.style.backgroundColor = new Color(0, 0, 0, 0.5f);
                bifurcationBox.Add(a);
                DefaultLabel.Add(a);

                var l = new CommandListView(CommandItem.ParentList, item.GetChildProperty("commandList.commands"));
                a.Add(l);

                CustomDetailRoot.Add(bifurcationBox);
            }
        }

        public override VisualElement CreateCommandEditorGUI()
        {
            if (CommandEditorRoot == null)
            {
                RebuildCommandEditor();
            }

            return CommandEditorRoot;
        }

        private void RebuildCommandEditor()
        {
            var root = CommandEditorRoot ?? new VisualElement();
            root.Clear();
            var bifurcations = new VisualElement();

            var bifurcationsProp = CommandItem.CommandProperty.GetChildProperty("bifurcations").GetProperty();
            for (int i = 0; i < bifurcationsProp.arraySize; i++)
            {
                var item = bifurcationsProp.GetArrayElementAtIndex(i);

                var b = new VisualElement();
                b.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

                var labelText = new TextField();
                labelText.bindingPath = item.FindPropertyRelative("label").propertyPath;
                labelText.style.flexGrow = new StyleFloat(1f);
                
                labelText.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
                labelText.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
                b.Add(labelText);

                int index = i;

                var removeButton = new Button();
                removeButton.text = "-";
                removeButton.clickable.clicked += () => { RemoveBifurcation(index); };
                b.Add(removeButton);

                var addButton = new Button();
                addButton.text = "+";
                addButton.clickable.clicked += () => { AddBifurcation(index + 1); };
                b.Add(addButton);


                bifurcations.Add(b);
            }

            var addLastButton = new Button();
            addLastButton.text = "+";
            addLastButton.clickable.clicked += () => { AddBifurcation(bifurcationsProp.arraySize); };
            root.Add(addLastButton);

            bifurcations.Bind(bifurcationsProp.serializedObject);

            root.Add(bifurcations);
            CommandEditorRoot = root;
        }

        private void AddBifurcation(int index)
        {
            //データの更新
            var bifurcationsProp = CommandItem.CommandProperty.GetChildProperty("bifurcations");
            bifurcationsProp.InsertArrayElementAt(index);
            bifurcationsProp.SerializedObject.ApplyModifiedProperties();

            //UIの更新
            //リストの追加
            var bifurcationBox = new VisualElement();

            var item = bifurcationsProp.GetArrayElementAt(index);
            var labelProp = item.GetChildProperty("label").GetProperty();

            var a = new Foldout();
            a.text = $"「{labelProp.stringValue}」";
            var toggle = a.Q<Toggle>();
            toggle.style.marginBottom = 0f;
            a.style.marginTop = 10f;
            toggle.style.backgroundColor = new Color(0, 0, 0, 0.5f);
            bifurcationBox.Add(a);
            DefaultLabel.Insert(index, a);

            item.GetProperty().FindPropertyRelative("commandList.commands").arraySize = 0;
            bifurcationsProp.SerializedObject.ApplyModifiedProperties();
            var commandArrayProp = item.GetChildProperty("commandList.commands");

            var l = new CommandListView(CommandItem.ParentList, commandArrayProp);
            a.Add(l);

            CustomDetailRoot.Insert(index, bifurcationBox);

            bifurcationsProp.SerializedObject.ApplyModifiedProperties();

            //CommandEditorの更新
            RebuildCommandEditor();
        }

        private void RemoveBifurcation(int index)
        {
            //データの更新
            var bifurcationsProp = CommandItem.CommandProperty.GetChildProperty("bifurcations");
            bifurcationsProp.DeleteArrayElementAt(index);
            bifurcationsProp.SerializedObject.ApplyModifiedProperties();

            //UIの更新
            //リストの追加
            CustomDetailRoot.RemoveAt(index);
            DefaultLabel.RemoveAt(index);

            //CommandEditorの更新
            RebuildCommandEditor();
        }

        //private void RemoveBifurcation
    }
}