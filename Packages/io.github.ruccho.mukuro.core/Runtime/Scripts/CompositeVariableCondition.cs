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

        /*
        [SerializeField] private List<VariableCondition> conditions = new List<VariableCondition>()
        {
            new VariableCondition()
        };*/

        [SerializeField, Range(0, 4)] private int conditionCount = 0;
        [SerializeField] private VariableCondition condition0 = default;
        [SerializeField] private VariableCondition condition1 = default;
        [SerializeField] private VariableCondition condition2 = default;
        [SerializeField] private VariableCondition condition3 = default;

        private VariableCondition GetCondition(int index)
        {
            switch (index)
            {
                case 0:
                    return condition0;
                case 1:
                    return condition1;
                case 2:
                    return condition2;
                case 3:
                    return condition3;
            }
            throw new IndexOutOfRangeException();
        }

        public bool Evaluate(IVariableStoreContainer context)
        {
            bool result = conditionOperator == CompositeVariableConditionOperator.And;

            for (int i = 0; i < conditionCount; i++)
            {
                var con = GetCondition(i);
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

        public SubscriptionHandle SubscribeOnUpdatedEventVariables(IVariableStore eventVariablesStore, Action onUpdated)
        {
            List<SubscriptionHandle> handles = new List<SubscriptionHandle>();
            
            for (int i = 0; i < conditionCount; i++)
            {
                var c = GetCondition(i);
                var handle = c.SubscribeOnUpdatedEventVariables(eventVariablesStore, onUpdated);
                handles.Add(handle);
            }
            
            return new SubscriptionHandle(() =>
            {
                foreach (var h in handles)
                {
                    h.Dispose();
                }
            });
            
        }
        
    }

    public enum CompositeVariableConditionOperator
    {
        And,
        Or
    }
}