using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    [CustomEditor(typeof(EventScriptAsset))]
    public class EventScriptAssetEditor : Editor
    {
        private EventScriptAsset Target => target as EventScriptAsset;

        private static readonly string visualTreeAssetPath =
            "Packages/io.github.ruccho.mukuro.core/Editor/Layout/EventScriptAsset.uxml";

        public override VisualElement CreateInspectorGUI()
        {
            VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreeAssetPath);
            VisualElement root = uiAsset.CloneTree();

            root.Q<Button>("OpenButton").clickable.clicked += () => { EventScriptEditorWindow.ShowWindow(Target); };

            root.Q<TextField>("IDField").bindingPath = serializedObject.FindProperty("id").propertyPath;
            /*
            root.Q<Button>("RegenerateIDButton").clickable.clicked += () =>
            {
                var id = Guid.NewGuid().ToString();
                serializedObject.FindProperty("id").stringValue = id;
                serializedObject.ApplyModifiedProperties();
            };*/

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(target, out string guid, out long localId))
            {
                var idProp = serializedObject.FindProperty("id");

                if (
                    string.IsNullOrEmpty(idProp.stringValue) ||
                    (
                        (idProp.stringValue != guid) && 
                        EditorUtility.DisplayDialog("Mukuro", $"EventScriptAssetのID({idProp.stringValue})がアセットGUID({guid})と一致していません。修正しますか？", "Yes",
                            "No")
                    )
                )
                {
                    string old = idProp.stringValue;
                    idProp.stringValue = guid;
                    serializedObject.ApplyModifiedProperties();
                    Debug.Log($"[EventScriptAssetEditor] New GUID has been set.\n{old} > {guid}", target);
                }
            }

            return root;
        }
    }
}