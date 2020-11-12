using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Mukuro
{
    public class EventExecutionContext : IVariableStoreContainer
    {
        private EventScriptPlayerBase Player { get; set; }
        public EventRuntimeReferenceHost ReferenceResolver => CurrentSession?.ReferenceResolver;
        
        public IVariableStore TemporaryVariables => CurrentSession?.TemporaryVariables;
        public IVariableStore EventVariables => CurrentSession?.EventVariables;
        
        private List<ScriptExecutionSession> CallStack { get; set; } = new List<ScriptExecutionSession>();
        
        private Action OnFinished { get; set; }
        private Action<Exception> OnError { get; set; }
        public bool IsFinished { get; private set; } = false;

        public ScriptExecutionSession CurrentSession => CallStack.LastOrDefault();

        private bool IsRootSessionFinished { get; set; } = false;

        private bool IsErrorRaised { get; set; } = false;
        
        private int runningCommands = 0;
        public int RunningCommands
        {
            get => runningCommands;
            set
            {
                runningCommands = value;
                if (!IsFinished && IsRootSessionFinished && runningCommands == 0)
                {
                    IsFinished = true;
                    OnFinished?.Invoke();
                }
            }
        }

        public EventExecutionContext(EventScriptPlayerBase player)
        {
            Player = player;
        }
        
        public void Play(EventScript script, Action onFinished, Action<Exception> onError, EventRuntimeReferenceHost runtimeReferences,
            IVariableStore eventVariables, IVariableStore temporaryVariables, Action onPlayEvent)
        {
            if (script == null) throw new NullReferenceException();
            if (eventVariables == null) throw new NullReferenceException();

            this.OnFinished = onFinished;
            this.OnError = onError;
            
            var rootSession = new ScriptExecutionSession(script, this, OnRootSessionFinished, runtimeReferences, temporaryVariables, eventVariables);
            CallStack.Add(rootSession);
            onPlayEvent?.Invoke();
            rootSession.StartScript();
        }

        private void OnRootSessionFinished()
        {
            IsRootSessionFinished = true;
            if (!IsFinished && runningCommands == 0)
            {
                IsFinished = true;
                OnFinished?.Invoke();
            }
        }
        
        
        public void Insert(EventScriptAsset scriptAsset, Action onFinished, IVariableStore eventVariables = null)
        {
            Insert(scriptAsset,onFinished, SceneManager.GetActiveScene(), eventVariables);
        }

        public void Insert(EventScriptAsset scriptAsset, Action onFinished, Scene scene, IVariableStore eventVariables = null)
        {
            if (scene == default) throw new NullReferenceException();
            var runtimeReference = EventRuntimeReferenceHostRegistry.Get(scene, scriptAsset);

            Insert(scriptAsset,onFinished, runtimeReference, eventVariables);
        }

        public void Insert(EventScriptAsset scriptAsset, Action onFinished, EventRuntimeReferenceHost runtimeReferences,
            IVariableStore eventVariables = null)
        {
            if (scriptAsset == null) throw new NullReferenceException();
            if (eventVariables == null)
            {
                eventVariables = Player.ScriptVariablesDatabase.GetStore(scriptAsset);
            }

            Insert(scriptAsset.Script,onFinished, runtimeReferences, eventVariables);
        }
        
        public void Insert(EventScript script, Action onFinished, EventRuntimeReferenceHost runtimeReferences,
            IVariableStore eventVariables)
        {
            Insert(script, onFinished, runtimeReferences, eventVariables, new RegularVariableStore());
        }

        public void Insert(EventScript script, Action onFinished, EventRuntimeReferenceHost runtimeReferences,
            IVariableStore eventVariables, IVariableStore temporaryVariables)
        {
            if (script == null) throw new NullReferenceException();
            //if (runtimeReferences == null) throw new NullReferenceException();
            if (eventVariables == null) throw new NullReferenceException();

            CurrentSession?.Freeze();
            
            var session = new ScriptExecutionSession(script, this, () =>
            {
                CallStack.RemoveAt(CallStack.Count - 1);
                CurrentSession.Unfreeze();
                onFinished?.Invoke();
            }, runtimeReferences, temporaryVariables, eventVariables);
            CallStack.Add(session);
            session.StartScript();
        }

        public void InsertInherit(EventScript script, Action onFinished)
        {
            Insert(script, onFinished, ReferenceResolver, EventVariables, TemporaryVariables);
        }

        public T GetModule<T>() where T : EventScriptPlayerModule
        {
            return Player.GetModule<T>();
        }

        public void RaiseError(Exception exception)
        {
            //エラーの通知は1回だけ！（AdvanceLineのネストにより再帰的にRaiseするのを防ぐため）
            if (!IsErrorRaised)
            {
                IsErrorRaised = true;
                //全Sessionを凍結
                foreach (var s in CallStack)
                {
                    s.Freeze();
                }

                //エラーを通知
                OnError?.Invoke(exception);
            }

            ExceptionDispatchInfo.Capture(exception).Throw();;
        }
        
    }

    public class ScriptExecutionException : Exception
    {
        public ScriptExecutionException(string message) : base(message)
        {
        }
    }

}