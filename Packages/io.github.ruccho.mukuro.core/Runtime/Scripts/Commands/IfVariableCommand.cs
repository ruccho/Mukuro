using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("制御", "条件分岐（変数）")]
    public class IfVariableCommand : EventCommand
    {
        [SerializeField] private bool waitForAllCommand = default;
        [SerializeField] private VariableCondition condition = default;
        [SerializeField] private EventCommandList ifTrue = default;

        [SerializeField] private EventCommandList ifFalse = default;

        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            if (condition.Evaluate(context))
            {
                var script = new EventScript();
                script.CommandList = ifTrue;
                script.WaitForAllCommands = waitForAllCommand;
                context.InsertInherit(script, handle.Complete);
            }
            else
            {
                var script = new EventScript();
                script.CommandList = ifFalse;
                script.WaitForAllCommands = waitForAllCommand;
                context.InsertInherit(script, handle.Complete);
            }
        }
    }
}