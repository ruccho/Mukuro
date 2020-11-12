using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [EventCommand("変数操作", "演算")]
    public class CalculationCommand : EventCommand
    {
        [SerializeField] private VariableCalculation calculation = default;
        public override void Execute(EventExecutionContext context, CommandExecutionHandle handle)
        {
            calculation.Execute(context);
            handle.Complete();
        }
    }
}