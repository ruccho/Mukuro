using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mukuro.Dialog
{
    public class DialogModule : EventScriptPlayerModule
    {
        [SerializeField] private DialogProvider[] providers = default;
        
        public DialogProvider CurrentProvider { get; private set; }
        
        public void Open(Action onOpened)
        {
            Open(null, onOpened);
        }
        public void Open(string providerName, Action onOpened)
        {
            if (CurrentProvider != null)
            {
                throw new InvalidOperationException("すでに別のDialogProviderが有効です。");
            }

            if (!string.IsNullOrEmpty(providerName))
            {
                CurrentProvider = providers.FirstOrDefault(p => p.ProviderName == providerName);
            }
            else
            {
                CurrentProvider = providers[0];
            }
            

            if (CurrentProvider == default)
            {
                throw new NullReferenceException($"指定されたDialogProvider \"{providerName}\" は見つかりませんでした。");
            }
            
            CurrentProvider.Open(onOpened);
        }

        public void Close(Action onClosed)
        {
            if(CurrentProvider == null) throw new NullReferenceException("有効なDialogPlayerがありません。");
            
            CurrentProvider.Close(onClosed);
            
            CurrentProvider = null;
        }

        public void ShowMessage(DialogShowMessageSettings settings, Action onNext)
        {
            if(CurrentProvider == null) throw new NullReferenceException("有効なDialogPlayerがありません。");
            CurrentProvider.ShowMessage(settings, onNext);
        }

        public void ShowMenu(DialogShowMenuSettings settings, Action<int> onSelected)
        {
            if(CurrentProvider == null) throw new NullReferenceException("有効なDialogPlayerがありません。");
            CurrentProvider.ShowMenu(settings, onSelected);
            
        }
        
    }
}