using System.Collections;
using System.Collections.Generic;
using Ruccho.Utilities;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("ユーティリティ", "UnityEvent")]
    public class ExposedUnityEventCommand : EventCommand
    {
        [SerializeField] private ExposedUnityEvent ev = default;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            ev.Invoke(context.ReferenceResolver);
            handle.Complete();
        }
    }
}