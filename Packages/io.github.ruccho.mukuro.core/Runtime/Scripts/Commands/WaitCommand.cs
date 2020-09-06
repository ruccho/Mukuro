using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("タイマー", "ウェイト")]
    public class WaitCommand : EventCommand
    {
        [SerializeField] private Timer.UpdateModeType updateMode = Timer.UpdateModeType.Scaled;
        [SerializeField] private FloatVariableReference duration;

        private CommandExecutionHandle currentHandle;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            if (duration.Evaluate(context, out float durationValue))
            {
                currentHandle = handle;
                Timer.AddTimer(durationValue, Tick, updateMode);
            }
            else
            {
                handle.Complete();
            }
        }

        private void Tick()
        {
            Debug.Log("Elapsed!");
            currentHandle.Complete();
        }
    }
}