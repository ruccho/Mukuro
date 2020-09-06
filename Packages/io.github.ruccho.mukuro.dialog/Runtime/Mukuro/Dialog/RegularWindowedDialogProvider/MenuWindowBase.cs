using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro.Dialog
{
    public abstract class MenuWindowBase : MonoBehaviour
    {
        public abstract void Open(DialogShowMenuSettings settings, Action<int> onSelected);
    }
}