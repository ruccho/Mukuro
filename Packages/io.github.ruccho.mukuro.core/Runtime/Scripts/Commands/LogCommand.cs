using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("デバッグ", "ログ出力")]
    public class LogCommand : EventCommand
    {
        [SerializeField] private string message = default;

        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            Debug.Log(message);
            handle.Complete();
        }
    }
}