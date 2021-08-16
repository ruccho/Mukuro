using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("デバッグ", "スキップブロック")]
    public class SkipContainerCommand : EventCommand
    {
        [SerializeField] private EventCommandList container = default;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            handle.Complete();
        }
    }
}