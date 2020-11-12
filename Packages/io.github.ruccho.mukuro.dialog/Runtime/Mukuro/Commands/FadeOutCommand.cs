using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    [EventCommand("Mukuro Dialog", "フェードアウト")]
    public class FadeOutCommand : EventCommand
    {
        [SerializeField] private float duration = default;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var dialog = context.GetModule<DialogModule>();
            if (dialog == default)
            {
                context.RaiseError(new ScriptExecutionException("DialogModuleが登録されていません。"));
                return;
            }
            
            dialog.Fade.Out(duration, handle.Complete);
        }
    }
}