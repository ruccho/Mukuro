using System;
using System.Linq;
using Mukuro.Editors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Dialog.Editors
{
    [CustomEventCommandEditor(typeof(DialogShowMessageCommand), IconTexturePath = iconPath)]
    public class DialogShowMessageCommandEditor : EventCommandEditor
    {
        private const string iconPath =
            "Packages/io.github.ruccho.mukuro.dialog/Editor/Mukuro/CommandEditors/Icons/DialogShowMessage.png";
        private static readonly string EditorPrefsDefaultSettingsTypeNameKey =
            "Mukuro.Dialog.Editors.DialogShowMessageCommandEditor.DefaultSettingsTypeName";
        
        private static Type[] DialogShowMessageSettingsTypes;

        [InitializeOnLoadMethod]
        private static void GatherSettingsTypes()
        {
            var t_dialogProviderBase = typeof(DialogShowMessageSettings);
            DialogShowMessageSettingsTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
                a.GetTypes().Where(t => t.IsSubclassOf(t_dialogProviderBase) && !t.IsAbstract)).ToArray();
        }

        private static Texture2D Icon { get; set; }

        private static void EnsureIconLoaded()
        {
            if (Icon == null)
                Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        }

        public DialogShowMessageCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(
            commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            var settingsProp = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("settings");
            var messageProp = settingsProp.FindPropertyRelative("message");
            if (messageProp != null)
            {
                var message = messageProp.stringValue;
                message = message.Replace('\n', ' ');
                return $"{message}";
            }
            else
            {
                return "";
            }
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

        public override VisualElement CreateCommandEditorGUI()
        {
            var root = new VisualElement();
            BuildCommandEditor(root);
            return root;
        }


        private void BuildCommandEditor(VisualElement root)
        {
            var commandProp = CommandItem.CommandProperty.GetProperty();


            //DialogShowMessageSettings
            var settingsProp = commandProp.FindPropertyRelative("settings");

            var settingsTypeInfo = settingsProp.managedReferenceFullTypename.Split(' ');
            string settingsTypeName = "";
            if (settingsTypeInfo.Length >= 2)
            {
                settingsTypeName = settingsTypeInfo[1];
            }
            else
            {
                //Settingsが未設定(null)
                
                object settingsInstance;
                
                //直前に使用したSettingsを使用する
                if (EditorPrefs.HasKey(EditorPrefsDefaultSettingsTypeNameKey))
                {
                    var defaultSettingsTypeName = EditorPrefs.GetString(EditorPrefsDefaultSettingsTypeNameKey);
                    var settingsType = Type.GetType(defaultSettingsTypeName);
                    if (settingsType == null)
                    {
                        settingsType = typeof(DialogShowMessageSettings);
                    }

                    settingsInstance = Activator.CreateInstance(settingsType);
                }
                else
                {
                    settingsInstance = new DialogShowMessageSettings();
                }

                settingsProp.managedReferenceValue = settingsInstance;
                settingsProp.serializedObject.ApplyModifiedProperties();

                settingsTypeName = settingsInstance.GetType().FullName;
            }

            var settingsField = new VisualElement();

            var settingsButton = new Button();
            settingsButton.text = settingsTypeName.Split('.').Last();
            settingsButton.clickable.clicked += () => SettingsTypeMenu(settingsButton, settingsField);

            var settingsTypeBox = new VisualElement();
            settingsTypeBox.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            settingsTypeBox.Add(new Label("Settings Type"));
            settingsTypeBox.Add(settingsButton);

            root.Add(settingsTypeBox);

            UpdateSettingsField(settingsField);

            root.Add(settingsField);

            root.Bind(CommandItem.CommandProperty.SerializedObject);
        }

        private void UpdateSettingsField(VisualElement settingsField)
        {
            var root = settingsField;
            root.Clear();

            var commandProp = CommandItem.CommandProperty.GetProperty();
            var settingsProp = commandProp.FindPropertyRelative("settings");
            root.Add(new PropertyField() {bindingPath = settingsProp.propertyPath});
            /*if (settingsProp.hasChildren)
            {
                int rootDepth = settingsProp.depth;
                settingsProp.NextVisible(true);

                do
                {
                    PropertyField propertyField = new PropertyField();
                    propertyField.bindingPath = settingsProp.propertyPath;
                    root.Add(propertyField);
                } while (settingsProp.NextVisible(false) && settingsProp.depth > rootDepth);
            }*/
            root.Bind(CommandItem.CommandProperty.SerializedObject);
        }

        private void SettingsTypeMenu(Button settingsButton, VisualElement settingsField)
        {
            var commandProp = CommandItem.CommandProperty.GetProperty();
            var settingsProp = commandProp.FindPropertyRelative("settings");
            var settingsTypeInfo = settingsProp.managedReferenceFullTypename.Split(' ');
            string settingsTypeName = "";
            if (settingsTypeInfo.Length >= 2)
            {
                settingsTypeName = settingsTypeInfo[1];
            }
            else
            {
                settingsProp.managedReferenceValue = new DialogShowMessageSettings();
                settingsProp.serializedObject.ApplyModifiedProperties();
                settingsTypeName = typeof(DialogShowMessageSettings).FullName;
            }

            var settingsMenu = new GenericMenu();
            settingsMenu.AddItem(new GUIContent(nameof(DialogShowMessageSettings)),
                settingsTypeName == typeof(DialogShowMessageSettings).FullName, () =>
                {
                    settingsButton.text = nameof(DialogShowMessageSettings);
                    SetSettings(new DialogShowMessageSettings(), settingsField);
                });
            settingsMenu.AddSeparator("");
            foreach (var t in DialogShowMessageSettingsTypes)
            {
                settingsMenu.AddItem(new GUIContent(t.Name), settingsTypeName == t.FullName, () =>
                {
                    settingsButton.text = t.Name.Split('.').Last();
                    SetSettings((DialogShowMessageSettings) Activator.CreateInstance(t), settingsField);
                });
            }

            settingsMenu.DropDown(settingsButton.worldBound);
        }

        private void SetSettings(DialogShowMessageSettings settings, VisualElement settingsField)
        {
            var commandProp = CommandItem.CommandProperty.GetProperty();
            var settingsProp = commandProp.FindPropertyRelative("settings");
            settingsProp.managedReferenceValue = settings;
            settingsProp.serializedObject.ApplyModifiedProperties();
            UpdateSettingsField(settingsField);
            
            //次回以降のデフォルトに設定
            EditorPrefs.SetString(EditorPrefsDefaultSettingsTypeNameKey, settings.GetType().AssemblyQualifiedName);
        }
/*
        private VisualElement BuildFaceField()
        {
            var commandProp = CommandItem.CommandProperty.GetProperty();
            var speakerInfo = (commandProp.FindPropertyRelative("speaker").objectReferenceValue as SpeakerInfoAsset);
            if (!speakerInfo)
            {
                var p = new PropertyField();
                p.label = "表情";
                p.bindingPath = commandProp.FindPropertyRelative("face").propertyPath;
                return p;
            }

            var faces = speakerInfo.SpeakerInfo.Faces.ToList();
            faces.Insert(0, "<Missing>");
            var currentFaceProp = commandProp.FindPropertyRelative("face");
            var currentFace = currentFaceProp.stringValue;
            var currentFaceIndex = 0;
            if (faces.Contains(currentFace))
            {
                currentFaceIndex = faces.IndexOf(currentFace);
            }
            else
            {
                if (faces.Count >= 2)
                {
                    currentFaceIndex = 1;
                    currentFaceProp.stringValue = faces[1];
                    currentFaceProp.serializedObject.ApplyModifiedProperties();
                }
            }

            var face = new PopupField<string>(faces, currentFaceIndex);
            face.label = "表情";
            face.RegisterValueChangedCallback((e) =>
            {
                if (e.newValue == "<Missing>")
                {
                    face.value = e.previousValue;
                    return;
                }

                currentFaceProp.stringValue = e.newValue;
                currentFaceProp.serializedObject.ApplyModifiedProperties();
            });
            return face;
        }
        */
    }
}