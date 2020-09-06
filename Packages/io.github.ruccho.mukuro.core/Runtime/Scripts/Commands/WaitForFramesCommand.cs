using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("タイマー", "ウェイト (フレーム)")]
    public class WaitForFramesCommand : EventCommand
    {
        [SerializeField] private int frames = default;
        
        private CommandExecutionHandle currentHandle;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            currentHandle = handle;
            Timer.AddFrameTimer(frames, Tick);
        }
        
        private void Tick()
        {
            Debug.Log("Elapsed!");
            currentHandle.Complete();
        }
    }
}