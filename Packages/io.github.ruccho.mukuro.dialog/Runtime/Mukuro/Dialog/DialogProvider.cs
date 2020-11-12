using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    public abstract class DialogProviderBase : MonoBehaviour
    {
        public abstract string ProviderName { get; }

        public abstract void Open(Action onOpened);
        public abstract void Close(Action onClosed);
        
        public abstract void ShowMessage(DialogShowMessageSettings settings, Action onNext);
        public abstract void ShowMenu(DialogShowMenuSettings settings, Action<int> onSelected);
    }

    public abstract class DialogProvider<TMessage> : DialogProviderBase
        where TMessage : DialogShowMessageSettings, new()
    {
        public sealed override void ShowMessage(DialogShowMessageSettings settings, Action onNext)
        {
            if (settings is TMessage s)
            {
                ShowMessage(s, onNext);
            }
            else
            {
                Debug.LogWarning($"このDialogProviderは{nameof(TMessage)}に対応していないため、いくつかのパラメータは無視されます。");
                s = new TMessage();
                s.CloneFrom(settings);
                ShowMessage(s, onNext);
            }
        }

        protected abstract void ShowMessage(TMessage settings, Action onNext);
    }
}