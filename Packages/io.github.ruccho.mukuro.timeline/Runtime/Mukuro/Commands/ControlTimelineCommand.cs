using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Mukuro.Timeline
{
    [EventCommand("Timeline", "タイムライン制御")]
    public class ControlTimelineCommand : EventCommand
    {
        [SerializeField] private ExposedReference<PlayableDirector> directorReference = default;
        [SerializeField] private ControlType control = ControlType.Pause;

        public enum ControlType
        {
            Resume,
            Pause,
            Stop
        }

        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var director = directorReference.Resolve(context.ReferenceResolver);
            if (!director) throw new NullReferenceException();

            switch (control)
            {
                case ControlType.Resume:
                    director.Resume();
                    break;
                case ControlType.Pause:
                    director.Pause();
                    break;
                case ControlType.Stop:
                    director.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            handle.Complete();
        }
    }
}