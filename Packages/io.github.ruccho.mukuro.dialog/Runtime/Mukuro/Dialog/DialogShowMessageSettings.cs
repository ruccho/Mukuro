using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

namespace Mukuro.Dialog
{
    [Serializable]
    public class DialogShowMessageSettings
    {
        public DialogShowMessageSettings()
        {
        }

        public DialogShowMessageSettings(ISpeakerInfo speakerInfo, string face, string message, bool allowSpeedUp,
            bool allowSkipping)
        {
            SpeakerInfo = speakerInfo;
            Face = face;
            Message = message;
            AllowSpeedUp = allowSpeedUp;
            AllowSkipping = allowSkipping;
        }

        public virtual void CloneFrom(DialogShowMessageSettings source)
        {
            speakerInfo = source.speakerInfo;
            SpeakerInfo = source.SpeakerInfo;
            Face = source.Face;
            Message = source.Message;
            AllowSpeedUp = source.AllowSpeedUp;
            AllowSkipping = source.AllowSkipping;
        }

        public ISpeakerInfo SpeakerInfo
        {
            get
            {
                if (speakerInfoConverted != null) return speakerInfoConverted;
                return (speakerInfo?.SpeakerInfo is ISpeakerInfo s) ? s : null;
            }
            private set => speakerInfoConverted = value;
        }

        private ISpeakerInfo speakerInfoConverted = default;
        [SerializeField] private SpeakerInfoAsset speakerInfo;

        public string Face
        {
            get => face;
            private set => face = value;
        }

        [SerializeField] private string face = default;

        public string Message
        {
            get => message;
            private set => message = value;
        }

        [SerializeField] private string message = default;

        public bool AllowSpeedUp
        {
            get => allowSpeedUp;
            private set => allowSpeedUp = value;
        }

        [SerializeField] private bool allowSpeedUp = true;

        public bool AllowSkipping
        {
            get => allowSkipping;
            private set => allowSkipping = value;
        }

        [SerializeField] private bool allowSkipping = true;
    }
}