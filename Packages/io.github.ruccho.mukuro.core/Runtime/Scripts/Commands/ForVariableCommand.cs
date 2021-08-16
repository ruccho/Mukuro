using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("制御", "回数繰り返し（変数）")]
    public class ForVariableCommand : EventCommand
    {
        [SerializeField] private bool waitForAllCommand = default;
        [SerializeReference, VariableReference.TypeSelectableAttribute] private VariableReference count = default;
        [SerializeField] private EventCommandList routine = default;

        private int counter = 0;

        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            counter = -1;
            UpdateLoop(context, handle);
        }

        private void UpdateLoop(EventExecutionContext context, CommandExecutionHandle handle)
        {
            counter++;
            if (count.Evaluate(context, out object result))
            {
                if (result is IComparable resultComparable && counter.CompareTo(resultComparable) < 0)
                {
                    var script = new EventScript();
                    script.CommandList = routine;
                    script.WaitForAllCommands = waitForAllCommand;
                    context.InsertInherit(script, () => UpdateLoop(context, handle));
                    return;
                }
            }

            handle.Complete();
        }
    }
}