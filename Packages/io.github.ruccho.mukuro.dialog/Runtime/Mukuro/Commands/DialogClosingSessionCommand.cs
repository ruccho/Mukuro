using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    [EventCommand("Mukuro Dialog", "ダイアログセッションの一時停止")]
    public class DialogClosingSessionCommand : EventCommand
    {
        [SerializeField] private EventCommandList commandList = default;

        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            var dialog = context.GetModule<DialogModule>();
            if (dialog == default)
            {
                context.RaiseError(new ScriptExecutionException("DialogModuleが登録されていません。"));
                return;
            }

            var providerName = dialog.CurrentProvider.ProviderName;

            dialog.Close(() =>
            {
                var script = new EventScript();
                script.CommandList = commandList;
                script.WaitForAllCommands = true;
                context.InsertInherit(script, () => { dialog.Open(providerName, handle.Complete); });
            });
        }
    }
}