using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Mukuro
{
    public class EventScriptPlayer : MonoBehaviour
    {
        [SerializeField] protected bool showDebugGui = false;

        [SerializeField] protected List<EventScriptPlayerModule> modules;

        protected Dictionary<Type, EventScriptPlayerModule> Modules { get;} = new Dictionary<Type, EventScriptPlayerModule>();

        private bool IsInitialized { get; set; } = false;
        public bool IsPlaying => CurrentContext != null;
        private EventExecutionContext CurrentContext { get; set; }

        public IScriptVariablesDatabase ScriptVariablesDatabase { get; private set; }

        protected virtual void Start()
        {
            EnsureInitialized();
        }
        
        private void EnsureInitialized()
        {
            Init();
        }

        private void Init()
        {
            if (IsInitialized) return;
            IsInitialized = true;
            ScriptVariablesDatabase = GetScriptVariablesDatabase();
            foreach (var m in modules)
            {
                RegisterModule(m);
            }

            OnInitialized();
        }

        public void RegisterModule(EventScriptPlayerModule module)
        {
            if (module == null) return;
            Type t = module.GetType();
            if (Modules.ContainsKey(t))
            {
                Debug.LogError("同じ型のModuleは複数登録できません。");
                return;
            }

            Modules.Add(t, module);
        }

        /// <summary>
        /// イベントを再生します。シーン参照は現在のアクティブなシーンから検索されます。
        /// </summary>
        /// <param name="scriptAsset"></param>
        /// <param name="eventVariables"></param>
        public void Play(EventScriptAsset scriptAsset, IVariableStore eventVariables = null)
        {
            Play(scriptAsset, SceneManager.GetActiveScene(), eventVariables);
        }


        /// <summary>
        /// イベントを再生します。
        /// </summary>
        /// <param name="scriptAsset"></param>
        /// <param name="scene"></param>
        /// <param name="eventVariables"></param>
        /// <exception cref="NullReferenceException"></exception>
        public void Play(EventScriptAsset scriptAsset, Scene scene, IVariableStore eventVariables = null)
        {
            if (scene == default) throw new NullReferenceException();
            var runtimeReference = EventRuntimeReferenceHostRegistry.Get(scene, scriptAsset);

            Play(scriptAsset, runtimeReference, eventVariables);
        }

        /// <summary>
        /// イベントを再生します。
        /// </summary>
        /// <param name="scriptAsset"></param>
        /// <param name="runtimeReferences"></param>
        /// <param name="eventVariables"></param>
        /// <exception cref="NullReferenceException"></exception>
        public void Play(EventScriptAsset scriptAsset, EventRuntimeReferenceHost runtimeReferences,
            IVariableStore eventVariables = null)
        {
            if (scriptAsset == null) throw new NullReferenceException();
            if (eventVariables == null)
            {
                EnsureInitialized();
                eventVariables = ScriptVariablesDatabase.GetStore(scriptAsset);
            }

            Play(scriptAsset.Script, runtimeReferences, eventVariables);
        }

        /// <summary>
        /// イベントを再生します。
        /// </summary>
        /// <param name="script"></param>
        /// <param name="runtimeReferences"></param>
        /// <param name="eventVariables"></param>
        /// <exception cref="NullReferenceException"></exception>
        public void Play(EventScript script, EventRuntimeReferenceHost runtimeReferences,
            IVariableStore eventVariables)
        {
            EnsureInitialized();
            if (script == null) throw new NullReferenceException();
            //if (runtimeReferences == null) throw new NullReferenceException();
            if (eventVariables == null) throw new NullReferenceException();

            if (IsPlaying)
            {
                throw new InvalidOperationException("EventScriptPlayerは別のイベントを再生中です。");
            }

            var context = new EventExecutionContext(this);

            CurrentContext = context;
            Debug.Log("EventScriptPlayer: Start.");
            context.Play(script, () =>
                {
                    Debug.Log("EventScriptPlayer: End.");
                    OnCompleteEvent(context);
                    CurrentContext = null;
                },
                (exception) =>
                {
                    Debug.Log("EventScriptPlayer: Error.");
                    CurrentContext = null;
                    OnErrorEvent(context);
                    //TODO: Modulesに対するエラー通知

                }, runtimeReferences, eventVariables, () => OnPlayEvent(context));
        }

        private void OnGUI()
        {
            if (!showDebugGui) return;
            var idEntries = ScriptVariablesDatabase.IdEntries;

            GUILayout.Label("イベント変数ストア");
            foreach (string id in idEntries)
            {
                GUILayout.Label("Database entry: " + id);
                var store = ScriptVariablesDatabase.GetStore(id);
                var keyEntries = store.KeyEntries;
                foreach (string key in keyEntries)
                {
                    object value = store.GetValue(key);
                    GUILayout.Label($"\t{key} : {value.ToString()}");
                }
            }

            //GUILayout.Label("CURRENT SCENE REFERENCES");
        }

        protected virtual IScriptVariablesDatabase GetScriptVariablesDatabase()
        {
            return new RegularScriptVariablesDatabase();
        }

        public T GetModule<T>() where T : EventScriptPlayerModule
        {
            if (Modules.TryGetValue(typeof(T), out var result))
            {
                return result as T;
            }

            return default;
        }

        /// <summary>
        /// 初期化処理後に呼ばれる。
        /// </summary>
        protected virtual void OnInitialized()
        {
        }

        /// <summary>
        /// イベントの再生前に呼ばれる。
        /// </summary>
        protected virtual void OnPlayEvent(EventExecutionContext context)
        {
        }
        
        /// <summary>
        /// イベントの再生が正常に終わったときに呼ばれる。
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnCompleteEvent(EventExecutionContext context)
        {}
        
        
        /// <summary>
        /// イベントの再生がエラーで終了したときに呼ばれる。
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnErrorEvent(EventExecutionContext context)
        {}
    }
}