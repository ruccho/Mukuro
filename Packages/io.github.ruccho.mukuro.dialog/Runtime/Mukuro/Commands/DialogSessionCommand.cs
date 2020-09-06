using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    [EventCommand("Mukuro Dialog", "ダイアログセッション")]
    public class DialogSessionCommand : EventCommand
    {
        [SerializeField] private string providerName = default;
        [SerializeField] private EventCommandList commandList = default;
        
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var dialog = context.GetModule<DialogModule>();
            if (dialog == default)
            {
                context.RaiseError(new ScriptExecutionException("DialogModuleが登録されていません。"));
                return;
            }
            
            dialog.Open(providerName, () =>
            {
                var script = new EventScript();
                script.CommandList = commandList;
                script.WaitForAllCommands = true;
            
                context.InsertInherit(script, () =>
                {
                    dialog.Close(handle.Complete);
                });
            });
            
        }
    }
}