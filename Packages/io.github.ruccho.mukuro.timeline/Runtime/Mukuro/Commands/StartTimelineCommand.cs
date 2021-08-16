using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Mukuro.Timeline
{
    [EventCommand("Timeline", "タイムラインを開始")]
    public class StartTimelineCommand : EventCommand
    {
        [SerializeField] private StopActionType stopAction = StopActionType.WaitForStop;
        [SerializeField] private ExposedReference<PlayableDirector> directorReference = default;
        [SerializeField] private PlayableAsset timelineAsset = default;
        [SerializeField] private EventCommandList nestedCommands = default;

        enum StopActionType
        {
            None,
            WaitForStop,
            ForceStop
        }
        
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            ExecuteAsync(context, handle).Forget();
        }

        private async UniTaskVoid ExecuteAsync(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var timelineModule = context.GetModule<TimelineModule>();
            if (!timelineModule) throw new NullReferenceException();

            var director = directorReference.Resolve(context.ReferenceResolver);
            if (!director) throw new NullReferenceException();

            var timelinePlaying = PlayTimeline(director, timelineModule);
            await PlayNestedCommands(context);
            await timelinePlaying;

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

        private async UniTask PlayTimeline(PlayableDirector director, TimelineModule timelineModule)
        {
            director.Play(timelineAsset);
            timelineModule.RegisterSignalReceiver(director.playableGraph);
            
            switch(stopAction)
            {
                case StopActionType.None:
                    break;
                case StopActionType.WaitForStop:
                    var cs = new UniTaskCompletionSource();
                    Action<PlayableDirector> onStopped = _ => { cs.TrySetResult(); };
                    director.stopped += onStopped;

                    await cs.Task;

                    director.stopped -= onStopped;
                    break;
                case StopActionType.ForceStop:
                    director.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                
            }
        }
    }
}