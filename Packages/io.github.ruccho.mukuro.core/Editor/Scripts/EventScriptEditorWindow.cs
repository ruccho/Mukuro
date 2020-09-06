using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mukuro.Editors.Utilities;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

namespace Mukuro.Editors
{
    public class EventScriptEditorWindow : EditorWindow
    {
        //コンパイル後にイベントを再度開くのに使用
        [SerializeField] private string selectedAssetGUID;
        
        private static readonly string visualTreeAssetPath =
            "Packages/io.github.ruccho.mukuro.core/Editor/Layout/EventScriptEditor.uxml";
        
        private static readonly string EditorResourcesBasePath = "";
        private static readonly string DefaultCommonDarkStyleSheetPath =
            Path.Combine(EditorResourcesBasePath, "StyleSheets/Generated/DefaultCommonDark.uss.asset");
        private static readonly string DefaultCommonLightStyleSheetPath =
            Path.Combine(EditorResourcesBasePath, "StyleSheets/Generated/DefaultCommonLight.uss.asset");
        internal static StyleSheet DefaultCommonDarkStyleSheet;
        internal static StyleSheet DefaultCommonLightStyleSheet;

        internal static string GetStyleSheetPathForFont(string sheetPath, string fontName)
        {
            return LocalizationDatabase.currentEditorLanguage == SystemLanguage.English ? sheetPath.Replace(".uss", "_" + fontName.ToLowerInvariant() + ".uss") : sheetPath;
        }
        
        public static void ShowWindow(EventScriptAsset scriptAsset)
        {
            
            var window = EditorWindow.GetWindow(typeof(EventScriptEditorWindow)) as EventScriptEditorWindow;
            window.Init(scriptAsset);

        }

        private VisualElement commandList;
        private CommandListView rootCommandListView;

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

            if (DefaultCommonDarkStyleSheet == null || DefaultCommonLightStyleSheet == null)
            {
                string fontName = (string) typeof(EditorResources).GetProperty("currentFontName", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                var darkStyleSheetPath = GetStyleSheetPathForFont(DefaultCommonDarkStyleSheetPath, fontName);
                var lightStyleSheetPath = GetStyleSheetPathForFont(DefaultCommonDarkStyleSheetPath, fontName);
                DefaultCommonDarkStyleSheet = (StyleSheet)EditorGUIUtility.Load(darkStyleSheetPath);
                DefaultCommonLightStyleSheet = (StyleSheet)EditorGUIUtility.Load(lightStyleSheetPath);
            }
            
            rootVisualElement.styleSheets.Clear();
            rootVisualElement.styleSheets.Add(DefaultCommonDarkStyleSheet);
            
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
            rootCommandListView = new CommandListView(OnItemSelected, commandsProperty);

            rootCommandListView.style.paddingBottom = 100f;

            commandList.Add(rootCommandListView);

            //コマンド追加メニューの構成
            var addCommandList = root.Q<ListView>("AddCommandList");
            addCommandList.contentContainer.Clear();

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

                var button = new Button() {text = definedCommand.Key.DisplayName};
                button.clickable.clicked += () =>
                {
                    var command = Activator.CreateInstance(definedCommand.Value) as EventCommand;
                    var newCommand = rootCommandListView.AddCommandAtSelected(command);
                    //選択
                    newCommand.Select();
                };
                categories[category].Add(button);
            }

            //キーボードショートカット
            rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Delete)
                {
                    rootCommandListView.RemoveCommandAtSelected();
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
            var editor = item.Editor;
            var commandEditor = rootVisualElement.Q<VisualElement>("CommandEditor");
            if (commandEditor == null) return;
            commandEditor.Clear();
            var customCommandEditor = editor.CreateCommandEditorGUI();
            commandEditor.Add(customCommandEditor);
        }
    }
}