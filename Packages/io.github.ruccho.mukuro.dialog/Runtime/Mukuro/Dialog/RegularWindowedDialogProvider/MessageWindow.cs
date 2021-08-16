using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Mukuro.Dialog
{
    public class MessageWindow : MessageWindowBase
    {
        [SerializeField] private string accelButton = "Fire1";
        [SerializeField] private string skipButton = "Fire2";
        [SerializeField] private Image messageWindow = default;
        [SerializeField] private float fadeSpeed = 4;
        [SerializeField] private float windowDefaultFade = 0.45f;
        [SerializeField] private float speakSpeed = 20;
        [SerializeField] private Text messageText = default;
        [SerializeField] private Text speakerNameText = default;
        [SerializeField] private Image faceImage = default;

        private float windowFade;

        private float WindowFade
        {
            get => windowFade;
            set
            {
                windowFade = value;

                Color c = messageWindow.color;
                c.a = value * windowDefaultFade;
                messageWindow.color = c;

                c = messageText.color;
                c.a = value;
                messageText.color = c;
            }
        }

        private MessageWindowState State { get; set; }

        private enum MessageWindowState
        {
            None,
            Closed,
            Opening,
            Opened,
            Speaking,
            Spoke,
            Closing
        }

        private Action OnNext { get; set; }

        private DialogShowMessageSettings CurrentSettings { get; set; }
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

        private CancellationTokenSource FaceSpriteLoadingCancellation { get; set; }

        private void Start()
        {
            State = MessageWindowState.Closed;
        }

        public override void Open(Action onNext)
        {
            CurrentSettings = null;
            SpeakingPointer = 0;
            SetState(MessageWindowState.Opening, onNext);
        }

        public override void Close(Action onNext)
        {
            CancelCurrentFaceSpriteLoading();
            SetFaceSprite(null);
            speakerNameText.text = null;
            SetState(MessageWindowState.Closing, onNext);
        }

        public override void Show(DialogShowMessageSettings settings, Action onNext)
        {
            CurrentSettings = settings;
            SpeakingPointer = 0;
            speakerNameText.text = settings.SpeakerInfo?.DisplayName;

            CancelCurrentFaceSpriteLoading();
            FaceSpriteLoadingCancellation = new CancellationTokenSource();

            //Debug.Log("Start Loading");
            LoadFaceSpriteAsync(settings, FaceSpriteLoadingCancellation.Token).Forget();
            //Debug.Log("Continue...");

            SetState(MessageWindowState.Speaking, onNext);
        }
        
        private void CancelCurrentFaceSpriteLoading()
        {
            if (FaceSpriteLoadingCancellation != null && !FaceSpriteLoadingCancellation.IsCancellationRequested)
                FaceSpriteLoadingCancellation.Cancel();
            
        }
        

        private async Task LoadFaceSpriteAsync(DialogShowMessageSettings settings, CancellationToken cancellationToken)
        {
            try
            {
                Sprite loaded = null;
                if (settings.SpeakerInfo != null)
                {
                    loaded = await settings.SpeakerInfo.GetFaceSpriteAsync(settings.Face, cancellationToken);
                }

                //Debug.Log("Loaded");

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                SetFaceSprite(loaded);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }

        }

        private void SetFaceSprite(Sprite sprite)
        {
            faceImage.sprite = sprite;
            if (sprite != null)
            {
                faceImage.color = Color.white;
            }
            else
            {
                faceImage.color = Color.clear;
            }
        }

        private void SetState(MessageWindowState s, Action onNext)
        {
            OnNext = onNext;
            State = s;
            //Debug.Log(State);
        }

        private void Update()
        {
            switch (State)
            {
                case MessageWindowState.None:
                    break;
                case MessageWindowState.Closed:
                    break;
                case MessageWindowState.Opening:
                    WindowFade += fadeSpeed * Time.deltaTime;
                    if (WindowFade >= 1)
                    {
                        WindowFade = 1;
                        var onNext = OnNext;
                        SetState(MessageWindowState.Opened, null);
                        onNext?.Invoke();
                    }

                    break;
                case MessageWindowState.Opened:
                    break;
                case MessageWindowState.Speaking:

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
                        SetState(MessageWindowState.Spoke, OnNext);
                    }

                    break;
                case MessageWindowState.Spoke:
                    if (Input.GetButtonDown(accelButton) || Input.GetButtonDown(skipButton))
                    {
                        var onNext = OnNext;
                        SetState(MessageWindowState.Opened, null);
                        onNext?.Invoke();
                    }

                    break;
                case MessageWindowState.Closing:
                    WindowFade -= fadeSpeed * Time.deltaTime;
                    if (WindowFade <= 0)
                    {
                        WindowFade = 0;
                        var onNext = OnNext;
                        SetState(MessageWindowState.Closed, null);
                        onNext?.Invoke();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}