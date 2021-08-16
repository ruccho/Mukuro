using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mukuro
{
    [Serializable]
    public abstract class VariableReference
    {
        [SerializeField] protected VariableReferenceType storeType;
        [SerializeField] protected string key;

        public VariableReferenceType StoreType => storeType;
        public string Key => key;

        public abstract Type ValueType { get; }

        public abstract bool Evaluate(IVariableStoreContainer context, out object value);
        public abstract void SetValue(IVariableStoreContainer context, object value);

        public class TypeSelectableAttribute : PropertyAttribute
        {
        }
    };

    public interface IVariableReferenceNumericalOut<out T>
    {
        T EvaluateNumerical(IVariableStoreContainer context);
    }

    public interface IVariableReferenceNumericalIn<in T>
    {
        void SetValue(IVariableStoreContainer context, T value);
    }

    public class VariableReference<T> : VariableReference
    {
        [SerializeField] private T constantValue = default;

        public override Type ValueType => typeof(T);

        public override bool Evaluate(IVariableStoreContainer context, out object value)
        {
            if (Evaluate(context, out T innerValue))
            {
                value = innerValue;
                return true;
            }

            value = default;
            return false;
        }

        public override void SetValue(IVariableStoreContainer context, object value)
        {
            SetValue(context, (T) value);
        }


        public virtual bool Evaluate(IVariableStoreContainer context, out T value)
        {
            IVariableStore store;
            switch (storeType)
            {
                case VariableReferenceType.Constant:
                    value = constantValue;
                    return true;
                case VariableReferenceType.Temporary:
                    store = context.TemporaryVariables;
                    break;
                case VariableReferenceType.Event:
                    store = context.EventVariables;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (store == null)
            {
                throw new NullReferenceException($"IVariableStoreContainerに変数ストア{storeType}が登録されていません。");
            }

            if (store.TryGetValueType(key, out Type t))
            {
                if (t != typeof(T))
                {
                    Debug.LogWarning($"変数の評価に失敗しました。型{typeof(T).Name}が指定されましたが、指定されたキー\"{key}\"の値の型は{t.Name}です。");
                    value = default;
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"変数の評価に失敗しました。指定されたキー\"{key}\"の値は見つかりませんでした。");
                value = default;
                return false;
            }

            value = store.GetValue<T>(key);
            return true;
        }

        public void SetValue(IVariableStoreContainer context, T value)
        {
            IVariableStore store;
            switch (storeType)
            {
                case VariableReferenceType.Constant:
                    Debug.LogWarning("Constantに設定されたVariableReferenceには代入できません。");
                    return;
                case VariableReferenceType.Temporary:
                    store = context.TemporaryVariables;
                    break;
                case VariableReferenceType.Event:
                    store = context.EventVariables;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            store.SetValue(key, value);
        }
    }

    public enum VariableReferenceType
    {
        Constant,
        Temporary,
        Event
    }

    public class VariableEntryNotFoundException : Exception
    {
    }

    [Serializable]
    public class IntVariableReference : VariableReference<int>, IVariableReferenceNumericalOut<int>,
        IVariableReferenceNumericalOut<long>, IVariableReferenceNumericalOut<float>,
        IVariableReferenceNumericalOut<double>, IVariableReferenceNumericalOut<decimal>,
        IVariableReferenceNumericalIn<int>, IVariableReferenceNumericalIn<sbyte>, IVariableReferenceNumericalIn<byte>,
        IVariableReferenceNumericalIn<short>, IVariableReferenceNumericalIn<ushort>, IVariableReferenceNumericalIn<char>
    {
        int IVariableReferenceNumericalOut<int>.EvaluateNumerical(IVariableStoreContainer context)
        {
            if (Evaluate(context, out int value))
            {
                return value;
            }
            throw new VariableEntryNotFoundException();
        }

        long IVariableReferenceNumericalOut<long>.EvaluateNumerical(IVariableStoreContainer context)
        {
            if (Evaluate(context, out int value))
            {
                return value;
            }
            throw new VariableEntryNotFoundException();
        }

        float IVariableReferenceNumericalOut<float>.EvaluateNumerical(IVariableStoreContainer context)
        {
            if (Evaluate(context, out int value))
            {
                return value;
            }
            throw new VariableEntryNotFoundException();
        }

        double IVariableReferenceNumericalOut<double>.EvaluateNumerical(IVariableStoreContainer context)
        {
            if (Evaluate(context, out int value))
            {
                return value;
            }
            throw new VariableEntryNotFoundException();
        }

        decimal IVariableReferenceNumericalOut<decimal>.EvaluateNumerical(IVariableStoreContainer context)
        {
            if (Evaluate(context, out int value))
            {
                return value;
            }
            throw new VariableEntryNotFoundException();
        }

        public void SetValue(IVariableStoreContainer context, sbyte value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, byte value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, short value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, ushort value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, char value)
        {
            base.SetValue(context, value);
        }
    }

    [Serializable]
    public class BoolVariableReference : VariableReference<bool>
    {
    }

    [Serializable]
    public class FloatVariableReference : VariableReference<float>, IVariableReferenceNumericalOut<float>,
        IVariableReferenceNumericalOut<double>, IVariableReferenceNumericalIn<float>,
        IVariableReferenceNumericalIn<sbyte>, IVariableReferenceNumericalIn<byte>, IVariableReferenceNumericalIn<short>,
        IVariableReferenceNumericalIn<ushort>, IVariableReferenceNumericalIn<int>, IVariableReferenceNumericalIn<uint>,
        IVariableReferenceNumericalIn<long>, IVariableReferenceNumericalIn<ulong>, IVariableReferenceNumericalIn<char>
    {
        float IVariableReferenceNumericalOut<float>.EvaluateNumerical(IVariableStoreContainer context)
        {
            if (Evaluate(context, out float value))
            {
                return value;
            }
            throw new VariableEntryNotFoundException();
        }

        double IVariableReferenceNumericalOut<double>.EvaluateNumerical(IVariableStoreContainer context)
        {
            if (Evaluate(context, out float value))
            {
                return value;
            }
            throw new VariableEntryNotFoundException();
        }

        public void SetValue(IVariableStoreContainer context, sbyte value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, byte value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, short value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, ushort value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, int value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, uint value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, long value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, ulong value)
        {
            base.SetValue(context, value);
        }

        public void SetValue(IVariableStoreContainer context, char value)
        {
            base.SetValue(context, value);
        }
    }

    [Serializable]
    public class StringVariableReference : VariableReference<string>
    {
    }

    [Serializable]
    public class ColorVariableReference : VariableReference<Color>
    {
    }

    [Serializable]
    public class ObjectVariableReference : VariableReference<UnityEngine.Object>
    {
    }

    [Serializable]
    public class LayerMaskVariableReference : VariableReference<LayerMask>
    {
    }

    [Serializable]
    public class Vector2VariableReference : VariableReference<Vector2>
    {
    }

    [Serializable]
    public class Vector3VariableReference : VariableReference<Vector3>
    {
    }

    [Serializable]
    public class Vector4VariableReference : VariableReference<Vector4>
    {
    }

    [Serializable]
    public class RectVariableReference : VariableReference<Rect>
    {
    }

    [Serializable]
    public class AnimationCurveVariableReference : VariableReference<AnimationCurve>
    {
    }

    [Serializable]
    public class BoundsVariableReference : VariableReference<Bounds>
    {
    }

    [Serializable]
    public class GradientVariableReference : VariableReference<Gradient>
    {
    }

    [Serializable]
    public class QuaternionVariableReference : VariableReference<Quaternion>
    {
    }

    [Serializable]
    public class Vector2IntVariableReference : VariableReference<Vector2Int>
    {
    }

    [Serializable]
    public class Vector3IntVariableReference : VariableReference<Vector3Int>
    {
    }

    [Serializable]
    public class RectIntVariableReference : VariableReference<RectInt>
    {
    }

    [Serializable]
    public class BoundsIntVariableReference : VariableReference<BoundsInt>
    {
    }
}