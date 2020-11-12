using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Mukuro
{

    public abstract class EventScriptPlayerBase : MonoBehaviour
    {
        
        [SerializeField] protected bool showDebugGui = false;

        [SerializeField] protected List<EventScriptPlayerModule> modules;

        protected Dictionary<Type, EventScriptPlayerModule> Modules { get; } =
            new Dictionary<Type, EventScriptPlayerModule>();

        private bool IsInitialized { get; set; } = false;
        public bool IsPlaying => CurrentContext != null;
        public EventExecutionContext CurrentContext { get; protected set; }

        private RegularScriptVariablesDatabase scriptVariablesDatabase =
            new RegularScriptVariablesDatabase();

        public virtual IScriptVariablesDatabase ScriptVariablesDatabase => scriptVariablesDatabase;

        protected virtual void Start()
        {
            EnsureInitialized();
        }

        protected void EnsureInitialized()
        {
            Init();
        }

        private void Init()
        {
            if (IsInitialized) return;
            IsInitialized = true;
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


    }

    public abstract class EventScriptPlayer<T> : EventScriptPlayerBase where T : EventPlayingOption
    {
        public void Play(T option)
        {
            if (!option.Ready(ScriptVariablesDatabase)) throw new ArgumentException();
            EnsureInitialized();

            if (IsPlaying)
            {
                throw new InvalidOperationException("EventScriptPlayerは別のイベントを再生中です。");
            }

            var context = new EventExecutionContext(this);

            CurrentContext = context;
            Debug.Log("EventScriptPlayer: Start.");
            context.Play(option.Script, () =>
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
                }, option.RuntimeReferenceHost, option.EventVariables, option.TemporaryVariables, () => OnPlayEvent(context, option));
            
            
        }
        
        /// <summary>
        /// イベントの再生前に呼ばれる。
        /// </summary>
        protected virtual void OnPlayEvent(EventExecutionContext context, T option)
        {
        }

        /// <summary>
        /// イベントの再生が正常に終わったときに呼ばれる。
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnCompleteEvent(EventExecutionContext context)
        {
        }


        /// <summary>
        /// イベントの再生がエラーで終了したときに呼ばれる。
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnErrorEvent(EventExecutionContext context)
        {
        }
    }
    
    public class EventScriptPlayer : EventScriptPlayer<EventPlayingOption>
    {
        
    }

    public class EventPlayingOption
    {
        public EventScriptAsset ScriptAsset { get; }

        private EventScript script;
        public EventScript Script
        {
            get
            {
                if (ScriptAsset) return ScriptAsset.Script;
                return script;
            }
        }
        public IVariableStore TemporaryVariables{ get; set; }
        public IVariableStore EventVariables{ get; set; }
        public Scene SceneForRuntimeReference{ get; set; }
        public EventRuntimeReferenceHost RuntimeReferenceHost{ get; set; }

        public EventPlayingOption(EventScriptAsset script)
        {
            ScriptAsset = script;
        }
        
        public EventPlayingOption(EventScript script)
        {
            this.script = script;
        }

        public bool Ready(IScriptVariablesDatabase eventVariableDatabase)
        {
            if (ScriptAsset == null) return false;

            if (RuntimeReferenceHost == null)
            {
                if (SceneForRuntimeReference == default) SceneForRuntimeReference = SceneManager.GetActiveScene();
                if (ScriptAsset == null) return false;
                RuntimeReferenceHost = EventRuntimeReferenceHostRegistry.Get(SceneForRuntimeReference, ScriptAsset);
            }

            if (EventVariables == null)
            {
                if (ScriptAsset == null) return false;
                EventVariables = eventVariableDatabase.GetStore(ScriptAsset.Id);
            }
            if (TemporaryVariables == null) TemporaryVariables = new RegularVariableStore();

            return true;
        }
    }
}