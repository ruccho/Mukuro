using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mukuro
{
    public class SubscriptionHandle : IDisposable
    {
        private Action onDisposed;
        public SubscriptionHandle(Action onDisposed)
        {
            this.onDisposed = onDisposed;
        }

        public void Dispose()
        {
            onDisposed?.Invoke();
            onDisposed = null;
        }
        
        public static SubscriptionHandle Empty = new SubscriptionHandle(null);
    }
}