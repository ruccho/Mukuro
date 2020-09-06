using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("デバッグ", "変数ログ出力")]
    public class LogVariableCommand : EventCommand
    {
        [SerializeReference] private VariableReference target = default;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            if (target.Evaluate(context, out object result))
            {
                string message = "";
                switch (target.StoreType)
                {
                    case VariableReferenceType.Constant:
                        message = "定数";
                        break;
                    case VariableReferenceType.Temporary:
                        message = "一時変数";
                        break;
                    case VariableReferenceType.Event:
                        message = "イベント変数";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                message += ": " + result;
                Debug.Log(message);
            }
            
            handle.Complete();
        }
    }
}