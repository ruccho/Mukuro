using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    [EventCommand("Mukuro Dialog", "カットの消去")]
    public class HideCutCommand : EventCommand
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private Fade.UpdateModeType updateMode = Fade.UpdateModeType.Scaled;
        
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var dialog = context.GetModule<DialogModule>();
            if (dialog == default)
            {
                context.RaiseError(new ScriptExecutionException("DialogModuleが登録されていません。"));
                return;
            }
            
            dialog.Cut.Hide(duration, updateMode, handle.Complete);
        }
    }
}