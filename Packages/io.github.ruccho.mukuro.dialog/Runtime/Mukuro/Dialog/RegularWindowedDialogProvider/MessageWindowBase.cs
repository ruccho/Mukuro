using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    public abstract class MessageWindowBase : MonoBehaviour
    {
        public abstract void Open(Action onOpened);

        public abstract void Close(Action onClosed);

        public abstract void Show(DialogShowMessageSettings settings, Action onNext);
        
    }
}