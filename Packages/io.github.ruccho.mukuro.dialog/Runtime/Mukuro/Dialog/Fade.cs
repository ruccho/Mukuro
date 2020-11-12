using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mukuro.Dialog
{
    public class Fade : MonoBehaviour
    {
        [SerializeField] private Graphic target = default;
        [SerializeField] private float alpha = default;
        
        public enum UpdateModeType
        {
            Scaled, Unscaled
        }
        
        [SerializeField] private UpdateModeType updateMode = default;
        private float time => (updateMode == UpdateModeType.Scaled ? Time.time : Time.unscaledTime);
        private float deltaTime => (updateMode == UpdateModeType.Scaled ? Time.deltaTime : Time.unscaledDeltaTime);
        
        private Action onNext;
        private bool IsFading => onNext != null;
        private float currentValue;
        private float fromValue;
        private float currentTarget;
        private float currentDuration;
        private float startTime;

        public void In(float duration, Action onNext)
        {
            SetTarget(1f, duration, onNext);
        }

        public void Out(float duration, Action onNext)
        {
            SetTarget(0f, duration, onNext);
        }
        
        private void SetTarget(float value, float duration, Action onNext)
        {
            Next();
            this.onNext = onNext;
            currentTarget = value;
            currentDuration = duration;
            startTime = time;
            fromValue = currentValue;
        }
        

        private void Update()
        {
            if (IsFading)
            {
                float r = (time - startTime) / currentDuration;
                float f = Mathf.Lerp(fromValue, currentTarget, r);
                var c = target.color;
                c.a = alpha * f;
                target.color = c;

                if (r >= 1f)
                {
                    currentValue = currentTarget;
                    Next();
                }

            }
        }

        private void Next()
        {
            onNext?.Invoke();
            onNext = null;
        }
        
    }
}