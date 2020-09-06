using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("制御", "イベントの埋め込み")]
    public class NestEventScriptCommand : EventCommand
    {

        [SerializeField] private EventScriptAsset scriptAsset = default;
        
        /// <summary>
        /// イベント変数や一時変数を引き継ぎます。
        /// </summary>
        [SerializeField] private bool inheritVariables = false;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            
            if (inheritVariables)
            {
                var script = scriptAsset.Script;
                script.WaitForAllCommands = true;
                context.InsertInherit(script, handle.Complete);
            }
            else
            {
                context.Insert(scriptAsset,handle.Complete, null);
            }
            
            
        }
    }
}