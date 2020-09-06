using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    public class RegularWindowedDialogProvider : DialogProvider
    {
        [SerializeField] private MessageWindowBase messageWindow = default;
        [SerializeField] private MenuWindowBase menuWindow = default;
        public override string ProviderName => "RegularWindowed";
        public override void Open(Action onOpened)
        {
            messageWindow.Open(onOpened);
        }

        public override void Close(Action onClosed)
        {
            messageWindow.Close(onClosed);
        }

        public override void ShowMessage(DialogShowMessageSettings settings, Action onNext)
        {
            messageWindow.Show(settings, onNext);
        }

        public override void ShowMenu(DialogShowMenuSettings settings, Action<int> onSelected)
        {
            menuWindow.Open(settings, onSelected);
        }
    }
}