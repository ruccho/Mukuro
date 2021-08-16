using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mukuro
{
    [Serializable]
    public class VariableCondition
    {
        [SerializeReference, VariableReference.TypeSelectable]
        private VariableReference left = default;

        [SerializeReference, VariableReference.TypeSelectable]
        private VariableReference right = default;

        [SerializeField] private ConditionOperatorType conditionOperator = ConditionOperatorType.Equal;

        public bool Evaluate(IVariableStoreContainer context, bool onFailedValue = false)
        {
            if (left.Evaluate(context, out var leftValue) && right.Evaluate(context, out var rightValue))
            {
                if (leftValue is IComparable leftComparable && rightValue is IComparable rightComparable)
                {
                    int comparison = leftComparable.CompareTo(rightComparable);

                    switch (conditionOperator)
                    {
                        case ConditionOperatorType.Equal:
                            return comparison == 0;
                        case ConditionOperatorType.NotEqual:
                            return comparison != 0;
                        case ConditionOperatorType.LessThan:
                            return comparison < 0;
                        case ConditionOperatorType.MoreThan:
                            return comparison > 0;
                        case ConditionOperatorType.LessThanEqual:
                            return comparison <= 0;
                        case ConditionOperatorType.MoreThanEqual:
                            return comparison >= 0;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    switch (conditionOperator)
                    {
                        case ConditionOperatorType.Equal:
                            return leftValue == rightValue;
                        case ConditionOperatorType.NotEqual:
                            return leftValue != rightValue;
                        default:
                            throw new InvalidOperationException("この変数に対してこの比較演算子は適用できません。");
                    }
                }
            }
            else
            {
                Debug.LogWarning("変数の評価に失敗しました");
                return onFailedValue;
            }
        }

        public SubscriptionHandle SubscribeOnUpdatedEventVariables(IVariableStore store, Action onUpdated)
        {
            if (right == null || left == null) throw new NullReferenceException("式が不正です。");
            if (left.StoreType == VariableReferenceType.Temporary || right.StoreType == VariableReferenceType.Temporary)
            {
                Debug.LogError("一時変数を含むConditionのウォッチは出来ません");
                return SubscriptionHandle.Empty;
            }

            SubscriptionHandle leftHandle = SubscriptionHandle.Empty;
            SubscriptionHandle rightHandle = SubscriptionHandle.Empty;
            ;
            if (left.StoreType == VariableReferenceType.Event)
            {
                leftHandle = store.SubscribeOnUpdated<object>(left.Key, _ => onUpdated());
            }

            if (right.StoreType == VariableReferenceType.Event)
            {
                rightHandle = store.SubscribeOnUpdated<object>(right.Key, _ => onUpdated());
            }

            return new SubscriptionHandle(() =>
            {
                leftHandle.Dispose();
                rightHandle.Dispose();
            });
        }
    }

    public enum ConditionOperatorType
    {
        Equal,
        NotEqual,
        LessThan,
        MoreThan,
        LessThanEqual,
        MoreThanEqual
    }
}