using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    public class ScriptExecutionSession
    {
        public EventRuntimeReferenceHost ReferenceResolver { get;  }
        public IVariableStore TemporaryVariables { get; }
        public IVariableStore EventVariables { get; }
        
        public bool IsFreezed { get; private set; }
        private bool IsAdvanceLineProposed { get; set; } = false;
        private bool IsEndSessionProposed { get; set; } = false;
        
        
        private List<EventCommand> Commands { get;  }
        private int CurrentLine { get; set; }

        private int RunningCommands { get; set; } = 0;
        private bool WaitForAllCommands { get;  } = false;
        
        private EventExecutionContext CurrentContext { get;  }
        
        private Action OnFinished { get;  }

        private bool IsAllCommandDispatched { get; set; } = false;

        public ScriptExecutionSession(EventScript script, EventExecutionContext context, Action onFinished, EventRuntimeReferenceHost resolver, IVariableStore temporaryVariables, IVariableStore eventVariables)
        {
            Commands = new List<EventCommand>(script.CommandList.Commands);
            WaitForAllCommands = script.WaitForAllCommands;
            OnFinished = onFinished;
            CurrentContext = context;
            ReferenceResolver = resolver;
            TemporaryVariables = temporaryVariables;
            EventVariables = eventVariables;
        }

        public void StartScript()
        {
            CurrentLine = -1;
            RunningCommands = 0;
            AdvanceLine();
        }

        private void AdvanceLine()
        {
            if (IsFreezed)
            {
                IsAdvanceLineProposed = true;
                return;
            }
            
            CurrentLine++;
            if (CurrentLine >= Commands.Count)
            {
                IsAllCommandDispatched = true;
                EndIfComplete();
                return;
            }

            var current = Commands[CurrentLine];
            CommandExecutionHandle handle; 
            IncreaseRunningCommandCount();
            switch(current.Synchronization)
            {
                //case CommandSynchronizationType.Sync:
                case CommandSynchronizationType.Async:
                    handle = new CommandExecutionHandle(OnAsyncCommandFinished);
                    try
                    {
                        current.Execute(CurrentContext, handle);
                    }
                    catch (Exception e)
                    {
                        CurrentContext.RaiseError(e);
                    }

                    AdvanceLine();
                    break; 
                default:
                    handle = new CommandExecutionHandle(OnSyncCommandFinished);
                    try
                    {
                        current.Execute(CurrentContext, handle);
                    }
                    catch (Exception e)
                    {
                        CurrentContext.RaiseError(e);
                    }
                    break;
                
            }

        }

        public void Freeze()
        {
            if (IsFreezed) return;
            IsFreezed = true;
            IsAdvanceLineProposed = false;
        }

        public void Unfreeze()
        {
            if (!IsFreezed) return;
            IsFreezed = false;
            if (IsAdvanceLineProposed)
            {
                IsAdvanceLineProposed = false;
                AdvanceLine();
            }else if (IsEndSessionProposed)
            {
                EndIfComplete();
            }
        }

        private void OnSyncCommandFinished()
        {
            DecreaseRunningCommandCount();
            AdvanceLine();
        }
        
        private void OnAsyncCommandFinished()
        {
            DecreaseRunningCommandCount();
            EndIfComplete();
        }

        private void EndIfComplete()
        {
            if (IsAllCommandDispatched && (!WaitForAllCommands || RunningCommands == 0))
            {
                if (IsFreezed)
                {
                    IsEndSessionProposed = true;
                    return;
                }
                OnFinished?.Invoke();
            }
        }

        private void IncreaseRunningCommandCount()
        {
            RunningCommands++;
            CurrentContext.RunningCommands++;
        }
        
        private void DecreaseRunningCommandCount()
        {
            RunningCommands--;
            CurrentContext.RunningCommands--;
            
        }
        
    }

}