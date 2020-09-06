using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mukuro.Dialog
{
    [EventCommand("Mukuro Dialog", "選択肢の表示")]
    public class DialogShowMenuCommand : EventCommand
    {
        [SerializeField] private DialogMenuBifurcation[] bifurcations = default;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            
            var dialog = context.GetModule<DialogModule>();
            if (dialog == default)
            {
                context.RaiseError(new ScriptExecutionException("DialogSessionが登録されていません。"));
                return;
            }

            string[] items = bifurcations.Select(b => b.Label).ToArray();
            var settings = new DialogShowMenuSettings(items);
            dialog.ShowMenu(settings, (index) =>
            {
                var script = new EventScript();
                script.WaitForAllCommands = false;
                script.CommandList = bifurcations[index].CommandList;
                context.InsertInherit(script, handle.Complete);
            });
        }
    }

    [Serializable]
    public class DialogMenuBifurcation
    {
        [SerializeField] private string label = default;
        [SerializeField] private EventCommandList commandList = default;

        public string Label => label;
        public EventCommandList CommandList => commandList;
    }

    public class DialogShowMenuSettings
    {
        public DialogShowMenuSettings(string[] items)
        {
            Items = items;
        }

        public string[] Items { get; }
        
        
        public ISpeakerInfo SpeakerInfo { get; set; }    
        public string Face { get; set; }
        
        
    }
}