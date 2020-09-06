using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mukuro
{
    public class RegularScriptVariablesDatabase : IScriptVariablesDatabase
    {
        private Dictionary<string, RegularVariableStore> stores = new Dictionary<string, RegularVariableStore>();

        public IEnumerable<string> IdEntries => stores.Keys;
        
        public IVariableStore GetStore(string scriptAssetId)
        {
            if (stores.ContainsKey(scriptAssetId))
            {
                return stores[scriptAssetId];
            }
            else
            {
                var store = new RegularVariableStore();
                stores.Add(scriptAssetId, store);
                return store;
            } 
        }

        public bool HasKey(string scriptAssetId)
        {
            throw new System.NotImplementedException();
        }

    }
}