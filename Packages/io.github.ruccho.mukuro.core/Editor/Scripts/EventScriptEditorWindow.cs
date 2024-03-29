﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mukuro.Editors
{
    public class EventScriptEditorWindow : EditorWindow
    {
        //コンパイル後にイベントを再度開くのに使用
        [SerializeField] private string selectedAssetGUID;

        private static readonly string visualTreeAssetPath =
            "Packages/io.github.ruccho.mukuro.core/Editor/Layout/EventScriptEditor.uxml";

        private static StyleSheet defaultCommonDarkStyleSheet;
        private static StyleSheet defaultCommonLightStyleSheet;

        private static void GetSkins()
        {
            //UnityEditor.UIElementsのアセンブリを取得
            var assembly = typeof(Toolbar).Assembly;

            //UIElementsEditorUtilityのTypeを取得
            var type = assembly.GetType("UnityEditor.UIElements.UIElementsEditorUtility");
            if (type == null)
            {
                Debug.LogWarning($"Type UnityEditor.UIElements.UIElementsEditorUtility is missing.");
                return;
            }

            //StyleSheetを格納するフィールドを取得
            var darkField = type.GetField("s_DefaultCommonDarkStyleSheet",
                BindingFlags.Static | BindingFlags.NonPublic);
            var lightField = type.GetField("s_DefaultCommonLightStyleSheet",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (darkField == null || lightField == null) return;

            //staticなフィールドとして値を取得
            defaultCommonDarkStyleSheet = (StyleSheet) darkField.GetValue(null);
            defaultCommonLightStyleSheet = (StyleSheet) lightField.GetValue(null);
        }

        public static void ShowWindow(EventScriptAsset scriptAsset)
        {
            var window = EditorWindow.GetWindow(typeof(EventScriptEditorWindow)) as EventScriptEditorWindow;
            window.Init(scriptAsset);
        }

        private VisualElement commandList;

        //private CommandListView rootCommandListView;
        private CommandEditorDomain domain;

        private EventScriptAsset target;
        private UnityEngine.Object contextObject;

        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(selectedAssetGUID))
            {
                var path = AssetDatabase.GUIDToAssetPath(selectedAssetGUID);
                if (!string.IsNullOrEmpty(path))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<EventScriptAsset>(path);
                    if (asset)
                    {
                        Init(asset);
                    }
                }
            }
        }

        private void Init(EventScriptAsset scriptAsset)
        {
            Init(scriptAsset, FindReferenceHostFromScene(scriptAsset));
        }

        private EventRuntimeReferenceHost FindReferenceHostFromScene(EventScriptAsset scriptAsset)
        {
            var references = FindObjectsOfType<EventRuntimeReferenceHost>();
            foreach (var r in references)
            {
                if (r.TargetAssetId == scriptAsset.Id)
                {
                    return r;
                }
            }

            return null;
        }

        private void Init(EventScriptAsset scriptAsset, UnityEngine.Object context)
        {
            selectedAssetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(scriptAsset));

            target = scriptAsset;
            VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreeAssetPath);
            VisualElement root = uiAsset.CloneTree();

            GetSkins();

            rootVisualElement.styleSheets.Clear();
            rootVisualElement.styleSheets.Add(defaultCommonDarkStyleSheet);

            
            root.style.flexGrow = 1f;

            SerializedObject sObj;
            contextObject = context;
            if (context)
            {
                sObj = new SerializedObject(scriptAsset, context);
            }
            else
            {
                sObj = new SerializedObject(scriptAsset);
            }

            var commandsProperty = sObj.FindProperty("script.CommandList.commands");

            //メニューボタンのハンドラ
            root.Q<Button>("RefreshButton").clickable.clicked += Refresh;
            root.Q<Button>("SaveAssetButton").clickable.clicked += SaveAsset;
            root.Q<Button>("SelectButton").clickable.clicked += () => { EditorGUIUtility.PingObject(scriptAsset); };
            var contextObjectField = root.Q<ObjectField>("ContextObject");
            contextObjectField.objectType = typeof(EventRuntimeReferenceHost);
            contextObjectField.value = context;
            root.Q<ObjectField>("ContextObject").RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
            {
                var newContext = evt.newValue;
                this.Init(target, newContext);
            });

            root.Q<Button>("FindReferenceHostButton").clickable.clicked += () => { Init(scriptAsset); };

            root.Q<Button>("CreateReferenceHostButton").clickable.clicked += () =>
            {
                var found = FindReferenceHostFromScene(scriptAsset);

                if (found)
                {
                    if (!EditorUtility.DisplayDialog("",
                        "現在のシーンにはすでにこのEventScriptAssetのEventRuntimeReferenceHostが存在します。重複によって実行時の自動参照が行われなくなるおそれがありますが、続行しますか？",
                        "はい", "キャンセル"))
                    {
                        return;
                    }
                }


                var referenceHost = new GameObject();
                referenceHost.name = scriptAsset.name + " RuntimeReference";
                var host = referenceHost.AddComponent<EventRuntimeReferenceHost>();
                host.TargetAsset = scriptAsset;
                Init(scriptAsset, host);
            };


            //コマンドリストの構築
            commandList = root.Q<VisualElement>("CommandList");
            commandList.Clear();

            domain = new CommandEditorDomain(commandsProperty, OnItemSelected);

            var rootCommandListView =
                domain.RootCommandListView; //new CommandListView(OnItemSelected, commandsProperty);

            rootCommandListView.style.paddingBottom = 100f;

            commandList.Add(rootCommandListView);

            //コマンド追加メニューの構成
            var addCommandList = root.Q<ScrollView>("AddCommandList");
            addCommandList.Clear();

            //-定義済みコマンドを列挙
            Dictionary<EventCommandAttribute, Type> definedCommands = new Dictionary<EventCommandAttribute, Type>();

            var commandTypes = EventCommandUtility.GetAllEventCommandTypes();
            foreach (var commandType in commandTypes)
            {
                object[] attributes = commandType.GetCustomAttributes(typeof(EventCommandAttribute), false);
                if (attributes.Length == 0)
                {
                    //EventCommand属性がついていないので、カテゴリなしとして分類
                    definedCommands.Add(new EventCommandAttribute("カテゴリなし", commandType.Name), commandType);
                    continue;
                }
                else
                {
                    foreach (var attribute in attributes)
                    {
                        var commandAttribute = attribute as EventCommandAttribute;
                        definedCommands.Add(commandAttribute, commandType);
                    }
                }
            }

            //-要素の構築

            Dictionary<string, Foldout> categories = new Dictionary<string, Foldout>();
            foreach (var definedCommand in definedCommands)
            {
                string category = definedCommand.Key.Category;
                if (!categories.ContainsKey(category))
                {
                    var foldout = new Foldout() {text = category};
                    addCommandList.Add(foldout);
                    categories.Add(category, foldout);
                }

                var commandType = definedCommand.Value;


                var button = new Button() {text = definedCommand.Key.DisplayName};
                button.clickable.clicked += () =>
                {
                    var command = Activator.CreateInstance(commandType) as EventCommand;
                    var newCommand = domain.AddCommandAtSelected(command);
                    //選択
                    newCommand.Select();
                };
                categories[category].Add(button);
                
                var editorType = EventCommandUtility.GetCommandEditorType(commandType);

                if (editorType == null) continue;

                var editorAttribute = editorType.GetCustomAttributes(typeof(CustomEventCommandEditorAttribute))
                    .OfType<CustomEventCommandEditorAttribute>().FirstOrDefault();

                var path = editorAttribute.IconTexturePath;

                if (!string.IsNullOrEmpty(path))
                {
                    var iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    var icon = new Image {image = iconTexture};
                    icon.style.width = new StyleLength(new Length(16f, LengthUnit.Pixel));
                    icon.style.height = new StyleLength(new Length(16f, LengthUnit.Pixel));
                    button.Add(icon);
                }
            }

            //キーボードショートカット
            rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Delete)
                {
                    domain.RemoveCommandAtSelected();
                }
            });

            titleContent = new GUIContent("EventScriptEditor");
            rootVisualElement.Clear();
            rootVisualElement.Add(root);
        }

        private void Refresh()
        {
            Init(target, contextObject);
        }

        private void SaveAsset()
        {
            if (target)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private void OnItemSelected(CommandItem item)
        {
            var commandEditor = rootVisualElement.Q<VisualElement>("CommandEditor");
            if (commandEditor == null) return;
            commandEditor.Clear();

            if (item == null) return;
            
            var editor = item.Editor;
            var customCommandEditor = editor.CreateCommandEditorGUI();
            commandEditor.Add(customCommandEditor);
        }
    }
}