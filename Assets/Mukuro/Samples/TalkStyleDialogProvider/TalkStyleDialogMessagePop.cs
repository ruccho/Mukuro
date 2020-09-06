using System;
using System.Collections;
using System.Collections.Generic;
using Mukuro.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace Mukuro.Samples
{
    public class TalkStyleDialogMessagePop : MonoBehaviour
    {
        [SerializeField] private string accelButton = "Fire1";
        [SerializeField] private string skipButton = "Fire2";
        [SerializeField] private float speakSpeed = 20;
        [SerializeField] private Text messageText = default;

        private DialogShowMessageSettings CurrentSettings { get; set; } = null;
        private string CurrentMessage => CurrentSettings?.Message;
        private float speakingPointer;

        private float SpeakingPointer
        {
            get => speakingPointer;
            set
            {
                if (CurrentMessage != null)
                {
                    speakingPointer = Mathf.Clamp(value, 0, CurrentMessage.Length);
                    messageText.text = CurrentMessage.Substring(0, Mathf.FloorToInt(speakingPointer));
                }
                else
                {
                    messageText.text = "";
                    speakingPointer = value;
                }
            }
        }
        private Action OnNext { get; set; }
        
        private bool textingCompleted = false;

        private bool popCompleted = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        public void SetMessage(DialogShowMessageSettings settings, Action onNext)
        {
            CurrentSettings = settings;
            OnNext = onNext;
        }

        // Update is called once per frame
        void Update()
        {
            if (popCompleted) return;
            if (CurrentSettings == null) return;
            
            if (!textingCompleted)
            {
                
                if (CurrentSettings.AllowSkipping && Input.GetButtonDown(skipButton))
                {
                    SpeakingPointer = CurrentMessage.Length;
                }
                else if (CurrentSettings.AllowSpeedUp && Input.GetButton(accelButton))
                {
                    SpeakingPointer += speakSpeed * Time.deltaTime * 5f;
                }
                else
                {
                    SpeakingPointer += speakSpeed * Time.deltaTime;
                }

                if (SpeakingPointer >= CurrentMessage.Length)
                {
                    textingCompleted = true;
                }
            }
            else
            {
                if (Input.GetButtonDown(accelButton) || Input.GetButtonDown(skipButton))
                {
                    OnNext?.Invoke();
                    popCompleted = true;
                }
            }

        }
    }
}