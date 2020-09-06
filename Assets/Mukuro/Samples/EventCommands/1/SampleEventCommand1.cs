using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Samples
{
    [EventCommand("サンプルカテゴリ", "サンプルコマンド1")]
    public class SampleEventCommand1 : EventCommand
    {
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            Debug.Log("SampleEventCommand1 executed!");
            handle.Complete();
        }
    }
}