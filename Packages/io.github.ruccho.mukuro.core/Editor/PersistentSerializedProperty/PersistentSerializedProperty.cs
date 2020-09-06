using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Mukuro.Editors
{
    /// <summary>
    /// SerializedPropertyで配列に対して要素の挿入(Insert)や削除(Delete)を行った際、その子要素のインデックスがずれてデータを見失うのを防ぎます。
    /// 具体的にはPersistentSerializedPropertyを通して配列の要素に対して操作を行うと、その子要素のPersistentSerializedPropertyはインデックスを自動で追跡し、常に同じデータを参照できるようにします。
    /// </summary>
    public class PersistentSerializedProperty
    {
        public bool IsArray { get; private set; }
        private List<PersistentSerializedProperty> ArrayElements { get; } = new List<PersistentSerializedProperty>();
        private int ArrayIndex { get; set; } = -1;

        public SerializedObject SerializedObject { get; }
        
        private PersistentSerializedProperty PivotParent { get; }
        private string RelativePath { get; set; }
        
        private Dictionary<string, PersistentSerializedProperty> Children { get; } = new Dictionary<string, PersistentSerializedProperty>();

        public string AbsolutePath
        {
            get
            {
                if (PivotParent != null)
                {
                    return $"{PivotParent.AbsolutePath}.{RelativePath}";
                }
                else
                {
                    return RelativePath;
                }
            }
        }

        public SerializedProperty GetProperty()
        {
            //Debug.Log(absolutePath);
            return this.SerializedObject.FindProperty(AbsolutePath);
        }
        
        
        /// <summary>
        /// あるSerializedPropertyからPersistentSerializedPropertyを生成する。こいつのパスが固定であることを前提としているため、親にArrayが存在しないことが望ましい。
        /// </summary>
        /// <param name="propertyToTrack"></param>
        public PersistentSerializedProperty(SerializedProperty propertyToTrack)
        {
            this.RelativePath = propertyToTrack.propertyPath;
            this.SerializedObject = propertyToTrack.serializedObject;
            InitArray();
        }

        /// <summary>
        /// あるPersistentSerializedPropertyを起点として、そこからの相対パスでPersistentSerializedPropertyを生成する。
        /// この方法で生成したPersistentSerializedPropertyは親要素のインデックス変更を追跡する。
        /// </summary>
        /// <param name="pivotParent"></param>
        /// <param name="relativePath"></param>
        private PersistentSerializedProperty(PersistentSerializedProperty pivotParent, string relativePath)
        {
            this.PivotParent = pivotParent;
            this.RelativePath = relativePath;
            this.SerializedObject = pivotParent.SerializedObject;
            InitArray();
        }

        /// <summary>
        /// あるArrayのPersistentSerializedPropertyの子要素を取得する。
        /// この方法で生成したPersistentSerializedPropertyは親要素のインデックス変更を追跡する。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        private PersistentSerializedProperty(PersistentSerializedProperty array, int index)
        {
            this.PivotParent = array;
            this.SerializedObject = array.SerializedObject;
            SetArrayIndex(index);
            InitArray();
        }

        /// <summary>
        /// あるPersistentSerializedPropertyを起点として、そこからの相対パスでPersistentSerializedPropertyを生成する。
        /// この方法で生成したPersistentSerializedPropertyは親要素のインデックス変更を追跡する。
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public PersistentSerializedProperty GetChildProperty(string relativePath)
        {
            string[] splitted = relativePath.Split('.');
            if (splitted.Length == 1)
            {
                return GetDirectChild(relativePath);
            }
            else
            {
                int first = relativePath.IndexOf('.');
                string childPath = relativePath.Substring(0, first);
                var child = GetDirectChild(childPath);
                return child.GetChildProperty(relativePath.Substring(first + 1));
            }
        }

        private PersistentSerializedProperty GetDirectChild(string key)
        {
            if (Children.ContainsKey(key))
            {
                return Children[key];
            }
            var n = new PersistentSerializedProperty(this, key);
            Children.Add(key, n);
            return n;
        }
        

        /// <summary>
        /// 配列の直接的な子要素に対して、インデックスを更新する。
        /// </summary>
        /// <param name="index"></param>
        private void SetArrayIndex(int index)
        {
            this.RelativePath = $"Array.data[{index}]";
            ArrayIndex = index;
            InitArray();
        }

        public int GetArrayIndex()
        {
            return ArrayIndex;
        }

        private void InitArray()
        {
            var prop = GetProperty();
            if (prop == null) return;
            IsArray = prop.isArray;
            if (IsArray)
            {
                for (int i = 0; i < prop.arraySize; i++)
                {
                    ArrayElements.Add(new PersistentSerializedProperty(this, i));
                }
            }
        }

        public int GetArraySize()
        {
            EnsureThisIsArray();
            CheckArraySizeCorruption();
            
            return ArrayElements.Count;
        }

        public PersistentSerializedProperty GetArrayElementAt(int index)
        {
            EnsureThisIsArray();
            CheckArraySizeCorruption();
            
            return ArrayElements[index];
        }

        public PersistentSerializedProperty InsertArrayElementAt(int index)
        {
            EnsureThisIsArray();
            CheckArraySizeCorruption();
            
            var prop = GetProperty();
            
            prop.InsertArrayElementAtIndex(index);

            //Apply insert
            prop.serializedObject.ApplyModifiedProperties();
            
            var insertedElementProp = new PersistentSerializedProperty(this, "");
            ArrayElements.Insert(index, insertedElementProp);
            
            //Rebind relativePath of children
            for (int i = 0; i < prop.arraySize; i++)
            {
                ArrayElements[i].SetArrayIndex(i);
            }

            return insertedElementProp;
        }

        public void DeleteArrayElementAt(int index)
        {
            EnsureThisIsArray();
            CheckArraySizeCorruption();
            
            var prop = GetProperty();
            prop.DeleteArrayElementAtIndex(index);
            
            //Apply delete
            prop.serializedObject.ApplyModifiedProperties();
            
            ArrayElements.RemoveAt(index);
            
            //Rebind relativePath of children
            for (int i = 0; i < prop.arraySize; i++)
            {
                ArrayElements[i].SetArrayIndex(i);
            }
        }

        private void EnsureThisIsArray()
        {
            if(!IsArray) throw new InvalidOperationException("This PersistentSerializedProperty is not an array");
        }

        private bool CheckArraySizeCorruption()
        {
            var prop = GetProperty();

            if (prop.arraySize != ArrayElements.Count)
            {
                Debug.LogWarning($"Array size corruption detected. {ArrayElements.Count} but actually {prop.arraySize}. I am {AbsolutePath}");
                return true;
            }

            return true;
        }
        
    }
}