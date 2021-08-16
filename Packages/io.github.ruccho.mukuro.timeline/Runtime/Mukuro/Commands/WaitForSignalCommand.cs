using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Mukuro.Timeline
{
    [EventCommand("Timeline", "シグナルの待ち受け")]
    public class WaitForSignalCommand : EventCommand
    {
        [SerializeField] private SignalAsset target = default;
        [SerializeField] private EventCommandList nestedCommands = default;

        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            ExecuteAsync(context, handle).Forget();
        }

        private async UniTaskVoid ExecuteAsync(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var timelineModule = context.GetModule<TimelineModule>();
            if (!timelineModule) throw new NullReferenceException();

            var cs = new UniTaskCompletionSource();
            
            Action<SignalAsset> onSignalNotify = signalAsset =>
            {
                if (signalAsset == target)
                {
                    cs.TrySetResult();
                }
            };

            timelineModule.OnSignalNotify += onSignalNotify;
            
            await PlayNestedCommands(context);
            await cs.Task;

            timelineModule.OnSignalNotify -= onSignalNotify;
            
            handle.Complete();
        }


        private UniTask PlayNestedCommands(EventExecutionContext context)
        {
            var cs = new UniTaskCompletionSource();

            var script = new EventScript();
            script.CommandList = nestedCommands;
            script.WaitForAllCommands = false;
            context.InsertInherit(script, () => { cs.TrySetResult(); });

            return cs.Task;
        }
    }
}