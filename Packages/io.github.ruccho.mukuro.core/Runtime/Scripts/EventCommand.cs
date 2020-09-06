using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [Serializable]
    public abstract class EventCommand
    {
        [SerializeField] private CommandSynchronizationType synchronization = CommandSynchronizationType.Sync;
        public CommandSynchronizationType Synchronization => synchronization;
        
        [SerializeField]
        private bool enabled = true;
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        
        public EventCommand()
        {
        }
        
        public abstract void Execute(EventExecutionContext context, CommandExecutionHandle handle);
    }

    public enum CommandSynchronizationType
    {
        Sync, Async
    }
}