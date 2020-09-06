using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mukuro
{
    public class RegularVariableStore : IVariableStore
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public IEnumerable<string> KeyEntries => data.Keys;

        public object GetValue(string key)
        {
            return data[key];
        }

        public T GetValue<T>(string key)
        {
            return (T)data[key];
        }

        public void SetValue<T>(string key, T value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        public bool HasKey(string key)
        {
            return data.ContainsKey(key);
        }

        public Type GetValueType(string key)
        {
            return GetValue(key).GetType();
        }
    }
}