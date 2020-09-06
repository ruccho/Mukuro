using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    public abstract class DialogProvider : MonoBehaviour
    {
        public abstract string ProviderName { get; }

        public abstract void Open(Action onOpened);
        public abstract void Close(Action onClosed);
        
        public abstract void ShowMessage(DialogShowMessageSettings settings, Action onNext);
        public abstract void ShowMenu(DialogShowMenuSettings settings, Action<int> onSelected);
    }
}