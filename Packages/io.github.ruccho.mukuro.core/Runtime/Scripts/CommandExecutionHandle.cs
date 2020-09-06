using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    /// <summary>
    /// イベントコマンドの実行時にコマンド側に渡れる。
    /// CommandExecutionHandle.Complete()でコマンドの実行完了を通知します。
    /// </summary>
    public class CommandExecutionHandle
    {
        public bool IsCompleted { get; private set; } = false;
        private Action OnCompleted;
        public CommandExecutionHandle(Action onCompleted)
        {
            OnCompleted = onCompleted;
        }
        
        public void Complete()
        {
            if (IsCompleted)
            {
                Debug.LogWarning("CommandExecutionHandle.Complete()が2度以上呼ばれました。");
                return;
            }
            IsCompleted = true;
            OnCompleted?.Invoke();
        }
    }
}