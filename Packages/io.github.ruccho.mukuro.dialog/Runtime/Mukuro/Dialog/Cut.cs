using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mukuro.Dialog
{
    public class Cut : MonoBehaviour
    {
        [SerializeField] private Image back = default;
        [SerializeField] private Image front = default;

        private Fade.UpdateModeType currentUpdateMode = Fade.UpdateModeType.Scaled;
        private float time => (currentUpdateMode == Fade.UpdateModeType.Scaled ? Time.time : Time.unscaledTime);

        private float deltaTime =>
            (currentUpdateMode == Fade.UpdateModeType.Scaled ? Time.deltaTime : Time.unscaledDeltaTime);

        private Sprite currentSprite = default;
        private Sprite nextSprite = default;
        private float currentSpeed = default;

        private float transition = 0;

        private Action OnNext { get; set; }

        public void Show(Sprite sprite, float fadeDuration, Fade.UpdateModeType updateMode, Action onNext)
        {
            if(sprite == null) throw new ArgumentNullException();
            
            currentSprite = nextSprite;
            nextSprite = sprite;
            
            if (fadeDuration <= 0)
            {
                OnNext?.Invoke();
                onNext?.Invoke();
                OnNext = null;
                transition = 1;
                ApplySprite();
                ApplyFade();
                return;
            }
            
            currentUpdateMode = updateMode;
            currentSpeed = 1f / fadeDuration;

            transition = 0;
            
            ApplySprite();
            ApplyFade();

            
            OnNext?.Invoke();
            OnNext = onNext;

        }
        
        public void Hide(float fadeDuration, Fade.UpdateModeType updateMode, Action onNext)
        {

            currentSprite = nextSprite;
            nextSprite = null;
            
            if (fadeDuration <= 0)
            {
                OnNext?.Invoke();
                onNext?.Invoke();
                OnNext = null;
                transition = 1;
                ApplySprite();
                ApplyFade();
                return;
            }
            
            currentUpdateMode = updateMode;
            currentSpeed = 1f / fadeDuration;

            transition = 0;
            
            ApplySprite();
            ApplyFade();
            
            OnNext?.Invoke();
            OnNext = onNext;
        }

        private void Update()
        {
            if (currentSprite == null && nextSprite == null) return;
            
            ApplyFade();

            if (transition >= 1)
            {
                OnNext?.Invoke();
                OnNext = null;
            }
                
            transition += currentSpeed * deltaTime;
            transition = Mathf.Clamp01(transition);
        }

        private void ApplyFade()
        {
            if (nextSprite != null)
            {
                //show
                if (currentSprite == null)
                {
                    SetImageAlpha(back, 0f);
                }
                else
                {
                    SetImageAlpha(back, 1f);
                }

                SetImageAlpha(front, transition);

            }
            else
            {
                //hide
                SetImageAlpha(back, 1f - transition);
                SetImageAlpha(front, 0f);
            }

        }

        private void ApplySprite()
        {
            back.sprite = currentSprite;
            front.sprite = nextSprite;
        }

        private static void SetImageAlpha(Image image, float alpha)
        {
            var c = image.color;
            c.a = alpha;
            image.color = c;
        }
    }
}