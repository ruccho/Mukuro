using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("変数操作",  "変数代入")]
    public class SetVariableCommand : EventCommand
    {
        [SerializeReference] private VariableReference value = null;
        [SerializeReference] private VariableReference target = null;
        
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            if (target == null || value == null)
            {
                Debug.LogWarning("VariableReferenceが未設定です。");
                handle.Complete();
                return;
            }
            
            if (target.ValueType != value.ValueType)
            {
                Debug.LogWarning("VariableReferenceの型が一致しません。");
                handle.Complete();
                return;
            }

            if (value.Evaluate(context, out var evaluated))
            {
                target.SetValue(context, evaluated);
            }
            handle.Complete();
        }
    }
}