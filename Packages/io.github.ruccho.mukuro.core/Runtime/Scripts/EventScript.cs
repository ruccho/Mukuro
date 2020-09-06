using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    [Serializable]
    public class EventScript
    {
        [SerializeField] public EventCommandList CommandList;
        /// <summary>
        /// 同期実行のコマンドがすべて実行された後、非同期実行のコマンドの終了を待ちます。
        /// </summary>
        [SerializeField] public bool WaitForAllCommands;
    }
}