﻿using System.Collections;
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
        private static readonly string visualTreeAssetPath = "Packages/io.github.ruccho.mukuro.core/Editor/Layout/EventScriptAsset.uxml";
        public override VisualElement CreateInspectorGUI()
        {
            VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreeAssetPath);
            VisualElement root = uiAsset.CloneTree();
            
            root.Q<Button>("OpenButton").clickable.clicked += () =>
            {
                EventScriptEditorWindow.ShowWindow(Target);
            };
            
            return root;
        }
    }
}