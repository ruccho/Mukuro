using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Dialog.Editors
{
    [CustomPropertyDrawer(typeof(DialogShowMessageSettings))]
    public class DialogShowMessageSettingsEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();


            var message = new TextField();
            message.multiline = true;
            message.label = "メッセージ";
            message.bindingPath = property.FindPropertyRelative("message").propertyPath;
            message.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            message.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
            root.Add(message);

            int faceIndex = -1;

            var speaker = BuildSpeakerField(property.FindPropertyRelative("speakerInfo"), asset =>
            {
                root.RemoveAt(faceIndex);
                root.Insert(faceIndex, BuildFaceField(property, asset));
            }); //new ObjectField("話者");
            //speaker.objectType = typeof(SpeakerInfoAsset);
            /*speaker.bindingPath =
                property.FindPropertyRelative("speakerInfo").propertyPath;*/
            root.Add(speaker);

            var face = BuildFaceField(property,
                property.FindPropertyRelative("speakerInfo").objectReferenceValue as SpeakerInfoAsset);
            root.Add(face);
            faceIndex = root.IndexOf(face);

            var allowSpeedUp = new PropertyField();
            allowSpeedUp.label = "文字送り加速を許可";
            allowSpeedUp.bindingPath =
                property.FindPropertyRelative("allowSpeedUp").propertyPath;
            root.Add(allowSpeedUp);

            var allowSkipping = new PropertyField();
            allowSkipping.label = "文字送りスキップを許可";
            allowSkipping.bindingPath =
                property.FindPropertyRelative("allowSkipping").propertyPath;
            root.Add(allowSkipping);

            /*
            speaker.RegisterValueChangedCallback((e) =>
            {
            });
            */

            return root;
        }

        private VisualElement BuildSpeakerField(SerializedProperty property, Action<SpeakerInfoAsset> updateFaceField)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) throw new ArgumentException();

            var currentObject = property.objectReferenceValue;

            var assets = Resources.FindObjectsOfTypeAll<SpeakerInfoAsset>().ToList();
            assets.Insert(0, null);


            int index = -1;
            if (currentObject is SpeakerInfoAsset currentSpeakerInfoAsset)
            {
                index = assets.IndexOf(currentSpeakerInfoAsset);
            }
            else if (currentObject == null) index = 0;

            if (index < 0)
            {
                return new ObjectField("話者")
                {
                    bindingPath = property.propertyPath
                };
            }

            Func<SpeakerInfoAsset, string> formatter =  asset => asset ? asset.SpeakerInfo.Name : "(None)";
            var menu = new PopupField<SpeakerInfoAsset>(assets, index, formatter, formatter);
            menu.label = "話者";

            menu.RegisterValueChangedCallback(evt =>
            {
                property.objectReferenceValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
                updateFaceField.Invoke(evt.newValue);
            });


            return menu;
        }

        private VisualElement BuildFaceField(SerializedProperty property, SpeakerInfoAsset speakerInfo)
        {
            if (!speakerInfo)
            {
                var p = new PropertyField();
                p.label = "表情";
                p.bindingPath = property.FindPropertyRelative("face").propertyPath;
                return p;
            }

            var faces = speakerInfo.SpeakerInfo.Faces.ToList();
            faces.Insert(0, "<Missing>");
            var currentFaceProp = property.FindPropertyRelative("face");
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
    }
}