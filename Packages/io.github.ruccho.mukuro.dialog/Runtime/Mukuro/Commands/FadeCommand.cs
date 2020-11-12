using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    [EventCommand("Mukuro Dialog", "フェード")]
    public class FadeCommand : EventCommand
    {
        [SerializeField] private float inDuration = 0.2f;
        [SerializeField] private float outDuration = 0.2f;

        [SerializeField, ShowListViewInCustomDetail]
        private EventCommandList nestedCommands = default;

        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var dialog = context.GetModule<DialogModule>();
            if (dialog == default)
            {
                context.RaiseError(new ScriptExecutionException("DialogModuleが登録されていません。"));
                return;
            }
            
            dialog.Fade.In(inDuration, () =>
            {
                var script = new EventScript()
                {
                    CommandList = nestedCommands,
                    WaitForAllCommands = true
                };
                context.InsertInherit(script, () =>
                {
                    dialog.Fade.Out(outDuration, handle.Complete);
                });
            });
        }
    }
}