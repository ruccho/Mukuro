using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Mukuro.Timeline
{
    public class TimelineModule : EventScriptPlayerModule
    {
        private readonly TimelineSignalReceiver signalReceiver = new TimelineSignalReceiver();

        public event Action<SignalAsset> OnSignalNotify;

        private void Start()
        {
            signalReceiver.OnSignalNotify += SignalReceiverOnOnSignalNotify; 
        }

        private void SignalReceiverOnOnSignalNotify(Playable origin, INotification notification, object context)
        {
            var signal = notification as SignalEmitter;
            if (signal != null && signal.asset != null)
            {
                Debug.Log("Signal!");
                OnSignalNotify?.Invoke(signal.asset);
            }
        }


        public void RegisterSignalReceiver(PlayableGraph graph)
        {
            var outputCount = graph.GetOutputCount();
            for (int i = 0; i < outputCount; i++)
            {
                var output = graph.GetOutput(i);
                
                output.AddNotificationReceiver(signalReceiver);
            }
        }
    }
    
    class TimelineSignalReceiver : INotificationReceiver
    {
        public delegate void SignalNotifyEventHandler(Playable origin, INotification notification, object context);

        public event SignalNotifyEventHandler OnSignalNotify;
        
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            OnSignalNotify?.Invoke(origin, notification, context);
        }
    }
}