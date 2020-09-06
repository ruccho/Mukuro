using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Mukuro
{
    public static class Timer
    {
        private static List<TimerEntry> timers = new List<TimerEntry>();
        private static List<FrameTimerEntry> frameTimers = new List<FrameTimerEntry>();

        private static bool isInitialized = false;
        private static void EnsureInitialized() => Initialize();
        private static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;
            
            MukuroPlayerLoopRegisterer.AddListenerOnUpdate(Update);
        }
        
        public static void AddTimer(float time, Action tick, UpdateModeType updateMode)
        {
            EnsureInitialized();
            TimerEntry entry = null;
            switch (updateMode)
            {
                case UpdateModeType.Scaled:
                    entry = new TimerEntry(Time.time + time, UpdateModeType.Scaled, tick);
                    break;
                case UpdateModeType.Unscaled:
                    entry = new TimerEntry(Time.unscaledTime + time, UpdateModeType.Scaled, tick);
                    break;
                default:
                    throw new NotImplementedException();
            }
            timers.Add(entry);
        }
        
        public static void AddFrameTimer(int frames, Action tick)
        {
            EnsureInitialized();
            FrameTimerEntry entry = new FrameTimerEntry(frames, tick);
            frameTimers.Add(entry);
        }

        private static void Update()
        {
            for(int i = 0; i < timers.Count;)
            {
                var t = timers[i];
                switch (t.UpdateMode)
                {
                    case UpdateModeType.Scaled:
                        if (t.Tick <= Time.time)
                        {
                            timers.Remove(t);
                            t.TickEvent?.Invoke();
                        }
                        else i++;
                        break;
                    case UpdateModeType.Unscaled:
                        if (t.Tick <= Time.unscaledTime)
                        {
                            timers.Remove(t);
                            t.TickEvent?.Invoke();
                        }
                        else i++;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            for (int i = 0; i < frameTimers.Count;)
            {
                if (frameTimers[i].Update())
                {
                    frameTimers.RemoveAt(i);
                }
                else i++;
            }
        }

        public enum UpdateModeType
        {
            Scaled, Unscaled
        }

        private class TimerEntry
        {
            public TimerEntry(float tick, UpdateModeType updateMode, Action tickEvent)
            {
                Tick = tick;
                UpdateMode = updateMode;
                TickEvent = tickEvent;
            }

            public float Tick { get; }
            public UpdateModeType UpdateMode { get; }
            public Action TickEvent { get; }
        }
        
        private class FrameTimerEntry
        {
            public FrameTimerEntry(int frames, Action tickEvent)
            {
                FramesRemain = frames;
                TickEvent = tickEvent;
            }

            public bool Update()
            {
                if (ticked) return true;
                
                FramesRemain--;
                if (FramesRemain <= 0)
                {
                    TickEvent?.Invoke();
                    ticked = true;
                    return true;
                }

                return false;

            }

            public int FramesRemain { get; private set; }
            private Action TickEvent { get; }

            private bool ticked = false;

        }
    }
}