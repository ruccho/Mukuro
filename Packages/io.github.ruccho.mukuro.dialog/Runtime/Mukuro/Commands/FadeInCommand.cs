using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    [EventCommand("Mukuro Dialog", "フェードイン")]
    public class FadeInCommand : EventCommand
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
            
            dialog.Fade.In(duration, handle.Complete);
        }
    }
}