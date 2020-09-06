using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mukuro
{
    public interface IVariableStore
    {
        object GetValue(string key);
        T GetValue<T>(string key);
        void SetValue<T>(string key, T value);
        bool HasKey(string key);
        Type GetValueType(string key);

        IEnumerable<string> KeyEntries { get; }
    }

    public static class VariableStoreExtension
    {
        public static bool TryGetValue<T>(this IVariableStore store, string key, out T value)
        {
            Type t;
            if (store.TryGetValueType(key, out t) && t != typeof(T))
            {
                value = default;
                return false;
            }

            value = store.GetValue<T>(key);
            return true;

        }

        public static bool TryGetValueType(this IVariableStore store, string key, out Type type)
        {
            if (!store.HasKey(key))
            {
                type = default;
                return false;
            }

            type = store.GetValueType(key);
            return true;

        }
    }
}