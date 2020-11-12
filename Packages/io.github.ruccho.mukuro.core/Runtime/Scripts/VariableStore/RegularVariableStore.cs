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

        private Dictionary<string, List<Action<object>>> onUpdatedActions = new Dictionary<string, List<Action<object>>>();
        
        public SubscriptionHandle SubscribeOnUpdated<T>(string key, Action<T> onUpdated)
        {
            if (!onUpdatedActions.ContainsKey(key))
            {
                onUpdatedActions.Add(key, new List<Action<object>>());
            }

            Action<object> onUpdatedObject = (valueObject) => { onUpdated((T) valueObject); };
            onUpdatedActions[key].Add(onUpdatedObject);
            
            return new SubscriptionHandle(() => { onUpdatedActions[key].Remove(onUpdatedObject); });
        }

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

            if (onUpdatedActions.ContainsKey(key))
            {
                foreach(var onUpdated in onUpdatedActions[key]) onUpdated.Invoke(value);
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