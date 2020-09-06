using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mukuro
{
    [Serializable]
    public class VariableCalculation
    {
        [SerializeReference, VariableReference.TypeSelectableAttribute] private VariableReference left;
        [SerializeReference, VariableReference.TypeSelectableAttribute] private VariableReference right0;
        [SerializeReference, VariableReference.TypeSelectableAttribute] private VariableReference right1;
        
        [SerializeField] private CalculationOperatorType calculationOperator = CalculationOperatorType.Add;
        
        
        public void Execute(EventExecutionContext context)
        {
            //右辺の計算に使用する型を選ぶ
            //できるだけ低次（高精度、高速?）の型で処理できた方がよい?
            if (right0 is IVariableReferenceNumericalOut<sbyte> r0sbyte &&
                right1 is IVariableReferenceNumericalOut<sbyte> r1sbyte && left is IVariableReferenceNumericalIn<int> lsbyte)
            {
                var r0v = r0sbyte.EvaluateNumerical(context);
                var r1v = r1sbyte.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        lsbyte.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        lsbyte.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        lsbyte.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        lsbyte.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (right0 is IVariableReferenceNumericalOut<byte> r0byte &&
                     right1 is IVariableReferenceNumericalOut<byte> r1byte && left is IVariableReferenceNumericalIn<int> lbyte)
            {
                var r0v = r0byte.EvaluateNumerical(context);
                var r1v = r1byte.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        lbyte.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        lbyte.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        lbyte.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        lbyte.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (right0 is IVariableReferenceNumericalOut<short> r0short &&
                     right1 is IVariableReferenceNumericalOut<short> r1short && left is IVariableReferenceNumericalIn<int> lshort)
            {
                var r0v = r0short.EvaluateNumerical(context);
                var r1v = r1short.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        lshort.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        lshort.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        lshort.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        lshort.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (right0 is IVariableReferenceNumericalOut<ushort> r0ushort &&
                     right1 is IVariableReferenceNumericalOut<ushort> r1ushort && left is IVariableReferenceNumericalIn<int> lushort)
            {
                var r0v = r0ushort.EvaluateNumerical(context);
                var r1v = r1ushort.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        lushort.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        lushort.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        lushort.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        lushort.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (right0 is IVariableReferenceNumericalOut<int> r0int &&
                     right1 is IVariableReferenceNumericalOut<int> r1int && left is IVariableReferenceNumericalIn<int> lint)
            {
                var r0v = r0int.EvaluateNumerical(context);
                var r1v = r1int.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        lint.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        lint.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        lint.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        lint.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (right0 is IVariableReferenceNumericalOut<uint> r0uint &&
                     right1 is IVariableReferenceNumericalOut<uint> r1uint && left is IVariableReferenceNumericalIn<uint> luint)
            {
                var r0v = r0uint.EvaluateNumerical(context);
                var r1v = r1uint.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        luint.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        luint.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        luint.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        luint.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (right0 is IVariableReferenceNumericalOut<long> r0long &&
                     right1 is IVariableReferenceNumericalOut<long> r1long && left is IVariableReferenceNumericalIn<long> llong)
            {
                var r0v = r0long.EvaluateNumerical(context);
                var r1v = r1long.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        llong.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        llong.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        llong.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        llong.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (right0 is IVariableReferenceNumericalOut<ulong> r0ulong &&
                     right1 is IVariableReferenceNumericalOut<ulong> r1ulong && left is IVariableReferenceNumericalIn<ulong> lulong)
            {
                var r0v = r0ulong.EvaluateNumerical(context);
                var r1v = r1ulong.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        lulong.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        lulong.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        lulong.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        lulong.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (right0 is IVariableReferenceNumericalOut<float> r0float &&
                     right1 is IVariableReferenceNumericalOut<float> r1float && left is IVariableReferenceNumericalIn<float> lfloat)
            {
                var r0v = r0float.EvaluateNumerical(context);
                var r1v = r1float.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        lfloat.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        lfloat.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        lfloat.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        lfloat.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (right0 is IVariableReferenceNumericalOut<double> r0double &&
                     right1 is IVariableReferenceNumericalOut<double> r1double && left is IVariableReferenceNumericalIn<double> ldouble)
            {
                var r0v = r0double.EvaluateNumerical(context);
                var r1v = r1double.EvaluateNumerical(context);
                switch (calculationOperator)
                {
                    case CalculationOperatorType.Add:
                        ldouble.SetValue(context, r0v + r1v);
                        break;
                    case CalculationOperatorType.Subtract:
                        ldouble.SetValue(context, r0v - r1v);
                        break;
                    case CalculationOperatorType.Multiply:
                        ldouble.SetValue(context, r0v * r1v);
                        break;
                    case CalculationOperatorType.Divide:
                        ldouble.SetValue(context, r0v / r1v);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                throw new InvalidCastException("指定された変数の型に対する計算は実行できません。");
            }
        }
    }

    public enum CalculationOperatorType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
}