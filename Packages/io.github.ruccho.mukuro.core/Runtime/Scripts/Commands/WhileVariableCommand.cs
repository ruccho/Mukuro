using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("制御", "条件繰り返し（変数）")]
    public class WhileVariableCommand : EventCommand
    {
        [SerializeField] private bool waitForAllCommand = default;
        [SerializeField] private VariableCondition condition = default;
        [SerializeField] private EventCommandList routine = default;

        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            UpdateLoop(context, handle);
        }

        private void UpdateLoop(EventExecutionContext context, CommandExecutionHandle handle)
        {
            if (condition.Evaluate(context))
            {
                var script = new EventScript();
                script.CommandList = routine;
                script.WaitForAllCommands = waitForAllCommand;
                context.InsertInherit(script, () => UpdateLoop(context, handle));
            }
            else
            {
                handle.Complete();
            }
        }
    }
}