using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mukuro
{
    /// <summary>
    /// EventScriptAssetとScriptVariablesを紐づけたデータベース。
    /// EventPlayerが保持し、イベントスクリプトの実行時に参照する。
    /// </summary>
    public interface IScriptVariablesDatabase
    {
        IVariableStore GetStore(string scriptAssetId);
        bool HasKey(string scriptAssetId);

        IEnumerable<string> IdEntries { get; }
    }

    public static class ScriptVariablesDatabaseExtension
    {
        public static IVariableStore GetStore(this IScriptVariablesDatabase database, EventScriptAsset scriptAsset)
        {
            return database.GetStore(scriptAsset.Id);
        }
        public static bool HasKey(this IScriptVariablesDatabase database, EventScriptAsset scriptAsset)
        {
            return database.HasKey(scriptAsset.Id);
        }
    }
    
}