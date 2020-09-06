using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mukuro.Editors;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Dialog.Editors
{
    [CustomEventCommandEditor(typeof(DialogShowMessageCommand))]
    public class DialogShowMessageCommandEditor : EventCommandEditor
    {
        private static Texture2D Icon { get; set; }

        private static void EnsureIconLoaded()
        {
            if (Icon == null)
                Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/io.github.ruccho.mukuro.dialog/Editor/Mukuro/CommandEditors/Icons/DialogShowMessage.png");
        }

        public DialogShowMessageCommandEditor(CommandItem commandItem, VisualElement customDetailRoot) : base(
            commandItem, customDetailRoot)
        {
        }

        public override string GetLabelText()
        {
            var message = CommandItem.CommandProperty.GetProperty().FindPropertyRelative("message").stringValue;
            message = message.Replace('\n', ' ');
            return $"メッセージの表示：「{message}」";
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

            var message = new TextField();
            message.multiline = true;
            message.label = "メッセージ";
            message.bindingPath = commandProp.FindPropertyRelative("message").propertyPath;
            root.Add(message);

            var speaker = new ObjectField("話者");
            speaker.objectType = typeof(SpeakerInfoAsset);
            speaker.bindingPath =
                commandProp.FindPropertyRelative("speaker").propertyPath;
            root.Add(speaker);

            var face = BuildFaceField();
            root.Add(face);
            var faceIndex = root.IndexOf(face);
            

            var allowSpeedUp = new PropertyField();
            allowSpeedUp.label = "文字送り加速を許可";
            allowSpeedUp.bindingPath =
                commandProp.FindPropertyRelative("allowSpeedUp").propertyPath;
            root.Add(allowSpeedUp);

            var allowSkipping = new PropertyField();
            allowSkipping.label = "文字送りスキップを許可";
            allowSkipping.bindingPath =
                commandProp.FindPropertyRelative("allowSkipping").propertyPath;
            root.Add(allowSkipping);


            root.Bind(CommandItem.CommandProperty.SerializedObject);
            speaker.RegisterValueChangedCallback((e) =>
            {
                root.RemoveAt(faceIndex);
                root.Insert(faceIndex, BuildFaceField());
                
            });
        }

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
    }
}