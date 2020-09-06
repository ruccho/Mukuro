using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Samples
{
    [EventCommand("サンプルカテゴリ", "サンプルコマンド3: コマンドのネスト")]
    public class SampleEventCommand3 : EventCommand
    {
        [SerializeField] private EventCommandList commandList = default;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            Debug.Log("SampleEventCommand3: ネストされたイベントを実行します。");
            var script = new EventScript();
            script.WaitForAllCommands = false;
            script.CommandList = commandList;
            
            context.InsertInherit(script, () =>
            {
                Debug.Log("SampleEventCommand3: ネストの終了。");
                handle.Complete();
            });
            
        }
    }
}