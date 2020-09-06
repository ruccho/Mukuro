using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [Serializable]
    public class CompositeVariableCondition
    {
        [SerializeField] private CompositeVariableConditionOperator conditionOperator = default;

        [SerializeField] private List<VariableCondition> conditions = new List<VariableCondition>()
        {
            new VariableCondition()
        };

        public bool Evaluate(IVariableStoreContainer context)
        {
            bool result = conditionOperator == CompositeVariableConditionOperator.And;

            foreach (var con in conditions)
            {
                var res = con.Evaluate(context);
                if (result && !res)
                {
                    result = false;
                    break;
                }
                else if (!result && res)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }

    public enum CompositeVariableConditionOperator
    {
        And,
        Or
    }
}